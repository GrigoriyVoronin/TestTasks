using System.Collections.Generic;

namespace OrganizationApi.Models.Response
{
    public class BypassSheetInfoResponse
    {
        public BypassSheetInfoResponse(RouteStatus routeStatus, List<BypassSheet> uniqueBypassSheets)
        {
            RouteStatus = routeStatus;
            UniqueBypassSheets = uniqueBypassSheets;
        }

        public RouteStatus RouteStatus { get; }
        public List<BypassSheet> UniqueBypassSheets { get; }
    }
}