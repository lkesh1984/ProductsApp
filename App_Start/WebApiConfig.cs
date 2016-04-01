using ProductsApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace ProductsApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

			config.Services.Replace(typeof(IHttpControllerTypeResolver), new CustomHttpControllerTypeResolver());
			config.Services.Replace(typeof(IHttpControllerSelector), new ByPassCacheSelector(config));
        }
    }
}
