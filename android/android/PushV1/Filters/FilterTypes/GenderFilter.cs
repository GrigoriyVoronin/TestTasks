namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class GenderFilter : Filter
    {
        public GenderFilter(SystemData systemData)
            : base(systemData)
        {
            FilteringFunc = IsFilteringByGender;
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(GenderAgePush), typeof(GenderPush)
            };
        }

        public override Func<Push, bool> FilteringFunc { get; }
        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByGender(Push push)
        {
            var gender = push.Parameters.Get<GenderParameter>();
            if (gender == null)
                return false;

            return !gender.Value.Equals(SystemData.Gender.Value);
        }
    }
}