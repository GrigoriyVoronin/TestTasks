namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class AgeFilter : Filter
    {
        public AgeFilter(SystemData systemData)
            : base(systemData)
        {
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(AgeSpecificPush), typeof(LocationAgePush),
                typeof(GenderAgePush)
            };
            FilteringFunc = IsFilteringByAge;
        }

        public override Func<Push, bool> FilteringFunc { get; }
        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByAge(Push push)
        {
            var age = push.Parameters.Get<AgeParameter>();
            if (age == null)
                return false;

            return age.Value > SystemData.Age.Value;
        }
    }
}

namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class GenderFilter : Filter
    {
        public GenderFilter(SystemData systemData)
            : base(systemData)
        {
            FilteringFunc = IsFilteringByGender;
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(GenderAgePush), typeof(GenderPush)
            };
        }

        public override Func<Push, bool> FilteringFunc { get; }
        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByGender(Push push)
        {
            var gender = push.Parameters.Get<GenderParameter>();
            if (gender == null)
                return false;

            return !gender.Value.Equals(SystemData.Gender.Value);
        }
    }
}

namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class LocationFilter : Filter
    {
        public LocationFilter(SystemData systemData)
            : base(systemData)
        {
            FilteringFunc = IsFilteringByRadius;
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(LocationPush), typeof(LocationAgePush)
            };
        }

        public override Func<Push, bool> FilteringFunc { get; }

        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByRadius(Push push)
        {
            var x = push.Parameters.Get<XCoordParameter>();
            var y = push.Parameters.Get<YCoordParameter>();
            var radius = push.Parameters.Get<RadiusParameter>();
            if (x == null || y == null || radius == null)
                return false;

            return Math.Sqrt(Math.Pow(SystemData.XCoord.Value - x.Value, 2) +
                             Math.Pow(SystemData.YCoord.Value - y.Value, 2)) > radius.Value;
        }
    }
}

namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class TechFilter : Filter
    {
        public TechFilter(SystemData systemData)
            : base(systemData)
        {
            FilteringFunc = IsFilteringByOsVersion;
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(TechPush)
            };
        }

        public override Func<Push, bool> FilteringFunc { get; }
        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByOsVersion(Push push)
        {
            var osVersion = push.Parameters.Get<OsVersionParameter>();
            if (osVersion == null)
                return false;

            return SystemData.OsVersion.Value > osVersion.Value;
        }
    }
}

namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class TimeFilter : Filter
    {
        public TimeFilter(SystemData systemData)
            : base(systemData)
        {
            FilteringFunc = IsFilteringByTime;
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(LocationPush), typeof(AgeSpecificPush)
            };
        }

        public override Func<Push, bool> FilteringFunc { get; }
        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByTime(Push push)
        {
            var expiryDate = push.Parameters.Get<ExpiryDateParameter>();
            if (expiryDate == null)
                return false;

            return expiryDate.Value < SystemData.Time.Value;
        }
    }
}

namespace PushV1.Filters
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pushes;
    #endregion

    public abstract class Filter
    {
        protected Filter(SystemData systemData)
        {
            SystemData = systemData;
        }

        protected SystemData SystemData { get; }

        public abstract Func<Push, bool> FilteringFunc { get; }
        public abstract HashSet<Type> FiltratingPushTypes { get; }

        public static IEnumerable<Push> FiltratePushes(IEnumerable<Push> pushesToFiltrate, ICollection<Filter> filters)
        {
            foreach (var push in pushesToFiltrate)
            {
                var isFiltrate = filters
                    .Where(f => f.FiltratingPushTypes
                        .Contains(push.GetType()))
                    .Aggregate(false, (current, filter) => current | filter.FilteringFunc(push));

                if (!isFiltrate)
                    yield return push;
            }
        }
    }
}

namespace PushV1.Parameters.Abstractions
{
    #region using
    using System.Globalization;
    #endregion

    public abstract class FloatParameter : Parameter<float>
    {
        static FloatParameter()
        {
            ParseValueFunc = x => float.Parse(x, CultureInfo.InvariantCulture);
        }

        protected FloatParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Abstractions
{
    public abstract class IntParameter : Parameter<int>
    {
        static IntParameter()
        {
            ParseValueFunc = int.Parse;
        }

        public IntParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Abstractions
{
    public abstract class LongParameter : Parameter<long>
    {
        static LongParameter()
        {
            ParseValueFunc = long.Parse;
        }

        public LongParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Abstractions
{
    #region using
    using System;
    #endregion

    public abstract class Parameter<T> : ParameterBase
    {
        protected Parameter(string value)
            : base(value)
        {
            Value = ParseValueFunc(value);
        }

        public T Value { get; }
        public static Func<string, T> ParseValueFunc { get; protected set; }
    }
}

namespace PushV1.Parameters.Abstractions
{
    public abstract class ParameterBase
    {
        protected ParameterBase(string value)
        {
            StrValue = value;
        }

        public string StrValue { get; }
    }
}

namespace PushV1.Parameters.Abstractions
{
    public abstract class StringParameter : Parameter<string>
    {
        static StringParameter()
        {
            ParseValueFunc = x => x;
        }

        protected StringParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("age")]
    public class AgeParameter : IntParameter
    {
        public AgeParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("expiry_date")]
    public class ExpiryDateParameter : LongParameter
    {
        public ExpiryDateParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("gender")]
    public class GenderParameter : StringParameter
    {
        public GenderParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("os_version")]
    public class OsVersionParameter : IntParameter
    {
        public OsVersionParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("radius")]
    public class RadiusParameter : IntParameter
    {
        public RadiusParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("text")]
    public class TextParameter : StringParameter
    {
        public TextParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("time")]
    public class TimeParameter : LongParameter
    {
        public TimeParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("type")]
    public class TypeParameter : StringParameter
    {
        public TypeParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("x_coord")]
    public class XCoordParameter : FloatParameter
    {
        public XCoordParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("y_coord")]
    public class YCoordParameter : FloatParameter
    {
        public YCoordParameter(string value)
            : base(value)
        {
        }
    }
}

namespace PushV1.Parameters
{
    #region using
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Abstractions;
    #endregion

    public class ParametersCollection : IEnumerable<ParameterBase>
    {
        public static readonly Dictionary<string, ConstructorInfo> ParametersParser =
            Utils.GetChildrenWithConstructors(typeof(ParameterBase), typeof(string));

        private ParametersCollection(Dictionary<string, ParameterBase> parameters)
        {
            Parameters = parameters;
        }

        public Dictionary<string, ParameterBase> Parameters { get; }

        public IEnumerator<ParameterBase> GetEnumerator()
        {
            return Parameters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T Get<T>()
            where T : ParameterBase
        {
            var name = typeof(T).GetParseName();
            if (name == null || !Parameters.ContainsKey(name))
                return null;

            return (T) Parameters[name];
        }

        public static ParametersCollection ParseInput(IEnumerable<string> input)
        {
            var parameters = new Dictionary<string, ParameterBase>();

            foreach (var nameValue in input
                .Select(str => str.Split()))
            {
                parameters[nameValue[0]] =
                    (ParameterBase) ParametersParser[nameValue[0]].Invoke(new object[] {nameValue[1]});
            }

            return new ParametersCollection(parameters);
        }
    }
}

namespace PushV1.Pushes
{
    #region using
    using System.Collections.Generic;
    using System.Reflection;
    using Parameters;
    using Parameters.Implementations;
    #endregion

    public abstract class Push
    {
        public static readonly Dictionary<string, ConstructorInfo> TypedPushConstructors =
            Utils.GetChildrenWithConstructors(typeof(Push), typeof(ParametersCollection));

        protected Push(ParametersCollection parameters)
        {
            Parameters = parameters;
        }

        public TextParameter Text => Parameters.Get<TextParameter>();
        public TypeParameter Type => Parameters.Get<TypeParameter>();
        public ParametersCollection Parameters { get; }

        public static T Parse<T>(IEnumerable<string> input)
            where T : Push
        {
            var parameters = ParametersCollection.ParseInput(input);
            var pushType = parameters.Get<TypeParameter>().Value;
            return (dynamic) TypedPushConstructors[pushType].Invoke(new object?[] {parameters});
        }
    }
}

namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    #endregion

    [ParseName(nameof(AgeSpecificPush))]
    public class AgeSpecificPush : Push
    {
        public AgeSpecificPush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}

namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    #endregion

    [ParseName(nameof(GenderAgePush))]
    public class GenderAgePush : Push
    {
        public GenderAgePush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}

namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    #endregion

    [ParseName(nameof(GenderPush))]
    public class GenderPush : Push
    {
        public GenderPush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}

namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    #endregion

    [ParseName(nameof(LocationAgePush))]
    public class LocationAgePush : Push
    {
        public LocationAgePush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}

namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    #endregion

    [ParseName(nameof(LocationPush))]
    public class LocationPush : Push
    {
        public LocationPush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}

namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    #endregion

    [ParseName(nameof(TechPush))]
    public class TechPush : Push
    {
        public TechPush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}

namespace PushV1
{
    #region using
    using System;
    #endregion

    [AttributeUsage(AttributeTargets.Class)]
    public class ParseNameAttribute : Attribute
    {
        public ParseNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}

namespace PushV1
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Filters;
    using Filters.FilterTypes;
    using Pushes;
    #endregion

    public static class Program
    {
        public static void Main()
        {
            var systemData = ParseSystemData();
            var pushes = ParsePushes();
            var filters = InitFilters(systemData);
            var filteringPushes = Filter
                .FiltratePushes(pushes, filters)
                .ToList();
            PrintPushes(filteringPushes);
        }

        private static void PrintPushes(ICollection<Push> filteringPushes)
        {
            if (filteringPushes.Count == 0)
                Console.WriteLine("-1");
            else
                foreach (var push in filteringPushes)
                    Console.WriteLine(push.Text.Value);
        }

        private static List<Filter> InitFilters(SystemData systemData)
        {
            return new List<Filter>
            {
                new AgeFilter(systemData),
                new GenderFilter(systemData),
                new LocationFilter(systemData),
                new TechFilter(systemData),
                new TimeFilter(systemData)
            };
        }

        private static IEnumerable<Push> ParsePushes()
        {
            var pushCount = int.Parse(Console.ReadLine());
            for (var i = 0; i < pushCount; i++)
                yield return ParsePush();
        }

        private static Push ParsePush()
        {
            var inputLength = int.Parse(Console.ReadLine());
            var input = GetInputLines(inputLength);
            return Push.Parse<Push>(input);
        }

        private static SystemData ParseSystemData()
        {
            var input = GetInputLines(6);
            return SystemData.Parse(input);
        }

        private static IEnumerable<string> GetInputLines(int count)
        {
            for (var i = 0; i < count; i++)
                yield return Console.ReadLine();
        }
    }
}

namespace PushV1
{
    #region using
    using System.Collections.Generic;
    using Parameters;
    using Parameters.Implementations;
    #endregion

    public class SystemData
    {
        private SystemData(ParametersCollection parameters)
        {
            Parameters = parameters;
        }

        public TimeParameter Time => Parameters.Get<TimeParameter>();
        public GenderParameter Gender => Parameters.Get<GenderParameter>();
        public AgeParameter Age => Parameters.Get<AgeParameter>();
        public OsVersionParameter OsVersion => Parameters.Get<OsVersionParameter>();
        public XCoordParameter XCoord => Parameters.Get<XCoordParameter>();
        public YCoordParameter YCoord => Parameters.Get<YCoordParameter>();

        public ParametersCollection Parameters { get; }

        public static SystemData Parse(IEnumerable<string> input)
        {
            return new SystemData(ParametersCollection.ParseInput(input));
        }
    }
}

namespace PushV1
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    #endregion

    public static class Utils
    {
        public static string GetParseName(this Type type)
        {
            var attribute = (ParseNameAttribute) type.GetCustomAttribute(typeof(ParseNameAttribute));
            return attribute?.Name;
        }

        public static Dictionary<string, ConstructorInfo> GetChildrenWithConstructors(Type parentType,
            params Type[] constructorsParameters)
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => IsSearchingType(x, parentType, constructorsParameters))
                .ToDictionary(x => x.GetParseName(),
                    x => x.GetConstructor(constructorsParameters));
        }

        private static bool IsSearchingType(Type currentType, Type parentType, Type[] constructorsParameters)
        {
            if (currentType == null || currentType.IsAbstract ||
                currentType.GetConstructor(constructorsParameters) == null)
                return false;

            return currentType.IsSubclassOf(parentType);
        }
    }
}