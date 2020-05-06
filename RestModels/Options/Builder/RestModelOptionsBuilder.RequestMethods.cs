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
		public RestModelOptionsBuilder<TModel, TUser> SetupGet(string? routePattern, Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanGet();
			OptionsBuilder.ClearBodyParsers();
			OptionsBuilder.ClearOperation();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupGet(Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			return this.SetupGet("", optionsHandler);
		}

		/// <summary>
		///     Sets up a GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupGet() {
			return this.SetupGet(null);
		}

		/// <summary>
		///     Sets up an anonymous GET request for the given route pattern, clearing auth providers, body parsers, and any operation.
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupAnonymousGet(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			return this.SetupGet(
				routePattern,
				(o) => {
					o.ClearAuthProviders();
					optionsHandler?.Invoke(o);
				});
		}


		/// <summary>
		///     Sets up an anonymous GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupAnonymousGet(Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			return this.SetupAnonymousGet("", optionsHandler);
		}

		/// <summary>
		///     Sets up an anonymous GET request for the same route pattern, clearing body parsers and any operation.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupAnonymousGet() {
			return this.SetupAnonymousGet(null);
		}

		/// <summary>
		///     Sets up a POST request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPost(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPost();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a POST request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPost(Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			return this.SetupPost("", optionsHandler);
		}

		/// <summary>
		///     Sets up a POST request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPost() {
			return this.SetupPost(null);
		}

		/// <summary>
		///     Sets up a PUT request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPut(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPut();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a PUT request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPut(Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			return this.SetupPut("", optionsHandler);
		}

		/// <summary>
		///     Sets up a PUT request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPut() {
			return this.SetupPut(null);
		}

		/// <summary>
		///     Sets up a DELETE request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupDelete(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.ClearBodyParsers();
			OptionsBuilder.CanDelete();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a DELETE request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupDelete(Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			return this.SetupDelete("", optionsHandler);
		}

		/// <summary>
		///     Sets up a DELETE request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupDelete() {
			return this.SetupDelete(null);
		}

		/// <summary>
		///     Sets up a PATCH request for the given route pattern
		/// </summary>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPatch(string routePattern, Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			RestModelOptionsBuilder<TModel, TUser> OptionsBuilder = this.FlatMap(routePattern);
			OptionsBuilder.ClearRequestMethods();
			OptionsBuilder.CanPatch();

			optionsHandler?.Invoke(OptionsBuilder);
			return this;
		}

		/// <summary>
		///     Sets up a PATCH request for the same route pattern.
		/// </summary>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPatch(Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler) {
			return this.SetupPatch("", optionsHandler);
		}

		/// <summary>
		///     Sets up a PATCH request for the same route pattern.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetupPatch() {
			return this.SetupPatch(null);
		}
	}
}