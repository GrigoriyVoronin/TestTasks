namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("expiry_date")]
    public class ExpiryDateParameter : LongParameter
    {
        public ExpiryDateParameter(string value)
            : base(value)
        {
        }
    }
}