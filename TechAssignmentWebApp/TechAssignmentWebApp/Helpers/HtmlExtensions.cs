using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace TechAssignmentWebApp.Helpers
{
    public static class HtmlExtensions
    {
        public static string BuildBreadcrumbNavigation(this HtmlHelper helper)
        {
            var area = helper.ViewContext.RouteData.DataTokens["area"].ToString();
            var controller = helper.ViewContext.RouteData.Values["controller"].ToString();
            var action = helper.ViewContext.RouteData.Values["action"].ToString();

            var breadcrumb = new StringBuilder("<ol class='hbreadcrumb breadcrumb'><li>").Append(helper.ActionLink(area.Titleize(), "Index", "Home").ToHtmlString()).Append("</li>");

            if (controller == "Dashboard")
            {
                return breadcrumb.Append("</ol>").ToString();
            }

            breadcrumb.Append("<li>");
            breadcrumb.Append("<span>" + helper.ActionLink(controller.Titleize(), "Index", controller) + "</span>");
            breadcrumb.Append("</li>");

            if (action == "Index") return breadcrumb.Append("</ol>").ToString();

            breadcrumb.Append("<li  class='active'>");
            breadcrumb.Append("<span>" + action.Titleize() + "</span>");
            breadcrumb.Append("</li>");

            return breadcrumb.Append("</ol>").ToString();
        }
    }
}