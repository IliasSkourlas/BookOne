using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookOne.Startup))]
namespace BookOne
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
