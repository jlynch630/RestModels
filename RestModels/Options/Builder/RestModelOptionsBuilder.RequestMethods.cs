// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.Template.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Auth;
	using RestModels.Conditions;
	using RestModels.ExceptionHandlers;
	using RestModels.Exceptions;
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
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Indicates that this route accepts GET requests
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CanGet() {
			return this.AddRequestMethod("GET");
		}

		/// <summary>
		///     Indicates that this route accepts POST requests
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CanPost() {
			return this.AddRequestMethod("POST");
		}

		/// <summary>
		///     Indicates that this route accepts PUT requests
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CanPut() {
			return this.AddRequestMethod("PUT");
		}

		/// <summary>
		///     Indicates that this route accepts DELETE requests
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CanDelete() {
			return this.AddRequestMethod("DELETE");
		}

		/// <summary>
		///     Indicates that this route accepts PATCH requests
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CanPatch() {
			return this.AddRequestMethod("DELETE");
		}

		/// <summary>
		///     Indicates that this route accepts the given request methods
		/// </summary>
		/// <param name="methods">The request methods to accept</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> UseRequestMethods(params string[] methods) {
			this.ClearRequestMethods();
			foreach (string Method in methods) this.AddRequestMethod(Method);
			return this;
		}


		/// <summary>
		///     Sets up a GET request for the given route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Get(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanGet();
			OptionsBuilder.ClearBodyParsers();
			OptionsBuilder.ClearOperation();

			optionsHandler(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Get(Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			return this.Get("", optionsHandler);
		}

		/// <summary>
		///     Sets up a GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Get() {
			return this.Get(null);
		}

		/// <summary>
		///     Sets up an anonymous GET request for the given route pattern, clearing auth providers, body parsers, and any operation.
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AnonymousGet(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			return this.Get(
				routePattern,
				(o) => {
					o.ClearAuthProviders();
					optionsHandler(o);
				});
		}


		/// <summary>
		///     Sets up an anonymous GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AnonymousGet(Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			return this.AnonymousGet("", optionsHandler);
		}

		/// <summary>
		///     Sets up an anonymous GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AnonymousGet() {
			return this.AnonymousGet(null);
		}

		/// <summary>
		///     Sets up a POST request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Post(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPost();
			
			optionsHandler(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a POST request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Post(Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			return this.Post("", optionsHandler);
		}

		/// <summary>
		///     Sets up a POST request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Post() {
			return this.Post(null);
		}

		/// <summary>
		///     Sets up a PUT request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Put(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPut();

			optionsHandler(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a PUT request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Put(Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			return this.Put("", optionsHandler);
		}

		/// <summary>
		///     Sets up a PUT request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Put() {
			return this.Put(null);
		}

		/// <summary>
		///     Sets up a DELETE request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Delete(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanDelete();

			optionsHandler(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a DELETE request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Delete(Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			return this.Delete("", optionsHandler);
		}

		/// <summary>
		///     Sets up a DELETE request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Delete() {
			return this.Delete(null);
		}

		/// <summary>
		///     Sets up a PATCH request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Patch(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPatch();

			optionsHandler(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a PATCH request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Patch(Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			return this.Patch("", optionsHandler);
		}

		/// <summary>
		///     Sets up a PATCH request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Patch() {
			return this.Patch(null);
		}
	}
}