using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ACM.Startup))]
namespace ACM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
