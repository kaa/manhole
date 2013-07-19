using System.Web.Routing;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Web.App_Start.ManholeStart), "Start")]
namespace Web.App_Start {
	public class ManholeStart {
		public static void Start() {
			RouteTable.Routes.Add(new Route("manhole/{*path}",new ManholeModule()));
		}
	}
}