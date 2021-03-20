namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("text")]
    public class TextParameter : StringParameter
    {
        public TextParameter(string value)
            : base(value)
        {
        }
    }
}