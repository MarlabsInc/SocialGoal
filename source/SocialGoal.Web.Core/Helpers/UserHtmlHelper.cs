using System.Web.Mvc;

namespace SocialGoal.Web.Core.Helpers
{
    public class UserHtmlHelper: BaseHtmlHelper
    {
        public UserHtmlHelper(HtmlHelper html, UrlHelper url) : base(html,url)
        {
        }

        public MvcHtmlString Avatar(string profilePicUrl, object htmlAttributes=null)
        {
            var src = string.IsNullOrEmpty(profilePicUrl) ? 
                "../../Content/templatemo_329_blue_urban/images/facebook-avatar.png"
                : Url.Content(profilePicUrl);
            var tag = new TagBuilder("img");
            tag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), true);
            tag.AddCssClass("thumbnail");
            tag.Attributes.Add("src", src);
            return new MvcHtmlString(tag.ToString());
        }
    }
}