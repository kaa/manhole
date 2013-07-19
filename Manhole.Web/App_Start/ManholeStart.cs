using System.Web.Routing;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Manhole.Web.App_Start.ManholeStart), "Start")]
namespace Manhole.Web.App_Start {
	public class ManholeStart {
		public static void Start() {
			RouteTable.Routes.Add(new Route("manhole", new Manhole.ManholeRouteHandler { 

				// Add authorization here, by default only local requests are served
				// 
				// WARNING! The user of the console is allowed to perform any action that the account
				// running the application pool has access to, this includes creating and modifying files,
				// accessing databases, and other local and remote resources. Use with care!
		
				Authorize = httpContext => httpContext.Request.IsLocal

			}));
		}
	}
}