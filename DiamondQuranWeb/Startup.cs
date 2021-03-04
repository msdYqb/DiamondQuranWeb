using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DiamondQuranWeb.Startup))]
namespace DiamondQuranWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
