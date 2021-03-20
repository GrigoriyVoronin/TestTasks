namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("age")]
    public class AgeParameter : IntParameter
    {
        public AgeParameter(string value)
            : base(value)
        {
        }
    }
}