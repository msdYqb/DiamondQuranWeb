using System.Web.Optimization;

namespace DiamondQuranWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/keyboard.js",
                        "~/Scripts/datatables.min.js"));            

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr", "https://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js"));
            //.Include("~/Scripts/modernizr-*")

            bundles.Add(new ScriptBundle("~/bundles/popper", "https://cdnjs.cloudflare.com/ajax/libs/popper.js/2.6.0/umd/popper.min.js"));
            //.Include("~/Scripts/popper.min.js")

            bundles.Add(new ScriptBundle("~/bundles/bootstrap", "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.5.0/js/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/shared")
                .Include("~/Scripts/shared.js" ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/datatables.min.css",
                      "~/Content/all.css",
                      "~/Content/keyboard.css",
                      "~/Content/site.css"));

        }
    }
}
