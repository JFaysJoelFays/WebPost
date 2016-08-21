using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebPost.Startup))]
namespace WebPost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
