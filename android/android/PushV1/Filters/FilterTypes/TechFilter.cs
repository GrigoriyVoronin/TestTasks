namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class TechFilter : Filter
    {
        public TechFilter(SystemData systemData)
            : base(systemData)
        {
            FilteringFunc = IsFilteringByOsVersion;
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(TechPush)
            };
        }

        public override Func<Push, bool> FilteringFunc { get; }
        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByOsVersion(Push push)
        {
            var osVersion = push.Parameters.Get<OsVersionParameter>();
            if (osVersion == null)
                return false;

            return SystemData.OsVersion.Value > osVersion.Value;
        }
    }
}