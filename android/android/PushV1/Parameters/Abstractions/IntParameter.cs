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