using Microsoft.AspNetCore.Builder;
using RestModels.Middleware;
using RestModels.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection {
	public static class ApplicationBuilderExtensions {
		public static IApplicationBuilder UseRestModels<TModel, TUser>(this IApplicationBuilder app, string route, Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler, Action<IEndpointConventionBuilder> routeOptionsHandler)
			where TModel : class where TUser : class {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = new RestModelOptionsBuilder<TModel, TUser>(route, routeOptionsHandler);
			optionsHandler(OptionsBuilder);
			app.UseRouting().UseEndpoints(
				builder => {
					foreach (RestModelOptions<TModel, TUser> Options in OptionsBuilder.BuildAll()) {
						// only build routes with result writers
						if (Options.ResultWriter == null) continue;

						IEndpointConventionBuilder EndpointBuilder = builder.MapMethods(
							Options.RoutePattern,
							Options.RequestMethods.ToArray(),
							new RestModelMiddleware<TModel, TUser>(Options).TryHandleRequest);
						Options.RouteOptionsHandler?.Invoke(EndpointBuilder);
					}
				});

			return app;
		}
	}
}
