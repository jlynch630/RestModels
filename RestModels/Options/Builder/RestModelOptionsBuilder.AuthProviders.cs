// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.AuthProviders.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System;
	using System.Threading.Tasks;

	using RestModels.Auth;
	using RestModels.Context;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds an auth provider to this route that authenticates with ASP.NET Core's Identity system
		/// </summary>
		/// <param name="policy">The policy that the user must fulfill for auth to succeed</param>
		/// <param name="roles">The roles that the user must be in for auth to succeed</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthAspNetIdentity(string? policy, params string[] roles) {
			this.AddAuthProvider(new IdentityAuthProvider<TModel, TUser>(policy, roles));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with ASP.NET Core's Identity system
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthAspNetIdentity() {
			this.AuthAspNetIdentity(null);
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with ASP.NET Core's Identity system, requiring that the user
		///     fulfill certain roles for auth to succeed
		/// </summary>
		/// <param name="roles">The roles that the user must be in for auth to succeed</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthAspNetIdentityRole(params string[] roles) {
			this.AuthAspNetIdentity(null, roles);
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with basic auth
		/// </summary>
		/// <param name="handler">
		///     The handler which, when given the username and password, will return a user context or throw if
		///     the value is invalid
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthBasic(Func<string, string, TUser> handler) {
			this.AuthBasicAsync(async (u, p) => handler(u, p));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with basic auth
		/// </summary>
		/// <param name="handler">
		///     The handler which, when given the username and password, will return a user context or throw if
		///     the value is invalid
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthBasicAsync(Func<string, string, Task<TUser>> handler) {
			this.AddAuthProvider(new BasicAuthProvider<TModel, TUser>(handler));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a header value
		/// </summary>
		/// <param name="headerName">The name of the header to authenticate with</param>
		/// <param name="handler">
		///     The handler which, when given the header value, will return a user context or throw if the value
		///     is invalid
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthHeader(string headerName, Func<string, TUser> handler) {
			this.AuthHeaderAsync(headerName, async s => handler(s));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a header value
		/// </summary>
		/// <param name="headerName">The name of the header to authenticate with</param>
		/// <param name="handler">
		///     The handler which, when given the header value, will return a user context or throw if the value
		///     is invalid
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthHeaderAsync(
			string headerName,
			Func<string, Task<TUser>> handler) {
			this.AddAuthProvider(new HeaderAuthProvider<TModel, TUser>(headerName, handler));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a query parameter
		/// </summary>
		/// <param name="parameterName">The name of the parameter to authenticate with</param>
		/// <param name="handler">
		///     The handler which, when given the parameter value, will return a user context or throw if the
		///     value is invalid
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthQuery(string parameterName, Func<string, TUser> handler) {
			this.AuthQueryAsync(parameterName, async s => handler(s));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a query parameter
		/// </summary>
		/// <param name="parameterName">The name of the parameter to authenticate with</param>
		/// <param name="handler">
		///     The handler which, when given the parameter value, will return a user context or throw if the
		///     value is invalid
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthQueryAsync(
			string parameterName,
			Func<string, Task<TUser>> handler) {
			this.AddAuthProvider(new QueryAuthProvider<TModel, TUser>(parameterName, handler));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates using a delegate
		/// </summary>=
		/// <param name="handler">
		///     The handler which, when given the API request context, will return a user context or throw if authentication fails
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AuthAsync(
			Func<IApiContext<TModel, TUser>, Task<TUser>> handler) {
			this.AddAuthProvider(new DelegateAuthProvider<TModel, TUser>(handler));
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates using a delegate
		/// </summary>=
		/// <param name="handler">
		///     The handler which, when given the API request context, will return a user context or throw if authentication fails
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Auth(
			Func<IApiContext<TModel, TUser>, TUser> handler) {
			this.AuthAsync(async c => handler(c));
			return this;
		}
	}
}