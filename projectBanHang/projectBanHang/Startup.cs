using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(projectBanHang.Startup))]
namespace projectBanHang
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
