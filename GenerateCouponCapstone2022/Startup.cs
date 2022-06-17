using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GenerateCouponCapstone2022.Startup))]
namespace GenerateCouponCapstone2022
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
