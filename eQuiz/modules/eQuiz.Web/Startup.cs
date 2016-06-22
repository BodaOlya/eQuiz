using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eQuiz.Web.Startup))]
namespace eQuiz.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
