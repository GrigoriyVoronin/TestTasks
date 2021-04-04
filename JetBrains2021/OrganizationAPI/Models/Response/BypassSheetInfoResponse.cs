using System.Collections.Generic;

namespace OrganizationApi.Models.Response
{
    public class BypassSheetInfoResponse
    {
        public BypassSheetInfoResponse(RouteStatus routeStatus, IReadOnlyList<BypassSheet> uniqueBypassSheets)
        {
            RouteStatus = routeStatus;
            UniqueBypassSheets = uniqueBypassSheets;
        }

        public RouteStatus RouteStatus { get; }
        public IReadOnlyList<BypassSheet> UniqueBypassSheets { get; }
    }
}