using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LuxBio.Website.Startup))]
namespace LuxBio.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
