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