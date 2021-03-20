namespace PushV1.Parameters.Implementations
{
    #region using
    using Abstractions;
    #endregion

    [ParseName("y_coord")]
    public class YCoordParameter : FloatParameter
    {
        public YCoordParameter(string value)
            : base(value)
        {
        }
    }
}