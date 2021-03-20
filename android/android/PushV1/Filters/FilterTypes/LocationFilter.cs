namespace PushV1.Filters.FilterTypes
{
    #region using
    using System;
    using System.Collections.Generic;
    using Parameters.Implementations;
    using Pushes;
    using Pushes.PushTypes;
    #endregion

    public class LocationFilter : Filter
    {
        public LocationFilter(SystemData systemData)
            : base(systemData)
        {
            FilteringFunc = IsFilteringByRadius;
            FiltratingPushTypes = new HashSet<Type>
            {
                typeof(LocationPush), typeof(LocationAgePush)
            };
        }

        public override Func<Push, bool> FilteringFunc { get; }

        public override HashSet<Type> FiltratingPushTypes { get; }

        private bool IsFilteringByRadius(Push push)
        {
            var x = push.Parameters.Get<XCoordParameter>();
            var y = push.Parameters.Get<YCoordParameter>();
            var radius = push.Parameters.Get<RadiusParameter>();
            if (x == null || y == null || radius == null)
                return false;

            return Math.Sqrt(Math.Pow(SystemData.XCoord.Value - x.Value, 2) +
                             Math.Pow(SystemData.YCoord.Value - y.Value, 2)) > radius.Value;
        }
    }
}