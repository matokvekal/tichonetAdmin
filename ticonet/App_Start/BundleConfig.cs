using System.Web.Optimization;

namespace IdentitySample
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/mainScripts").Include(
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/jquery-ui.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/modernizr-2.6.2.js"));



            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/map").Include(
                        "~/Scripts/Map/Map.js",
                         "~/Scripts/Map/Map.Table.js",
                         "~/Scripts/Map/Map.Stations.js",
                         "~/Scripts/Map/Map.Lines.js",
                         "~/Scripts/Map/Map.UI.js",
                        "~/Scripts/jqGrid/jquery.jqGrid.min.js",
                        "~/Scripts/jqGrid/plugins/grid.addons.js",
                        "~/Scripts/jqGrid/plugins/grid.subgrid.js",
                        "~/Scripts/jqGrid/plugins/jquery.searchFilter.js",
                        "~/Scripts/jqGrid/i18n/grid.locale-en.js",
                        "~/Scripts/spectrum.js"));

            bundles.Add(new ScriptBundle("~/bundles/insets").Include(
                        "~/Scripts/Insets/Insets.js",
                         "~/Scripts/Insets/Insets.Table.js",
                         "~/Scripts/Insets/Insets.Buses.js",
                        "~/Scripts/jqGrid/jquery.jqGrid.min.js",
                        "~/Scripts/jqGrid/plugins/grid.addons.js",
                        "~/Scripts/jqGrid/plugins/grid.subgrid.js",
                        "~/Scripts/jqGrid/plugins/jquery.searchFilter.js",
                        "~/Scripts/jqGrid/i18n/grid.locale-en.js",
                        "~/Scripts/spectrum.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/Site.css",
                      "~/Content/Map.css",
                      "~/Content/spectrum.css",
                      "~/Content/ui.jqgrid.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/jquery-ui.structure.min.css",
                      "~/Content/jquery-ui.theme.min.css",
                      "~/Scripts/jqGrid/plugins/searchFilter.css"));
        }
    }
}
