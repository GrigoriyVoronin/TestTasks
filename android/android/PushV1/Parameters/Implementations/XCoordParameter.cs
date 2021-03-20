namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("x_coord")]
    public class XCoordParameter : FloatParameter
    {
        public XCoordParameter(string value)
            : base(value)
        {
        }
    }
}