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
                        "~/Scripts/datepicker-he.js",
                        "~/Scripts/jquery-ui-timepicker-addon.min.js",
                        "~/Scripts/jquery-ui-timepicker-he.js",
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
                        "~/Scripts/jqGrid/i18n/grid.locale-he.js",
                        "~/Scripts/spectrum.js"));

            bundles.Add(new ScriptBundle("~/bundles/stable").Include(
                          "~/Scripts/jqGrid/jquery.jqGrid.min.js",
                          "~/Scripts/jqGrid/plugins/grid.addons.js",
                          "~/Scripts/jqGrid/plugins/grid.subgrid.js",
                          "~/Scripts/jqGrid/plugins/jquery.searchFilter.js",
                          "~/Scripts/jqGrid/i18n/grid.locale-en.js",
                          "~/Scripts/StudentsGrid/sTable.js",
                          "~/Scripts/chosen.jquery.min.js",
                          "~/Scripts/jquery.fileDownload.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqGridExtend").Include(
                        "~/Scripts/jqGrid/jquery.jqGrid.min.js",
                        "~/Scripts/jqGrid/plugins/grid.addons.js",
                        "~/Scripts/jqGrid/plugins/grid.subgrid.js",
                        "~/Scripts/jqGrid/plugins/jquery.searchFilter.js",
                        "~/Scripts/jqGrid/i18n/grid.locale-he.js",
                        "~/Scripts/spectrum.js",
                        "~/Scripts/jqGridExtend/jqGridExtend.js",
                        "~/Scripts/jqGridHelper.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/JquerySimpleUI").Include(
                    "~/Scripts/JquerySimpleUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/AppConfigControl").Include(
                    "~/Scripts/AppConfigControl.js"));


            bundles.Add(new ScriptBundle("~/bundles/AngularApp").Include(
                //AngularJS
                    "~/Scripts/angular.js",
                    "~/Scripts/angular-route.js",
                    "~/Scripts/angular-animate.js",
                //Libraries
                    "~/AngularApp/Libraries/TSNetLike/Collections/ArrayExtensions.js",
                    "~/AngularApp/Libraries/TSNetLike/Collections/Dictionary.js",
                    "~/AngularApp/Libraries/TSNetLike/Functors/Functors.js",
                    "~/AngularApp/AppGlobal.js",
                //Models
                    "~/AngularApp/SharedViewModels.js",
                //Controllers 
                    "~/AngularApp/Controller.js",
                    "~/AngularApp/Controllers/SHARED.js",
                    "~/AngularApp/Controllers/NotificationController.js",
                    "~/AngularApp/Controllers/FiltersController.js",
                    "~/AngularApp/Controllers/TemplatesController.js",
                    "~/AngularApp/Controllers/SendMessagesController.js",
                    "~/AngularApp/Controllers/MessageModulePageController.js",

                //Configs
                //Services
                //App
                    "~/AngularApp/App.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/JquerySimpleUIcss").Include(
                    "~/Content/JquerySimpleUI.css"));

            bundles.Add(new StyleBundle("~/bundles/MessagesModule").Include(
                    "~/Content/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                    "~/Content/bootstrap.min.css",
                    "~/Content/chosen.min.css",
                    "~/Content/Site.css",
                    "~/Content/Map.css",
                    "~/Content/stdentstable.css",
                    "~/Content/spectrum.css",
                    "~/Content/ui.jqgrid.css",
                    "~/Content/jquery-ui.min.css",
                    "~/Content/jquery-ui.structure.min.css",
                    "~/Content/jquery-ui.theme.min.css",
                    "~/Content/jquery-ui-timepicker-addon.min.css",
                    "~/Scripts/jqGrid/plugins/searchFilter.css"));

            bundles.Add(new StyleBundle("~/bundles/Home").Include(
                    "~/Content/Home.css"));

            bundles.Add(new StyleBundle("~/bundles/cssReport").Include(
                    "~/Content/Report.css"));
        }
    }
}
