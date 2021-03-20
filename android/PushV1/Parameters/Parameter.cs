using System;

namespace PushV1.Parameters
{
    public abstract class Parameter<T>
    {
        public string Name { get; }
        public Func<string, T> ParseFunc { get; }
        public T Value { get; }
        public string StrValue { get; }

        protected Parameter(string name, Func<string, T> parseFunc, string value)
        {
            Name = name;
            ParseFunc = parseFunc;
            StrValue = value;
            Value = parseFunc(value);
        }
    }
}