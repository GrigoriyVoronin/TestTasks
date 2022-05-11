using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ORM.Contracts;
using ORM.Db;

namespace ORM
{
    #region Context

    public class DataContext : IDataContext
    {
        private readonly Buffer addBuffer;
        private readonly IDbEngine dbEngine;
        private readonly Buffer immutableBuffer;
        private readonly Buffer updBuffer;

        public DataContext(IDbEngine dbEngine)
        {
            this.dbEngine = dbEngine;
            addBuffer = new Buffer();
            updBuffer = new Buffer();
            immutableBuffer = new Buffer();
        }

        public T Read<T>(string id) where T : DbEntity
        {
            return GetMethod<T>(id,
                () => throw new ArgumentException($"Ёлемент с Id={id} не найден"));
        }

        public T Find<T>(string id) where T : DbEntity
        {
            return GetMethod<T>(id, () => null);
        }

        public void Insert<T>(T entity) where T : DbEntity
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            addBuffer.AddOrUpdate(entity);
        }

        public void SubmitChanges()
        {
            var submitRequest = Request.GetSubmitRequest(addBuffer, updBuffer, immutableBuffer);
            var response = dbEngine.SendRequest(submitRequest);
            if (!response.IsSuccessful)
                throw new InvalidOperationException("ќшибка при отправке данных");

            UpdateBuffersAfterSubmit();
        }

        private T GetMethod<T>(string strId, Func<T> badResponseAnswerer) where T : DbEntity
        {
            var id = new EntityId(strId);
            if (updBuffer.TryGetValue<T>(id, out var entity))
                return entity;

            var getRequest = Request.GetGetRequest(id);
            var response = dbEngine.SendRequest(getRequest);
            if (!response.IsSuccessful)
                return badResponseAnswerer.Invoke();

            entity = GetResponseParser.ParseGetResponse<T>(response);
            UpdateBuffersAfterGet(entity);
            return entity;
        }

        private void UpdateBuffersAfterGet(DbEntity dbEntity)
        {
            updBuffer.AddOrUpdate(dbEntity);
            immutableBuffer.AddOrUpdate(dbEntity.GetSameEntityWithOtherReference());
        }

        private void UpdateBuffersAfterSubmit()
        {
            updBuffer.ForAll(e => immutableBuffer
                .AddOrUpdate(e.GetSameEntityWithOtherReference()));
            addBuffer.ForAll(UpdateBuffersAfterGet);
            addBuffer.Clear();
        }
    }

    internal sealed class Buffer : IEnumerable<DbEntity>
    {
        private Dictionary<EntityId, DbEntity> items;

        public Buffer()
        {
            items = new Dictionary<EntityId, DbEntity>();
        }

        public IEnumerator<DbEntity> GetEnumerator() => items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool TryGetValue<T>(EntityId id, out T entity) where T : DbEntity
        {
            if (items.ContainsKey(id))
            {
                entity = GetValue<T>(id);
                return true;
            }

            entity = null;
            return false;
        }

        public T GetValue<T>(EntityId id) where T : DbEntity => (T) items[id];

        public void AddOrUpdate(DbEntity entity)
        {
            var id = new EntityId(entity.Id);
            items[id] = entity;
        }

        public void Clear()
        {
            items = new Dictionary<EntityId, DbEntity>();
        }
    }

    internal sealed class EntityId
    {
        public EntityId(string id)
        {
            Value = id;
        }

        public string Value { get; }

        public override bool Equals(object obj) => obj is EntityId id && Equals(id);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public bool Equals(EntityId id) => Value == id.Value;

        public override string ToString() => Value;
    }

    #endregion

    #region DbMassages

    internal abstract class DbMessage
    {
        protected DbMessage(string message, MessageType messageType)
        {
            Value = message;
            MessageType = messageType;
        }

        public string Value { get; }
        public MessageType MessageType { get; }
        public override string ToString() => Value;
    }

    internal enum MessageType
    {
        Add,
        Get,
        Upd,
        Submit
    }

    internal sealed class Request : DbMessage
    {
        private Request(string requestStr, MessageType type)
            : base(requestStr, type)
        {
        }

        public static Request GetGetRequest(EntityId id) => new Request($"get Id={id};", MessageType.Get);

        public static Request GetSubmitRequest(Buffer addBuffer, Buffer updBuffer,
            Buffer immutableBuffer)
        {
            var requestBuilder = new StringBuilder();
            BuildAddRequests(addBuffer, requestBuilder);
            BuildUpdRequests(updBuffer, immutableBuffer, requestBuilder);

            return new Request(requestBuilder.ToString(), MessageType.Submit);
        }

        private static void BuildUpdRequests(Buffer updBuffer, Buffer immutableBuffer,
            StringBuilder requestBuilder)
        {
            updBuffer.ForAll(item =>
            {
                var id = new EntityId(item.Id);
                var immutableItem = immutableBuffer.GetValue<DbEntity>(id);
                var updRequest = GetUpdRequest(item, immutableItem);
                requestBuilder.Append(updRequest != null ? updRequest.Value : "");
            });
        }

        private static void BuildAddRequests(Buffer addBuffer, StringBuilder requestBuilder)
        {
            addBuffer.ForAll(item =>
            {
                var type = item.GetType();
                var defaultItem = type.GetDefaultEntity<DbEntity>();
                var addRequest = GetAddRequest(item, defaultItem);
                requestBuilder.Append(addRequest);
            });
        }

        public static Request GetUpdRequest(DbEntity updItem, DbEntity immutableItem) =>
            GetPostRequest(updItem, immutableItem, MessageType.Upd);

        public static Request GetAddRequest(DbEntity addItem, DbEntity defaultItem) =>
            GetPostRequest(addItem, defaultItem, MessageType.Add);

        private static Request GetPostRequest(DbEntity item, DbEntity compareItem, MessageType type)
        {
            var serializeItem = item.SerializeItemForPostRequest(compareItem, type);
            return serializeItem == null ? null : new Request($"{type.ToString().ToLower()} {serializeItem}", type);
        }
    }

    internal sealed class Response : DbMessage
    {
        private const string GetBadResponse = ";";
        private const string SuccessPostResponse = "ok;";

        private static readonly Regex SuccessRegex = new Regex(SuccessPostResponse,
            RegexOptions.Compiled);

        public Response(string responseStr, MessageType type)
            : base(responseStr, type)
        {
            IsSuccessful = CheckSuccess();
        }

        public bool IsSuccessful { get; }

        private bool CheckSuccess()
        {
            switch (MessageType)
            {
                case MessageType.Get:
                    return GetBadResponse != Value;
                case MessageType.Add:
                    return SuccessRegex.IsMatch(Value);
                case MessageType.Upd:
                    return SuccessRegex.IsMatch(Value);
                case MessageType.Submit:
                    return SuccessRegex
                        .Replace(Value, "") == "";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal static class GetResponseParser
    {
        public static T ParseGetResponse<T>(Response response) where T : DbEntity =>
            response.MessageType != MessageType.Get ? null : CreateEntity<T>(response.Value);

        private static T CreateEntity<T>(string responseValue) where T : DbEntity
        {
            var type = typeof(T);
            var propertiesValues = GetPropertiesFromValue(responseValue);
            var properties = type.GetProperties();
            var entity = type.GetDefaultEntity<T>();
            propertiesValues.ForAll(pair =>
            {
                var (name, value) = pair;
                var property = properties.FirstOrDefault(p => p.Name == name);
                entity.SetPropertyValue(property, value);
            });

            return entity;
        }

        private static IEnumerable<(string, string)> GetPropertiesFromValue(string value)
        {
            var nameBuilder = new StringBuilder();
            var valueBuilder = new StringBuilder();
            var isSeparatorFound = false;
            var currentIndex = -1;
            while (++currentIndex < value.Length - 1)
                switch (value[currentIndex])
                {
                    case '\\':
                        currentIndex++;
                        AppendNextSymbol(nameBuilder, isSeparatorFound, valueBuilder,
                            value, currentIndex);
                        break;
                    case '=':
                        isSeparatorFound = true;
                        break;
                    case ',':
                        yield return GetStringsPair(nameBuilder, valueBuilder);

                        isSeparatorFound = false;
                        nameBuilder.Clear();
                        valueBuilder.Clear();
                        break;
                    default:
                        AppendNextSymbol(nameBuilder, isSeparatorFound, valueBuilder,
                            value, currentIndex);
                        break;
                }

            yield return GetStringsPair(nameBuilder, valueBuilder);
        }

        private static (string, string) GetStringsPair(StringBuilder nameBuilder,
            StringBuilder valueBuilder) =>
            (nameBuilder.ToString(), valueBuilder.ToString());

        private static void AppendNextSymbol(StringBuilder nameBuilder, bool isSeparatorFound,
            StringBuilder valueBuilder, string value, int currentIndex)
        {
            if (isSeparatorFound)
                valueBuilder.Append(value[currentIndex]);
            else
                nameBuilder.Append(value[currentIndex]);
        }
    }

    #endregion

    #region Extensions

    public static class EnumerableExtensions
    {
        public static void ForAll<T>(this IEnumerable<T> set, Action<T> action)
        {
            foreach (var item in set)
                action(item);
        }
    }

    internal static class DbEngineExtensions
    {
        public static Response SendRequest(this IDbEngine dbEngine, Request request) =>
            new Response(dbEngine.Execute(request.Value), request.MessageType);
    }

    internal static class PropertyInfoExtensions
    {
        public static string SerializeProperty(this PropertyInfo property, DbEntity item)
        {
            const string escapedChars = @";\,=";
            var valueStringBuilder = new StringBuilder();
            var propertyValue = property.GetStringValueFromProperty(item);
            var currentIndex = 0;
            while (currentIndex < propertyValue.Length)
                if (escapedChars.Contains(propertyValue[currentIndex]))
                    valueStringBuilder.Append($"\\{propertyValue[currentIndex++]}");
                else
                    valueStringBuilder.Append(propertyValue[currentIndex++]);

            return $"{property.Name}={valueStringBuilder},";
        }

        public static string GetStringValueFromProperty(this PropertyInfo property, DbEntity item)
        {
            var propertyValueObj = property.GetValue(item);
            var typeProperty = propertyValueObj.GetType();
            var toString = typeProperty.GetToStringMethodWithCulture();
            if (toString != null)
                return (string) toString.Invoke(propertyValueObj, new object[] {CultureInfo.InvariantCulture});

            toString = typeProperty.GetDefaultToStringMethod();
            return (string) toString.Invoke(propertyValueObj, new object[] { });
        }
    }

    internal static class DbEntityExtensions
    {
        public static void SetPropertyValue<T>(this T entity, PropertyInfo property, string value)
            where T : DbEntity
        {
            var objValue = entity.GetParsePropertyValue(property, value);
            property.SetValue(entity, objValue);
        }

        public static object GetParsePropertyValue<T>(this T entity, PropertyInfo property, string strValue)
            where T : DbEntity
        {
            var propertyType = property.PropertyType;
            var parseMethod = propertyType.GetParseMethodWithCulture();
            if (parseMethod != null)
                return parseMethod.Invoke(entity, new object[] {strValue, CultureInfo.InvariantCulture});

            parseMethod = propertyType.GetDefaultParseMethod();
            return parseMethod?.Invoke(entity, new object[] {strValue}) ?? strValue;
        }

        public static DbEntity GetSameEntityWithOtherReference(this DbEntity entity)
        {
            var type = entity.GetType();
            var defaultEntity = type.GetDefaultEntity<DbEntity>();
            var properties = type.GetProperties();
            properties.ForAll(p =>
            {
                var propertyValue = p.GetValue(entity);
                p.SetValue(defaultEntity, propertyValue);
            });

            return defaultEntity;
        }

        public static string SerializeItemForPostRequest(this DbEntity item,
            DbEntity compareItem, MessageType messageType)
        {
            if (messageType != MessageType.Upd && messageType != MessageType.Add)
                return null;

            var requestBuilder = new StringBuilder();
            var type = item.GetType();
            var properties = type.GetProperties();
            var propertiesCounter = 0;
            properties.ForAll(p =>
            {
                var propertyValue = p.GetValue(item);
                var defaultValue = p.GetValue(compareItem);
                if (propertyValue?.Equals(defaultValue) != false && p.Name != "Id")
                    return;

                propertiesCounter++;
                requestBuilder.Append(p.SerializeProperty(item));
            });

            requestBuilder.Remove(requestBuilder.Length - 1, 1).Append(";");

            return messageType == MessageType.Add ? requestBuilder.ToString()
                : propertiesCounter > 1 ? requestBuilder.ToString() : null;
        }
    }

    internal static class TypeExtensions
    {
        public static T GetDefaultEntity<T>(this Type type)
        {
            var defaultConstructor = type.GetConstructor(new Type[] { });
            return (T) defaultConstructor?.Invoke(null);
        }

        public static MethodInfo GetParseMethodWithCulture(this Type type)
        {
            return type.GetMethod("Parse", new[] {typeof(string), typeof(CultureInfo)});
        }

        public static MethodInfo GetDefaultParseMethod(this Type type)
        {
            return type.GetMethod("Parse", new[] {typeof(string)});
        }

        public static MethodInfo GetToStringMethodWithCulture(this Type type)
        {
            return type.GetMethod("ToString", new[] {typeof(CultureInfo)});
        }

        public static MethodInfo GetDefaultToStringMethod(this Type type)
        {
            return type.GetMethod("ToString", new Type[] { });
        }
    }

    #endregion
}