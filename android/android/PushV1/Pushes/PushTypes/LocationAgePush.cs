namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    using Parameters.Implementations;
    #endregion

    [ParseName(nameof(LocationAgePush))]
    public class LocationAgePush : Push
    {
        public LocationAgePush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}