namespace PushV1.Pushes.PushTypes
{
    #region using
    using Parameters;
    using Parameters.Implementations;
    #endregion

    [ParseName(nameof(TechPush))]
    public class TechPush : Push
    {
        public TechPush(ParametersCollection parameters)
            : base(parameters)
        {
        }
    }
}