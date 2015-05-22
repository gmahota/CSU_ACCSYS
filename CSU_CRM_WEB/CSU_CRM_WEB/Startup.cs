using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CSU_CRM_WEB.Startup))]
namespace CSU_CRM_WEB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
