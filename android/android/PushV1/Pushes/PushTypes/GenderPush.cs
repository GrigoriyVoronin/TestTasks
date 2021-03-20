namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    using Parameters.Implementations;
    #endregion

    [ParseName(nameof(GenderPush))]
    public class GenderPush : Push
    {
        public GenderPush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}