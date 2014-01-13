
using System.Web;
using System.Web.Security;

namespace SocialGoal.Web.Core.Authentication
{
    public interface IFormsAuthentication
    {       
        void Signout();       
        void SetAuthCookie(HttpContextBase httpContext, FormsAuthenticationTicket authenticationTicket);        
        void SetAuthCookie(HttpContext httpContext, FormsAuthenticationTicket authenticationTicket);
        FormsAuthenticationTicket Decrypt(string encryptedTicket);
    }
}