using System.Web;
using System.Web.Optimization;

namespace eQuiz.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/libs/jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/libs/jquery/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/libs/jquery/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/libs/jquery/bootstrap.js",
                      "~/Scripts/libs/jquery/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Styles/bootstrap.css",
                      "~/Styles/equiz-temp.css"));
        }
    }
}
