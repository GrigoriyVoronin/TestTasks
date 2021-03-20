namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("gender")]
    public class GenderParameter : StringParameter
    {
        public GenderParameter(string value)
            : base(value)
        {
        }
    }
}