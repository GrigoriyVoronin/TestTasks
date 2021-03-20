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