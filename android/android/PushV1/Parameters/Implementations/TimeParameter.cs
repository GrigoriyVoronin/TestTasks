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