// -----------------------------------------------------------------------
// <copyright file="RestModelsApplicationBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace -- recommended by MS

namespace Microsoft.Extensions.DependencyInjection {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	using Microsoft.AspNetCore.Builder;

	using RestModels.Auth;
	using RestModels.Middleware;
	using RestModels.Options;
	using RestModels.Options.Builder;

	/// <summary>
	///     Extension methods for the <see cref="IApplicationBuilder" /> interface.
	/// </summary>
	public static class RestModelsApplicationBuilderExtensions {

		/// <summary>
		///     Adds rest models middleware to the app
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="route">The base route for rest models</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseRestModels<TModel>(
			this IApplicationBuilder app,
			string route,
			Action<RestModelOptionsBuilder<TModel, NoUser>> optionsHandler)
			where TModel : class =>
			app.UseRestModels<TModel>(route, optionsHandler, null);

		/// <summary>
		///     Adds rest models middleware to the app at the root ("/") endpoint
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseRestModels<TModel>(
			this IApplicationBuilder app,
			Action<RestModelOptionsBuilder<TModel, NoUser>> optionsHandler)
			where TModel : class =>
			app.UseRestModels<TModel>("/", optionsHandler);

		/// <summary>
		///     Adds rest models middleware to the app
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="route">The base route for rest models</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <param name="routeOptionsHandler">A handler to set ASP.NET Core options</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseRestModels<TModel>(
			this IApplicationBuilder app,
			string route,
			Action<RestModelOptionsBuilder<TModel, NoUser>> optionsHandler,
			Action<IEndpointConventionBuilder>? routeOptionsHandler)
			where TModel : class =>
			app.UseRestModels<TModel, NoUser>(route, optionsHandler, routeOptionsHandler);

		/// <summary>
		///     Adds rest models middleware to the app
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="route">The base route for rest models</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseRestModels<TModel, TUser>(
			this IApplicationBuilder app,
			string route,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TUser : class =>
			app.UseRestModels(route, optionsHandler, null);

		/// <summary>
		///     Adds rest models middleware to the app at the root ("/") endpoint
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseRestModels<TModel, TUser>(
			this IApplicationBuilder app,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TUser : class =>
			app.UseRestModels("/", optionsHandler);

		/// <summary>
		///     Adds rest models middleware to the app
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="route">The base route for rest models</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <param name="routeOptionsHandler">A handler to set ASP.NET Core options</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseRestModels<TModel, TUser>(
			this IApplicationBuilder app,
			string route,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder>? routeOptionsHandler)
			where TModel : class where TUser : class {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder =
				new RestModelOptionsBuilder<TModel, TUser>(route, routeOptionsHandler);
			optionsHandler(OptionsBuilder);
			return app.UseRestModels(OptionsBuilder.BuildAll());
		}

		/// <summary>
		///     Adds rest models middleware to the app using already built <see cref="RestModelOptions{TModel, TUser}" />. Not
		///     intended for application use.
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="options">The options for rest models, keyed to their route patterns</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseRestModels<TModel, TUser>(
			this IApplicationBuilder app,
			Dictionary<string, List<RestModelOptions<TModel, TUser>>> options)
			where TModel : class where TUser : class {
			app.UseRouting().UseEndpoints(
				builder => {
					// todo: eventually find a better way to do this, probably drag things out into objects and out of this extension class
					// routes are grouped by their route pattern
					foreach ((string RoutePattern, List<RestModelOptions<TModel, TUser>> AllOptions) in options) {
						// only build routes with result writers and req methods
						// call routes with auth providers first
						RestModelOptions<TModel, TUser>[] Valid = AllOptions
							.Where(o => o.ResultWriter != null && o.RequestMethods != null)
							.OrderByDescending(o => o.AuthProviders?.Count ?? 0).ToArray();

						if (Valid.Length == 0) continue;

						// req methods can't be null b/c of Valid up there ^
						HashSet<string> HttpMethods = Valid.Select(o => o.RequestMethods!)
							.Aggregate((total, next) => total.Concat(next).ToHashSet());
						IEndpointConventionBuilder EndpointBuilder = builder.MapMethods(
							RoutePattern,
							HttpMethods,
							async context => {
								Stopwatch Stopwatch = new Stopwatch();
								Stopwatch.Start();

								RestModelOptions<TModel, TUser>[] RequestOptions = Valid.Where(
									o => o.RequestMethods.Any(
										m => string.Equals(
											context.Request.Method,
											m,
											StringComparison.OrdinalIgnoreCase))).ToArray();

								for (int i = 0; i < RequestOptions.Length; i++) {
									RestModelOptions<TModel, TUser> Options = RequestOptions[i];
									RestModelMiddleware<TModel, TUser> Middleware =
										new RestModelMiddleware<TModel, TUser>(Options);
									bool HasNext = i != RequestOptions.Length - 1;
									if (!await Middleware.TryHandleRequest(context, HasNext))
										break;
								}

								Stopwatch.Stop();
								Debug.WriteLine($"total elapsed: {Stopwatch.ElapsedMilliseconds}");
							});

						foreach (RestModelOptions<TModel, TUser> Options in Valid)
							Options.RouteOptionsHandler?.Invoke(EndpointBuilder);
					}
				});

			return app;
		}
	}
}