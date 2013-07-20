using Mono.CSharp;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Manhole {
	public class ManholeRouteHandler : IHttpHandler, IRouteHandler {
		private static Evaluator evaluator;
		private static SingleMessageReportPrinter reportPrinter;
		public Evaluator Evaluator { 
			get {
				if(evaluator == null) {
					reportPrinter = new SingleMessageReportPrinter();
					evaluator = new Evaluator(
						new CompilerContext(
							new CompilerSettings {
								AssemblyReferences = AppDomain.CurrentDomain
									.GetAssemblies()
									.Select(t => t.FullName)
									.ToList()
							}, 
							reportPrinter
						)
					);
					evaluator.Evaluate(@"
						using System; 
						using System.Linq; 
						using System.Text
					");
				}
				return evaluator;
			}
			set {
				evaluator = value;
			}
		}

		class SingleMessageReportPrinter : ReportPrinter {
			public AbstractMessage Message { get; set; }
			public override void Print(AbstractMessage msg, bool showFullPath) {
 				Message = msg;
			}
		}

		public Func<HttpContext, bool> Authorize { get; set; }
		public ManholeRouteHandler() { 
			Authorize = ctx => ctx.Request.IsLocal;
		}

		public bool IsReusable {
			get { return true; }
		}

		public IHttpHandler GetHttpHandler(RequestContext requestContext) {
			return this;
		}

		public void ProcessRequest(HttpContext context) {
			if(!Authorize(context)) {
				Error(context, "403 Forbidden", "Access has been denied for this request");
				return;
			}
			string path = Path.GetFileName(context.Request.QueryString.ToString()).ToLowerInvariant();
			if(path == String.Empty && context.Request.HttpMethod == "POST") {
				Evaluate(context);
			} else if(path == String.Empty && context.Request.HttpMethod == "GET") {
				Resource(context, "index.html");
			} else if((path == "here" || path == "here-nojq") && context.Request.HttpMethod == "GET") {
				if(path == "here") {
					Resource(context, "jquery-1.9.1.min.js");
				}
				Resource(context, "jquery.terminal-0.6.3.min.js");
				Resource(context, "jquery.manhole.js");
				Resource(context, "manhole.bootstrap.js", new NameValueCollection { { "CSS_PATH", context.Request.Url.AbsolutePath+"?jquery.terminal.css" } });
			} else {
				Resource(context, path);
			}
		}

		private static Regex ReplacementPattern = new Regex(@"\$(\w+)\$");
		private void Resource(HttpContext context, string path, NameValueCollection replacements = null) {
			using(var stream = typeof(ManholeRouteHandler).Assembly.GetManifestResourceStream("Manhole.Resources." + path)) {
				if(stream == null) {
					Error(context, "404 Not found", "The requested resource " + path + " could not be found.");
					return;
				}
				context.Response.ContentType = MimeMapping.GetMimeMapping(path);
				if(replacements==null) {
					stream.CopyTo(context.Response.OutputStream);
				} else { 
					using(var reader = new StreamReader(stream)) {
						var content =	ReplacementPattern.Replace(
							reader.ReadToEnd(),
							t => replacements[t.Groups[1].Value] ?? t.Value
						);
						context.Response.Write(content);
					}
				}
			}
		}

		private void Evaluate(HttpContext context) {
			if(context.Request.HttpMethod.ToLowerInvariant() != "post") {
				Error(context, "405 Method not allowed", "Please use POST to submit commands");
			}
			if(context.Request.ContentType != "text/plain") {
				Error(context, "415 Unsupported media type", "Please submit commands as text/plain");
				return;
			}
			using(var input = context.Request.GetBufferedInputStream())
			using(var reader = new StreamReader(input, context.Request.ContentEncoding)) {
				var command = reader.ReadToEnd();
				object result = null; bool result_set;
				try {
					Evaluator.Evaluate(command, out result, out result_set);
					context.Response.ContentType = "application/json";
					context.Response.Write(JsonConvert.SerializeObject(result));
				} catch(Exception e) {
					Error(context, "500 Internal server error", e.Message);
				}
			}
		}

		private void Error(HttpContext context, string status, string message) {
			context.Response.Status = status;
			context.Response.ContentType = "text/plain";
			context.Response.Write(message);
		}
	}
}
