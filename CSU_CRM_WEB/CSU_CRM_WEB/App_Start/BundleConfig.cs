using System.Web;
using System.Web.Optimization;

namespace CSU_CRM_WEB
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/js/required").Include(
            //            "~/Scripts/jquery-1.10.2.js"));

            bundles.Add(new ScriptBundle("~/bundles/Morris_Charts").Include(
                        "~/Content/boostrap_sb/js/plugins/morris/raphael.min.js",
                        "~/Content/boostrap_sb/js/plugins/morris/morris.min.js",
                        "~/Content/boostrap_sb/js/plugins/morris/morris-data.js",
                        "~/Content/boostrap_sb/js/plugins/flot/jquery.flot.js",
                        "~/Content/boostrap_sb/js/plugins/flot/jquery.flot.tooltip.min.js",
                        "~/Content/boostrap_sb/js/plugins/flot/jquery.flot.resize.js",
                        "~/Content/boostrap_sb/js/plugins/flot/jquery.flot.pie.js",
                        "~/Content/boostrap_sb/js/plugins/flot/flot-data.js"
             ));

           


            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //"~/Content/site.css",
                      "~/Content/boostrap_sb/css/sb-admin.css",
                      "~/Content/boostrap_sb/css/plugins/morris.css"

                      ).Include("~/Content/fontawesome/font-awesome.css", new CssRewriteUrlTransform())
                      .Include("~/Content/fontawesome/font-awesome.min.css", new CssRewriteUrlTransform())
            );

            //var cssBundle = (new StyleBundle("~/bundles/css").Include(
            //    "~/Content/fontawesome/font-awesome.css",
            //    "~/Content/fontawesome/font-awesome.min.css"
            //    )
            //);
            //cssBundle.Transforms.Add(new CssTransformer(new YuiCssMinifier()));

            //bundles.Add(cssBundle);

            //// Code removed for clarity.
            //BundleTable.EnableOptimizations = true;

        }
    }
}
