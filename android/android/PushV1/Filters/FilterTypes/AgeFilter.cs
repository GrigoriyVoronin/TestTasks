namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class AgeFilter : Filter
    {
        public AgeFilter(SystemData systemData)
            : base(systemData)
        {
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(AgeSpecificPush), typeof(LocationAgePush),
                typeof(GenderAgePush)
            };
            FilteringFunc = IsFilteringByAge;
        }

        public override Func<Push, bool> FilteringFunc { get; }
        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByAge(Push push)
        {
            var age = push.Parameters.Get<AgeParameter>();
            if (age == null)
                return false;

            return age.Value > SystemData.Age.Value;
        }
    }
}