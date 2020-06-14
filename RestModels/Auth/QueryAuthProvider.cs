// -----------------------------------------------------------------------
// <copyright file="QueryAuthProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Auth {
	using System;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Context;
	using RestModels.Exceptions;
	using RestModels.Parsers;

	/// <summary>
	///     Auth provider that uses query parameter authentication to authorize the user
	/// </summary>
	/// <typeparam name="TModel">The type of model that this API handles</typeparam>
	/// <typeparam name="TUser">The type of the authenticated user context</typeparam>
	public class QueryAuthProvider<TModel, TUser> : IAuthProvider<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The delegate to use to get a reference to an authenticated user context.
		/// </summary>
		private readonly Func<string, Task<TUser>> AuthDelegate;

		/// <summary>
		///     The name of the query parameter containing the API key
		/// </summary>
		private readonly string ParameterName;

		/// <summary>
		///     Initializes a new instance of the <see cref="QueryAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="parameterName">The name of the query parameter containing the API key</param>
		/// <param name="authDelegate">
		///     The delegate that, when called with the parameter value, will return a
		///     reference to an authenticated user context, or throw if the key is invalid
		/// </param>
		public QueryAuthProvider(string parameterName, Func<string, Task<TUser>> authDelegate) {
			this.ParameterName = parameterName;
			this.AuthDelegate = authDelegate;
		}

		/// <summary>
		///     Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>The currently authenticated user context</returns>
		public async Task<TUser> AuthenticateAsync(IApiContext<TModel, TUser> context) {
			string QueryValue = context.Request.Query[this.ParameterName];
			if (QueryValue == null)
				throw new AuthFailedException("Failed to authorize user with query parameter authentication");

			return await this.AuthDelegate(QueryValue);
		}

		/// <summary>
		///     Gets whether or not the given request can be authenticated for
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>
		///     <see langword="true"/> if this request contains the query parameter this
		///     <see cref="IAuthProvider{TModel, TUser}" /> authenticates with, <see langword="false"/> otherwise.
		/// </returns>
		public async Task<bool> CanAuthAsync(IApiContext<TModel, TUser> context) =>
			context.Request.Query.ContainsKey(this.ParameterName);
	}
}