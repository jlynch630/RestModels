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
	using RestModels.Context;
	using RestModels.Parsers;

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
		private readonly Func<IApiContext<TModel, TUser>, Task<TUser>> AuthDelegate;

		/// <summary>
		///     Initializes a new instance of the <see cref="DelegateAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="authDelegate">The delegate to use to retrieve a user context</param>
		public DelegateAuthProvider(Func<IApiContext<TModel, TUser>, Task<TUser>> authDelegate) =>
			this.AuthDelegate = authDelegate;

		/// <summary>
		///     Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>The currently authenticated user context</returns>
		public Task<TUser> AuthenticateAsync(IApiContext<TModel, TUser> context) =>
			this.AuthDelegate(context);

		/// <summary>
		///     Gets whether or not the given request can be authenticated for
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns><see langword="true"/></returns>
		public async Task<bool> CanAuthAsync(IApiContext<TModel, TUser> context) => true;
	}
}