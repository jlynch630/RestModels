// -----------------------------------------------------------------------
// <copyright file="DelegateAuthProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Auth {
	using System;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	/// <summary>
	///     Auth provider that uses a delegate to retrieve the user context
	/// </summary>
	/// <typeparam name="TModel">The type of model served by the API</typeparam>
	/// <typeparam name="TUser">The authenticated user context type</typeparam>
	public class DelegateAuthProvider<TModel, TUser> : IAuthProvider<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The delegate to use to retrieve a user context
		/// </summary>
		private readonly Func<HttpContext, TModel[], Task<TUser>> AuthDelegate;

		/// <summary>
		///     Initializes a new instance of the <see cref="DelegateAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="authDelegate">The delegate to use to retrieve a user context</param>
		public DelegateAuthProvider(Func<HttpContext, TModel[], Task<TUser>> authDelegate) =>
			this.AuthDelegate = authDelegate;

		/// <summary>
		///     Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="requestContext">The current request context</param>
		/// <param name="parsed">The models parsed from the request body, if any</param>
		/// <returns>The currently authenticated user context</returns>
		public Task<TUser> AuthenticateAsync(HttpContext requestContext, TModel[] parsed) =>
			this.AuthDelegate(requestContext, parsed);
	}
}