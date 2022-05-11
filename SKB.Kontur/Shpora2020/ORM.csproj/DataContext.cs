using NUnit.Framework;
using ORM.Contracts;
using ORM.Db;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ORM
{
    public class DataContext : IDataContext
    {
        private readonly IDbEngine dbEngine;

        private readonly Dictionary<string, DbEntity> addBuffer;
        private readonly Dictionary<string, DbEntity> updBuffer;
        private readonly Dictionary<string, DbEntity> immutableBuffer;
        private int sendCommandsCount = 0;

        public DataContext(IDbEngine dbEngine)
        {
            this.dbEngine = dbEngine;
            addBuffer = new Dictionary<string, DbEntity>();
            updBuffer = new Dictionary<string, DbEntity>();
            immutableBuffer = new Dictionary<string, DbEntity>();
        }

        public Book Find(string id)
        {
            if (updBuffer.ContainsKey(id))
                return updBuffer[id] as Book;

            var getRequest = GetGetRequest(id);
            var response = dbEngine.Execute(getRequest);
            if (response == ";")
                return null;

            var dbEntity = CreateEntityByFields<Book>(response);
            var sameDbEntity = GetSameEntityWithOtherLink(dbEntity);
            updBuffer[id] = dbEntity;
            immutableBuffer[id] = sameDbEntity;
            return dbEntity;
        }

        public Book Read(string id)
        {
            if (updBuffer.ContainsKey(id))
                return updBuffer[id] as Book;

            var getRequest = GetGetRequest(id);
            var response = dbEngine.Execute(getRequest);
            if (response == ";")
                throw new Exception($"Ошибка при чтении из БД\n" +
                                    $"Method={nameof(Read)}\n" +
                                    $"Request={getRequest}\n" +
                                    $"Response={response}");

            var dbEntity = CreateEntityByFields<Book>(response);
            var sameDbEntity = GetSameEntityWithOtherLink(dbEntity);
            updBuffer[id] = dbEntity;
            immutableBuffer[id] = sameDbEntity;
            return dbEntity;
        }

        public void Insert(Book entity)
        {
            if (entity is null)
                throw new Exception($"Попытка вставить несуществующий элемент");

            addBuffer[entity.Id] = entity;
        }

        public void SubmitChanges()
        {
            var requestStr = CreateRequest();
            var response = dbEngine.Execute(requestStr);
            var successRequestsCount = new Regex("ok;").Matches(response).Count;
            if (successRequestsCount != sendCommandsCount)
                throw new Exception($"Ошибка при обращении к БД\n" +
                                    $"Method={nameof(SubmitChanges)}\n" +
                                    $"Request={requestStr}\n" +
                                    $"Response={response}");

            foreach (var dbEntity in addBuffer)
                updBuffer[dbEntity.Key] = dbEntity.Value;

            foreach (var dbEntity in updBuffer)
                immutableBuffer[dbEntity.Key] = GetSameEntityWithOtherLink(dbEntity.Value);

            sendCommandsCount = 0;
            addBuffer.Clear();
        }

        private DbEntity GetSameEntityWithOtherLink(DbEntity entity)
        {
            var type = entity.GetType();
            var defaultEntity = GetDefaultEntity(type);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(entity);
                property.SetValue(defaultEntity, propertyValue);
            }

            return defaultEntity;
        }

        private string CreateRequest()
        {
            var requestBuilder = new StringBuilder(addBuffer.Count + updBuffer.Count);
            foreach (var addItem in addBuffer.Values)
                requestBuilder.Append(GetAddRequest(addItem));
            foreach (var updItem in updBuffer.Values)
                requestBuilder.Append(GetUpdRequest(updItem));
            return requestBuilder.ToString();
        }

        private string GetGetRequest(string id)
        {
            return $"get Id={id};";
        }

        private string GetUpdRequest(DbEntity updItem)
        {
            var itemFromBuffer = immutableBuffer[updItem.Id];
            var serializeItem = SerializeItemForRequest(updItem, itemFromBuffer, out var propertiesCount);
            if (propertiesCount == 1)
                return string.Empty;

            sendCommandsCount++;
            return "upd " + serializeItem;
        }

        private string GetAddRequest(DbEntity addItem)
        {
            var type = addItem.GetType();
            var defaultItem = GetDefaultEntity(type);
            sendCommandsCount++;
            return "add " + SerializeItemForRequest(addItem, defaultItem, out _);
        }

        private string SerializeItemForRequest(DbEntity item, DbEntity compareItem, out int propertiesInRequestCount)
        {
            var itemAddRequestBuilder = new StringBuilder();

            var type = item.GetType();
            var properties = type.GetProperties();
            propertiesInRequestCount = 0;
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(item);
                var defaultValue = property.GetValue(compareItem);
                if (propertyValue != null && !propertyValue.Equals(defaultValue)
                    || property.Name == "Id")
                {
                    itemAddRequestBuilder.Append(SerializeProperty(property, item));
                    propertiesInRequestCount++;
                }
            }

            itemAddRequestBuilder.Remove(itemAddRequestBuilder.Length - 1, 1).Append(";");

            return itemAddRequestBuilder.ToString();
        }

        private string SerializeProperty(PropertyInfo property, DbEntity item)
        {
            const string escapedChars = @";\,=";
            var valueStringBuilder = new StringBuilder();
            var propertyValue = property.GetValue(item).ToString();
            var currentIndex = 0;
            while (currentIndex < propertyValue.Length)
                if (escapedChars.Contains(propertyValue[currentIndex]))
                    valueStringBuilder.Append($"\\{propertyValue[currentIndex++]}");
                else
                    valueStringBuilder.Append(propertyValue[currentIndex++]);

            return $"{property.Name}={valueStringBuilder},";
        }

        private T CreateEntityByFields<T>(string data) where T : DbEntity
        {
            var type = typeof(T);
            var propertiesValueFromDb = ParseDbData(data);
            var properties = type.GetProperties();
            var entity = GetDefaultEntity(type);
            foreach (var propertyNameValuePair in propertiesValueFromDb)
            {
                var property = properties.FirstOrDefault(p => p.Name == propertyNameValuePair.Key);
                SavePropertyValue(property, entity, propertyNameValuePair.Value);
            }

            return (T) entity;
        }

        private void SavePropertyValue<T>(PropertyInfo property, T entity, string value)
        {
            var parseMethod = GetParseMethod(property);
            var objValue = GetParseValue(parseMethod, entity, value);
            property.SetValue(entity, objValue);
        }

        private object GetParseValue<T>(MethodInfo parseMethod, T entity, string value)
        {
            return parseMethod != null
                ? parseMethod.Invoke(entity, new object[] {value, CultureInfo.InvariantCulture})
                : value;
        }

        private MethodInfo GetParseMethod(PropertyInfo property)
        {
            var parseMethod = property.PropertyType
                .GetMethod("Parse", new[] {typeof(string), typeof(CultureInfo)});
            return parseMethod ?? property.PropertyType.GetMethod("Parse", new[] {typeof(string)});
        }

        private DbEntity GetDefaultEntity(Type type)
        {
            var defaultConstructor = type.GetConstructor(new Type[] { });
            return (DbEntity) defaultConstructor.Invoke(null);
        }

        private Dictionary<string, string> ParseDbData(string data)
        {
            var propertiesAndValues = new Dictionary<string, string>();
            var propertiesNamesAndValues = GetProperties(data);
            foreach (var (name, value) in propertiesNamesAndValues)
                propertiesAndValues[name] = value;

            return propertiesAndValues;
        }

        private IEnumerable<(string, string)> GetProperties(string data)
        {
            var nameAndValuePair = new StringBuilder(data.Length);
            var separatorIndex = 0;
            var isSeparatorFound = false;
            var currentIndex = 0;
            while (currentIndex < data.Length - 1)
                if (data[currentIndex].ToString() == @"\")
                {
                    nameAndValuePair.Append(data[++currentIndex]);
                    separatorIndex = isSeparatorFound ? separatorIndex : separatorIndex + 1;
                    ++currentIndex;
                }
                else if (data[currentIndex] != ',')
                {
                    if (data[currentIndex] == '=')
                        isSeparatorFound = true;
                    nameAndValuePair.Append(data[currentIndex++]);
                    separatorIndex = isSeparatorFound ? separatorIndex : separatorIndex + 1;
                }
                else
                {
                    yield return GetNameAndValueFromPair(nameAndValuePair, separatorIndex);

                    separatorIndex = 0;
                    isSeparatorFound = false;
                    currentIndex++;
                    nameAndValuePair.Clear();
                }


            yield return GetNameAndValueFromPair(nameAndValuePair, separatorIndex);
        }

        private (string, string) GetNameAndValueFromPair(StringBuilder nameAndValuePair, int separatorIndex)
        {
            var nameAndValueStr = nameAndValuePair.ToString();
            var name = nameAndValueStr.Substring(0, separatorIndex);
            var value = nameAndValueStr.Substring(separatorIndex + 1,
                nameAndValueStr.Length - separatorIndex - 1);
            return (name, value);
        }
    }
}