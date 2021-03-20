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