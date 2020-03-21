// -----------------------------------------------------------------------
// <copyright file="IAuthProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Auth {
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	/// <summary>
	///		Providers for a RestModel API authentication
	/// </summary>
	/// <typeparam name="TModel">The type of model served by the API</typeparam>
	/// <typeparam name="TUser">The authenticated user context type</typeparam>
	public interface IAuthProvider<in TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///		Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="requestContext">The current request context</param>
		/// <param name="parsed">The models parsed from the request body, if any</param>
		/// <returns>The currently authenticated user context</returns>
		Task<TUser> AuthenticateAsync(HttpContext requestContext, TModel[] parsed);
	}
}