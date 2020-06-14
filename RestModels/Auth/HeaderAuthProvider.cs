// -----------------------------------------------------------------------
// <copyright file="HeaderAuthProvider.cs" company="John Lynch">
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
	///     Auth provider that uses header value authentication to authorize the user
	/// </summary>
	/// <typeparam name="TModel">The type of model that this API handles</typeparam>
	/// <typeparam name="TUser">The type of the authenticated user context</typeparam>
	public class HeaderAuthProvider<TModel, TUser> : IAuthProvider<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The delegate to use to get a reference to an authenticated user context.
		/// </summary>
		private readonly Func<string, Task<TUser>> AuthDelegate;

		/// <summary>
		///     The name of the header containing the API key
		/// </summary>
		private readonly string HeaderName;

		/// <summary>
		///     Initializes a new instance of the <see cref="HeaderAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="headerName">The name of the header containing the API key</param>
		/// <param name="authDelegate">
		///     The delegate that, when called with the header value, will return a
		///     reference to an authenticated user context, or throw if the key is invalid
		/// </param>
		public HeaderAuthProvider(string headerName, Func<string, Task<TUser>> authDelegate) {
			this.HeaderName = headerName;
			this.AuthDelegate = authDelegate;
		}

		/// <summary>
		///     Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>The currently authenticated user context</returns>
		public async Task<TUser> AuthenticateAsync(IApiContext<TModel, TUser> context) {
			string HeaderValue = context.Request.Headers[this.HeaderName];
			if (HeaderValue == null)
				throw new AuthFailedException("Failed to authorize user with Header key authentication");

			return await this.AuthDelegate(HeaderValue);
		}

		/// <summary>
		///     Gets whether or not the given request can be authenticated for
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>
		///     <see langword="true"/> if this request contains the header value this <see cref="HeaderAuthProvider{TModel, TUser}"/> authenticates with
		/// </returns>
		public async Task<bool> CanAuthAsync(IApiContext<TModel, TUser> context) =>
			context.Request.Headers.ContainsKey(this.HeaderName);
	}
}