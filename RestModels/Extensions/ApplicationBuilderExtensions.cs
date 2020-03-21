using Microsoft.AspNetCore.Builder;
using RestModels.Middleware;
using RestModels.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection {
	public static class ApplicationBuilderExtensions {
		public static IApplicationBuilder UseRestModels<TModel, TUser>(this IApplicationBuilder app, Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TUser : class {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = new RestModelOptionsBuilder<TModel, TUser>();
			optionsHandler(OptionsBuilder);
			app.UseRouting().UseEndpoints(
				builder => {
					foreach (RestModelOptions<TModel, TUser> Options in OptionsBuilder.BuildAll()) {
						IEndpointConventionBuilder EndpointBuilder = builder.MapMethods(
							Options.RoutePattern,
							new[] { "GET" },
							new RestModelMiddleware<TModel, TUser>(Options).TryHandleRequest);
						Options.RouteOptionsHandler?.Invoke(EndpointBuilder);
					}
				});

			return app;
		}
	}
}
