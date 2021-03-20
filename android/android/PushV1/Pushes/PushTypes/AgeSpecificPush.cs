namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    using Parameters.Implementations;
    #endregion

    [ParseName(nameof(AgeSpecificPush))]
    public class AgeSpecificPush : Push
    {
        public AgeSpecificPush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}