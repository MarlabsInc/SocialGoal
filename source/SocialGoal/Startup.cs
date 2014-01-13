using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SocialGoal.Startup))]
namespace SocialGoal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
