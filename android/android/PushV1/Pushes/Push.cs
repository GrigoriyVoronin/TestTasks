namespace PushV1.Pushes
{
    #region using
    using System.Collections.Generic;
    using System.Reflection;
    using Parameters;
    using Parameters.Abstractions;
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