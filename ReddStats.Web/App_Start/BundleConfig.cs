namespace ReddStats.Web.App_Start
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862

        public static void RegisterBundles(BundleCollection bundles)
        {
            //CSS Bundle
            //========================================================
            var cssBundle = new StyleBundle("~/content/css/cssbundle")
                .IncludeDirectory("~/Content/css/", "*.css", true);
            bundles.Add(cssBundle);

            //JAVASCRIPT Bundle
            //========================================================
            var jsBundle =
                new ScriptBundle("~/bundles/app").IncludeDirectory("~/angular/lib/", "*.js", false)
                    .IncludeDirectory("~/angular/lib/angular/", "*.js", true)
                    .IncludeDirectory("~/angular/", "*.js", false)
                    .IncludeDirectory("~/angular/controllers/", "*.js", true)
                    .IncludeDirectory("~/angular/directives/", "*.js", true)
                    .IncludeDirectory("~/angular/factorys/", "*.js", true)
                    .IncludeDirectory("~/angular/filters/", "*.js", true)
                    .IncludeDirectory("~/angular/modules/", "*.js", true)
                    .IncludeDirectory("~/angular/providers/", "*.js", true)
                    .IncludeDirectory("~/angular/services/", "*.js", true);
            bundles.Add(jsBundle);
        }
    }
}
