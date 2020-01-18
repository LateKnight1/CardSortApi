using CardSortApi.App_Start;
using CardSortApi.Extensions;
using CardSortApi.Services.Interfaces;
using CardSortApi.Services.MessageHandlers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CardSortApi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			var cors = new EnableCorsAttribute("*", "*", "*");
			config.EnableCors(cors);
			StructuremapWebApi.Start();

			var currentHttpContext = config.DependencyResolver.GetService<ICurrentHttpContext>();
			var authService = config.DependencyResolver.GetService<IAuthService>();

			config.MessageHandlers.Add(new PreflightRequestsHandler());
			config.MessageHandlers.Add(new JwtAuthHandler(currentHttpContext, authService));
			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				"DefaultApi",
				"api/{controller}/{action}/{id}",
				new { id = RouteParameter.Optional }
			);
		}
	}
}
