namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("radius")]
    public class RadiusParameter : IntParameter
    {
        public RadiusParameter(string value)
            : base(value)
        {
        }
    }
}