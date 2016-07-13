using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MutipleOAuth.Startup))]
namespace MutipleOAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
