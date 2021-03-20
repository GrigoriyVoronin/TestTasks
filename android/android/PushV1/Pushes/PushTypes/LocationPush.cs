namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    using Parameters.Implementations;
    #endregion

    [ParseName(nameof(LocationPush))]
    public class LocationPush : Push
    {
        public LocationPush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}