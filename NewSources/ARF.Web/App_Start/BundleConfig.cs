using System.Web.Optimization;

namespace ARF.Web.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Content/js/admin_js").Include(                     
                     "~/Content/js/jquery.validate.js",
                     "~/Content/js/jquery.validate.unobtrusive.js",
                     "~/Content/js/bootstrap.js",
                     "~/Content/js/moment-with-locales.js",
                     "~/Content/js/bootstrap-datetimepicker.js",
                     "~/Content/js/hijri-date.js",
                     "~/Content/js/calendar.js",
                     //"~/Content/js/bootstrap-select.js",
                     "~/Content/js/jquery.unobtrusive-ajax.js",
                     "~/Content/js/summernote.js",
                     "~/Content/js/jquery.bootgrid.custom.js",
                     "~/Content/js/jquery.bootgrid.fa.js",
                     "~/Content/js/sweet-alert.js",
                     //"~/Content/js/rwd-table.js",
                     "~/Content/js/app.js",
                     "~/Content/js/site.js"
                     //"~/Content/js/bootstrap-datepicker.js"
            ));


            bundles.Add(new ScriptBundle("~/Content/js/chat").Include(
                     "~/Content/js/chat.js",
                     "~/Scripts/jquery.signalR-2.2.2.min.js",
                     "~/Content/js/jScroll.js"
                ));


            bundles.Add(new StyleBundle("~/Content/css/admin_css")
                .Include("~/Content/css/bootstrap.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/AdminLTE.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/skins/_all-skins.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/bootstrap-datetimepicker.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/calendar.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/bootstrap-select.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/summernote.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/font-awesome.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/skins/square/blue.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/jquery.bootgrid.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/datatables.css",new CssRewriteUrlTransform())
                .Include("~/Content/css/sweet-alert.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/rwd-table.css",new CssRewriteUrlTransform())
                .Include("~/Content/css/fonts.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/admin-site.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/Content/css/admin_css_rtl")
                .Include("~/Content/css/bootstrap-rtl.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/AdminLTE-rtl.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/skins/_all-skins.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/bootstrap-datetimepicker.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/calendar.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/bootstrap-select-rtl.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/summernote.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/font-awesome.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/skins/square/blue.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/jquery.bootgrid.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/datatables-rtl.css",new CssRewriteUrlTransform())
                .Include("~/Content/css/sweet-alert-rtl.css", new CssRewriteUrlTransform())
                //.Include("~/Content/css/rwd-table-rtl.css",new CssRewriteUrlTransform())
                .Include("~/Content/css/fonts.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/admin-site-rtl.css", new CssRewriteUrlTransform())
            );
            

            bundles.Add(new ScriptBundle("~/Content/js/admin_js_login").Include(                    
                     "~/Content/js/bootstrap.js",
                    "~/Content/js/jquery.validate.js",
                    "~/Content/js/jquery.validate.unobtrusive.js",
                    "~/Content/js/login.js"
          ));

            bundles.Add(new StyleBundle("~/Content/css/admin_css_login")
                .Include("~/Content/css/bootstrap-rtl.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/fonts.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/login.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/Content/css/chat")
                .Include("~/Content/css/chat.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/Content/css/admin_css_login_rtl")
                .Include("~/Content/css/bootstrap-rtl.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/fonts.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/login-rtl.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new ScriptBundle("~/Content/js/temp_links_js").Include(
                     "~/Content/js/jquery.js",
                     "~/Content/js/bootstrap.js"
            ));

            bundles.Add(new StyleBundle("~/Content/css/temp_links_css")
                .Include("~/Content/css/fonts.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/bootstrap.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/Content/css/temp_links_css_rtl")
                .Include("~/Content/css/fonts.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/bootstrap-rtl.css", new CssRewriteUrlTransform())
            );
            //#if !DEBUG
            //            BundleTable.EnableOptimizations = true;
            //#endif

            BundleTable.EnableOptimizations = false;
        }
    }
}