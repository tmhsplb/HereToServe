﻿using System.Web;
using System.Web.Optimization;

namespace OPIDDaily
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
 
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Note the use of Bundle instead of ScriptBundle when loading bootstrap.
            // This solved an error problem described in
            // https://stackoverflow.com/questions/68009152/mvc-5-scripts-render-bundles-bootstrap-error-object-reference-not-set-to
            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            // jquery mvc5 datatables js files
            bundles.Add(new ScriptBundle("~/bundles/mvc5datatables").Include(
                         "~/Scripts/DataTables/jquery.dataTables.min.js",
                         "~/Scripts/DataTables/dataTables.bootstrap.js",
                         "~/Scripts/moment.js"));

           // jquery mvc5 datatables css file
           bundles.Add(new StyleBundle("~/Content/DataTables/css").Include(
                       "~/Content/DataTables/css/dataTables.bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      //"~/Content/themes/base/jquery-ui.css",
                      //"~/Content/themes/base/jquery.ui.theme.css"
                       "~/Content/themes/base/redmond-jquery-ui.css",
                       "~/Content/themes/base/redmond-jquery-ui.theme.css"
                      ));
        }
    }
}
