using System;
using System.Linq;
using System.Web.Mvc;

namespace TechAssignmentWebApp.Helpers
{
    public static class HtmlUtility
    {
        public static string IsActive(this HtmlHelper html, string control, string action, string area)
        {

            var routeData = html.ViewContext.RouteData;
            var areaValue = routeData.DataTokens.Values.Where(x => String.Equals(x.ToString(), area, StringComparison.CurrentCultureIgnoreCase) ).ToList();
            var areaLocation = string.Empty;
            if (areaValue.Any())
            {
                areaLocation = areaValue.FirstOrDefault().ToString().ToLower();
            }
            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            var returnActive = area.ToLower() == areaLocation && control == routeControl && action == routeAction;
            return returnActive ? "active current" : "";
        }

        public static string IsActive(this HtmlHelper html, string area)
        {
            var routeData = html.ViewContext.RouteData;
            var areaValue = routeData.DataTokens.Values.Where(x => String.Equals(x.ToString(), area, StringComparison.CurrentCultureIgnoreCase)).ToList();
            var areaLocation = string.Empty;
            if (areaValue.Any())
            {
                areaLocation = areaValue.FirstOrDefault().ToString().ToLower();
            }

            var returnActive = area.ToLower() == areaLocation;
            return returnActive ? "active" : "";
        }
    }
}