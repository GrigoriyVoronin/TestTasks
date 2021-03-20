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