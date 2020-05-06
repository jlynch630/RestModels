// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkRestModelOptionsBuilder.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Options {
	using System;
	using Microsoft.AspNetCore.Builder;

	using RestModels.Options;
	using RestModels.Options.Builder;

	/// <summary>Options builder for an Entity Framework based API</summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class
		EntityFrameworkRestModelOptionsBuilder<TModel, TUser> : RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///		Initializes a new instance of the <see cref="EntityFrameworkRestModelOptionsBuilder{TModel,TUser}"/> class.
		/// </summary>
		/// <param name="app">The application builder for the app</param>
		/// <param name="contextType">The type of the database context to get the set of <see cref="TModel"/> objects from</param>
		/// <param name="baseRoute">The base route for these options</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		internal EntityFrameworkRestModelOptionsBuilder(IApplicationBuilder app, Type contextType, string baseRoute, Action<IEndpointConventionBuilder>? routeOptionsHandler) : base(baseRoute, routeOptionsHandler) {
			this.ContextType = contextType;
			this.App = app;
		}

		/// <summary>
		///		Initializes a new instance of the <see cref="EntityFrameworkRestModelOptionsBuilder{TModel,TUser}"/> class.
		/// </summary>
		/// <param name="app">The application builder for the app</param>
		/// <param name="contextType">The type of the database context to get the set of <see cref="TModel"/> objects from</param>
		/// <param name="options">The existing options</param>
		internal EntityFrameworkRestModelOptionsBuilder(IApplicationBuilder app, Type contextType, RestModelOptions<TModel, TUser>? options) : base(options) {
			this.ContextType = contextType;
			this.App = app;
		}

		/// <summary>
		///     Gets the type of the database context to get the set of <see cref="TModel"/> objects from
		/// </summary>
		internal Type ContextType { get; }

		/// <summary>
		///		Gets the application builder for this application
		/// </summary>
		internal IApplicationBuilder App { get; }

		/// <summary>
		///		Creates a child instance of the <see cref="RestModelOptionsBuilder{TModel, TUser}"/> type sharing the given base options. When overriden in a derived class, this method can be used to ensure that the entire tree of <see cref="RestModelOptionsBuilder{TModel, TUser}"/> objects share the same derived type
		/// </summary>
		/// <param name="baseOptions">The base options for the new instance</param>
		/// <returns>The new <see cref="RestModelOptionsBuilder{TModel, TUser}"/> instance</returns>
		public override RestModelOptionsBuilder<TModel, TUser> CreateChild(RestModelOptions<TModel, TUser>? baseOptions) {
			return new EntityFrameworkRestModelOptionsBuilder<TModel, TUser>(this.App, this.ContextType, baseOptions);
		}
	}
}