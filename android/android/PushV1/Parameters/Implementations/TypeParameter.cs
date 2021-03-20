namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("type")]
    public class TypeParameter : StringParameter
    {
        public TypeParameter(string value)
            : base(value)
        {
        }
    }
}