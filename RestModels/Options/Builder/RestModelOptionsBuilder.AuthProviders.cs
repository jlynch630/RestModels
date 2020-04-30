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
		///     Adds an auth provider to this route that authenticates with a query parameter
		/// </summary>
		/// <param name="parameterName">The name of the parameter to authenticate with</param>
		/// <param name="handler">The handler which, when given the parameter value, will return a user context or throw if the value is invalid</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthQueryAsync(string parameterName, Func<string, Task<TUser>> handler) {
			this.AddAuthProvider(new QueryAuthProvider<TModel, TUser>(parameterName, handler));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a query parameter
		/// </summary>
		/// <param name="parameterName">The name of the parameter to authenticate with</param>
		/// <param name="handler">The handler which, when given the parameter value, will return a user context or throw if the value is invalid</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthQuery(string parameterName, Func<string, TUser> handler) {
			this.AuthQueryAsync(parameterName, async s => handler(s));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a header value
		/// </summary>
		/// <param name="headerName">The name of the header to authenticate with</param>
		/// <param name="handler">The handler which, when given the header value, will return a user context or throw if the value is invalid</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthHeaderAsync(string headerName, Func<string, Task<TUser>> handler) {
			this.AddAuthProvider(new HeaderAuthProvider<TModel, TUser>(headerName, handler));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a header value
		/// </summary>
		/// <param name="headerName">The name of the header to authenticate with</param>
		/// <param name="handler">The handler which, when given the header value, will return a user context or throw if the value is invalid</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthHeader(string headerName, Func<string, TUser> handler) {
			this.AuthHeaderAsync(headerName, async s => handler(s));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with basic auth
		/// </summary>
		/// <param name="handler">The handler which, when given the username and password, will return a user context or throw if the value is invalid</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthBasicAsync(Func<string, string, Task<TUser>> handler) {
			this.AddAuthProvider(new BasicAuthProvider<TModel, TUser>(handler));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with basic auth
		/// </summary>
		/// <param name="handler">The handler which, when given the username and password, will return a user context or throw if the value is invalid</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthBasic(Func<string, string, TUser> handler) {
			this.AuthBasicAsync(async (u, p) => handler(u, p));
			return this;
		}
	}
}