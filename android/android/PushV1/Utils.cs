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