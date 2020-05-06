// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkRestModelsApplicationBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace -- recommended by MS

namespace Microsoft.Extensions.DependencyInjection {
	using System;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.EntityFrameworkCore;
	using RestModels.EntityFramework;
	using RestModels.EntityFramework.Options;
	using RestModels.Options.Builder;

	/// <summary>
	///     Extension methods for the <see cref="IApplicationBuilder" /> interface.
	/// </summary>
	public static class EntityFrameworkRestModelsApplicationBuilderExtensions {
		/// <summary>
		///     Adds rest models middleware with Entity Framework to the app
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="route">The base route for rest models</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <param name="routeOptionsHandler">A handler to set ASP.NET Core options</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkRestModels<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			string route,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder>? routeOptionsHandler)
			where TModel : class where TContext : DbContext where TUser : class {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder =
				new EntityFrameworkRestModelOptionsBuilder<TModel, TUser>(app, typeof(TContext), route, routeOptionsHandler);

			OptionsBuilder.UseModelProvider(new EntityFrameworkModelProvider<TModel, TContext>());
			optionsHandler(OptionsBuilder);

			return app.UseRestModels(OptionsBuilder.BuildAll());
		}

		/// <summary>
		///     Adds rest models middleware with Entity Framework to the app
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="route">The base route for rest models</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkRestModels<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			string route,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TContext : DbContext where TUser : class {
			return app.UseEntityFrameworkRestModels<TModel, TContext, TUser>(route, optionsHandler, null);
		}

		/// <summary>
		///     Adds rest models middleware with Entity Framework to the app at the root ("/") endpoint
		/// </summary>
		/// <typeparam name="TModel">The type of model to use with the API</typeparam>
		/// <typeparam name="TContext">The type of database context to use to access <typeparamref name="TModel"/> entities</typeparam>
		/// <typeparam name="TUser">The user to authenticate with the API</typeparam>
		/// <param name="app">The app to add rest models to</param>
		/// <param name="optionsHandler">A handler to set options for this rest models API</param>
		/// <returns>The same <see cref="IApplicationBuilder" />, for chaining</returns>
		public static IApplicationBuilder UseEntityFrameworkRestModels<TModel, TContext, TUser>(
			this IApplicationBuilder app,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TContext : DbContext where TUser : class {
			return app.UseEntityFrameworkRestModels<TModel, TContext, TUser>("/", optionsHandler, null);
		}
	}
}