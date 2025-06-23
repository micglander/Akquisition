using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Akquisition.Startup))]
namespace Akquisition
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
