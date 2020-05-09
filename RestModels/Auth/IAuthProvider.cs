// -----------------------------------------------------------------------
// <copyright file="IAuthProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Auth {
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Context;
	using RestModels.Parsers;

	/// <summary>
	///     Providers for a RestModel API authentication
	/// </summary>
	/// <typeparam name="TModel">The type of model served by the API</typeparam>
	/// <typeparam name="TUser">The authenticated user context type</typeparam>
	public interface IAuthProvider<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>The currently authenticated user context</returns>
		Task<TUser> AuthenticateAsync(IApiContext<TModel, TUser> context);

		/// <summary>
		///     Gets whether or not the given request can be authenticated for
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>
		///     <c>true</c> if this request contains the necessary attributes to be authenticated by this
		///     <see cref="IAuthProvider{TModel, TUser}" />, <c>false</c> otherwise.
		/// </returns>
		Task<bool> CanAuthAsync(IApiContext<TModel, TUser> context);
	}
}