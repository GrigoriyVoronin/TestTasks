namespace PushV1
{
    #region using
    using System.Collections.Generic;
    using Parameters;
    using Parameters.Abstractions;
    using Parameters.Implementations;
    #endregion

    public class SystemData
    {
        private SystemData(ParametersCollection parameters)
        {
            Parameters = parameters;
        }

        public TimeParameter Time => Parameters.Get<TimeParameter>();
        public GenderParameter Gender => Parameters.Get<GenderParameter>();
        public AgeParameter Age => Parameters.Get<AgeParameter>();
        public OsVersionParameter OsVersion => Parameters.Get<OsVersionParameter>();
        public XCoordParameter XCoord => Parameters.Get<XCoordParameter>();
        public YCoordParameter YCoord => Parameters.Get<YCoordParameter>();

        public ParametersCollection Parameters { get; }

        public static SystemData Parse(IEnumerable<string> input)
        {
            return new SystemData(ParametersCollection.ParseInput(input));
        }
    }
}