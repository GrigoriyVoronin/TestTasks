namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class TimeFilter : Filter
    {
        public TimeFilter(SystemData systemData)
            : base(systemData)
        {
            FilteringFunc = IsFilteringByTime;
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(LocationPush), typeof(AgeSpecificPush)
            };
        }

        public override Func<Push, bool> FilteringFunc { get; }
        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByTime(Push push)
        {
            var expiryDate = push.Parameters.Get<ExpiryDateParameter>();
            if (expiryDate == null)
                return false;

            return expiryDate.Value < SystemData.Time.Value;
        }
    }
}