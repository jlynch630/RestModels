// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options {
	using System;
	using System.Collections.Generic;

	using Microsoft.AspNetCore.Builder;
	using RestModels.Auth;
	using RestModels.Conditions;
	using RestModels.ExceptionHandlers;
	using RestModels.Filters;
	using RestModels.Models;
	using RestModels.Operations;
	using RestModels.Parsers;
	using RestModels.Results;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The children options being built off of this one
		/// </summary>
		private readonly List<RestModelOptionsBuilder<TModel, TUser>> Children;

		/// <summary>
		///     The options object that's being built
		/// </summary>
		private readonly RestModelOptions<TModel, TUser> Options;

		/// <summary>
		///     Initializes a new instance of the <see cref="RestModelOptionsBuilder{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="existing">Starting options for this builder</param>
		internal RestModelOptionsBuilder(RestModelOptions<TModel, TUser> existing = null) {
			this.Options = existing ?? new RestModelOptions<TModel, TUser>();
			this.Children = new List<RestModelOptionsBuilder<TModel, TUser>>();
		}

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <param name="optionsHandler">The options for this route</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> MapRoute(
			string pattern,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder> routeOptionsHandler) {
			// create a copy of the options and use that for the new route
			RestModelOptions<TModel, TUser> Copy = this.Options.Copy();

			if (pattern.StartsWith("/")) pattern = pattern.Substring(1);
			if (!pattern.EndsWith("/")) pattern += "/";
			Copy.RoutePattern += pattern;

			if (routeOptionsHandler != null) Copy.RouteOptionsHandler = routeOptionsHandler;

			RestModelOptionsBuilder<TModel, TUser> Child = new RestModelOptionsBuilder<TModel, TUser>(Copy);
			optionsHandler(Child);
			this.Children.Add(Child);

			return this;
		}

		/// <summary>
		///		Adds an auth provider to this route
		/// </summary>
		/// <param name="authProvider">The auth provider to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddAuthProvider(IAuthProvider<TModel, TUser> authProvider) {
			return this;
		}

		/// <summary>
		///		Adds a body parser to this route
		/// </summary>
		/// <param name="bodyParser">The body parser to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddBodyParser(IBodyParser<TModel> bodyParser) {
			return this;
		}

		/// <summary>
		///		Adds a condition to this route, which much be met for the request to succeed
		/// </summary>
		/// <param name="condition">The condition to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddCondition(ICondition<TModel, TUser> condition) {
			return this;
		}

		/// <summary>
		///		Adds a filter to this route, which will filter the dataset retrieved
		/// </summary>
		/// <param name="filter">The filter to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddFilter(IFilter<TModel, TUser> filter) {
			return this;
		}

		/// <summary>
		///		Adds an exception handler to this route
		/// </summary>
		/// <param name="exceptionHandler">The exception handler to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddExceptionHandler(IExceptionHandler exceptionHandler) {
			return this;
		}

		/// <summary>
		///		Sets the model provider to use for this route
		/// </summary>
		/// <param name="modelProvider">The model provider to use</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> UseModelProvider(IModelProvider<TModel, TUser> modelProvider) {
			return this;
		}

		/// <summary>
		///		Adds a request method that this route supports
		/// </summary>
		/// <param name="requestMethod">The request method to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddRequestMethod(string requestMethod) {
			return this;
		}

		/// <summary>
		///		Sets the operation to use for this route, like create, update, or delete
		/// </summary>
		/// <param name="operation">The operation to use</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> UseOperation(IOperation<TModel, TUser> operation) {
			return this;
		}

		/// <summary>
		///		Sets the result formatter to use for this route
		/// </summary>
		/// <param name="resultFormatter">The result formatter to use</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> UseResultFormatter(IResultFormatter<TModel, TUser> resultFormatter) {
			return this;
		}

		/// <summary>
		///     Builds these options and all of this builder's child options
		/// </summary>
		/// <returns>A list of all of the options created by this builder, including its own</returns>
		internal IEnumerable<RestModelOptions<TModel, TUser>> BuildAll() {
			List<RestModelOptions<TModel, TUser>> AllOptions =
				new List<RestModelOptions<TModel, TUser>> { this.Options };
			foreach (RestModelOptionsBuilder<TModel, TUser> Builder in this.Children)
				AllOptions.AddRange(Builder.BuildAll());

			return AllOptions;
		}
	}
}