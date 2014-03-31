using System.Web.Mvc;
using System.Web.Routing;

namespace ReddStats.Web
{
    using System.Web.Optimization;

    using ReddStats.Web.App_Start;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
