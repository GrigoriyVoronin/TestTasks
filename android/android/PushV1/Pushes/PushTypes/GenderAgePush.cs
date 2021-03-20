namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    using Parameters.Implementations;
    #endregion

    [ParseName(nameof(GenderAgePush))]
    public class GenderAgePush : Push
    {
        public GenderAgePush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}