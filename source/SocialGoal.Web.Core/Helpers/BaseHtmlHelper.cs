using System.Web.Mvc;

namespace SocialGoal.Web.Core.Helpers
{
    public class BaseHtmlHelper
    {
        private readonly HtmlHelper _html;
        private readonly UrlHelper _url;

        public BaseHtmlHelper(HtmlHelper html, UrlHelper url)
        {
            _html = html;
            _url = url;
        }
        protected HtmlHelper Html { get { return _html; } }
        protected UrlHelper Url { get { return _url; } }
    }
}