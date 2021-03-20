namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("os_version")]
    public class OsVersionParameter : IntParameter
    {
        public OsVersionParameter(string value)
            : base(value)
        {
        }
    }
}