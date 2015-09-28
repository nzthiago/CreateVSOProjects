using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ARMAndASMTestWeb.Startup))]
namespace ARMAndASMTestWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
