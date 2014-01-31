using System.Web.Mvc;
using SocialGoal.Web.Core.Helpers;

namespace SocialGoal.Web.Core.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static UserHtmlHelper User(this HtmlHelper html)
        {
            return new UserHtmlHelper(html, new UrlHelper(html.ViewContext.RequestContext));
        }
    }
}