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