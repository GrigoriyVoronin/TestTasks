namespace PushV1.Filters
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pushes;
    #endregion

    public abstract class Filter
    {
        protected Filter(SystemData systemData)
        {
            SystemData = systemData;
        }

        protected SystemData SystemData { get; }

        public abstract Func<Push, bool> FilteringFunc { get; }
        public abstract HashSet<Type> FiltratingPushTypes { get; }

        public static IEnumerable<Push> FiltratePushes(IEnumerable<Push> pushesToFiltrate, ICollection<Filter> filters)
        {
            foreach (var push in pushesToFiltrate)
            {
                var isFiltrate = filters
                    .Where(f => f.FiltratingPushTypes
                        .Contains(push.GetType()))
                    .Aggregate(false, (current, filter) => current | filter.FilteringFunc(push));

                if (!isFiltrate)
                    yield return push;
            }
        }
    }
}