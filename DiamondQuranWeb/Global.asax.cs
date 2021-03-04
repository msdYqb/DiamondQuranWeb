using DiamondQuranWeb.Models;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DiamondQuranWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ar");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ar");
        }
        protected void Session_Start()
        {
            if (Session["AyahAutoComplete"] == null)
            {
                Session["AyahAutoComplete"] = new ApplicationDbContext().Quran.Select(x => x.NormalCleanest).ToList();
            }
        }
    }
}