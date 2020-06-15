// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkRestModelOptionsBuilder.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Options {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.Extensions.DependencyInjection;

	using RestModels.EntityFramework.Operations;
	using RestModels.Operations;
	using RestModels.Options;
	using RestModels.Options.Builder;
	using RestModels.OrmBase;
	using RestModels.OrmBase.Options;
	using RestModels.ParameterRetrievers;

	/// <summary>Options builder for an Entity Framework based API</summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public class
		EntityFrameworkRestModelOptionsBuilder<TModel, TUser> : RestModelOptionsBuilder<TModel, TUser>, IOrmRestModelOptionsBuilder<TModel, TUser>
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

		/// <summary>
		///		Gets the properties that make up the primary key of <see cref="TModel"/>
		/// </summary>
		/// <returns>The properties that make up the primary key of <see cref="TModel"/></returns>
		public List<KeyProperty> GetPrimaryKey() {
			using IServiceScope Scope = this.App.ApplicationServices.CreateScope();
			DbContext Context = (DbContext)Scope.ServiceProvider.GetService(this.ContextType);
			IKey Key = Context.Model.FindRuntimeEntityType(typeof(TModel)).FindPrimaryKey();

			return Key.Properties.Select(k => new KeyProperty(k.Name, k.PropertyInfo)).ToList();
		}

		/// <summary>
		///		Gets a create <see cref="IOperation{TModel, TUser}"/> for this model
		/// </summary>
		/// <returns>A new create operation for <see cref="TModel"/></returns>
		public IOperation<TModel, TUser> GetCreateOperation() => this.NewOperation(typeof(CreateOperation<,>));

		/// <summary>
		///		Gets an update <see cref="IOperation{TModel, TUser}"/> for this model
		/// </summary>
		/// <param name="properties">The properties to use to compare existing models with parsed models to determine which models to update</param>
		/// <param name="retrievers">A list of parameter retrievers to use to get values for the properties. If null, the parsed body will be used instead</param>
		/// <returns>A new update operation for <see cref="TModel"/></returns>
		public IOperation<TModel, TUser> GetUpdateOperation(
			PropertyInfo[] properties,
			ParameterRetriever[]? retrievers) {
			return (IOperation<TModel, TUser>)Activator.CreateInstance(
				typeof(UpdateOperation<,>).MakeGenericType(typeof(TModel), this.ContextType),
				new object?[] { properties, retrievers })!;
		}

		/// <summary>
		///		Gets a delete <see cref="IOperation{TModel, TUser}"/> for this model
		/// </summary>
		/// <returns>A new delete operation for <see cref="TModel"/></returns>
		public IOperation<TModel, TUser> GetDeleteOperation() => this.NewOperation(typeof(DeleteOperation<,>));

		/// <summary>
		///     Creates a new operation
		/// </summary>
		/// <param name="operationType">The type of operation to create</param>
		/// <returns>The created operation</returns>
		private IOperation<TModel, TUser> NewOperation(Type operationType) {
			IOperation<TModel, TUser> Operation =
				(IOperation<TModel, TUser>)Activator.CreateInstance(
					operationType.MakeGenericType(typeof(TModel), this.ContextType))!;
			return Operation;
		}
	}
}