using System.Web;
using System.Web.Optimization;

namespace SocialGoal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/Scripts/js").Include(
                "~/Scripts/jquery-1.10.2.min.js",
                "~/Scripts/jquery-ui-1.8.21.custom.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/jquery.pjax.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js",
                "~/Scripts/jquery.autocomplete.js",
                "~/Scripts/jquery-ui-1.8.11.min.js",
                "~/Scripts/jqDnR.js",
                "~/Scripts/jqModal.js",
                "~/Scripts/jquery.jqplot.min.js",
                "~/Scripts/jqplot.barRenderer.min.js",
                "~/Scripts/jqplot.dateAxisRenderer.min.js",
                "~/Scripts/jqplot.categoryAxisRenderer.min.js",
                "~/Scripts/jqplot.pointLabels.min.js",
                "~/Scripts/jqplot.canvasTextRenderer.min.js",
                "~/Scripts/jqplot.canvasAxisTickRenderer.min.js",
                "~/Scripts/jqplot.canvasAxisTickRenderer.min.js",
                "~/Scripts/jqplot.ohlcRenderer.min.js",
                "~/Scripts/jqplot.highlighter.min.js",
                "~/Scripts/handlebars-1.0.rc.1.js",
                "~/Scripts/jquery.nicescroll.min.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/HomePage.css",
                "~/Content/jquery-ui-1.8.21.custom.css",
                "~/Content/jqModal.css",
                "~/Content/jquery.jqplot.min.css",
                "~/Content/PagedList.css"
                ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/CustomStyles.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}
