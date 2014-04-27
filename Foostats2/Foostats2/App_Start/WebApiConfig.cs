using System.Web.Http;

namespace Foostats2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "FoostatsApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: null
            );
        }
    }
}
