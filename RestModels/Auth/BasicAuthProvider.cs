// -----------------------------------------------------------------------
// <copyright file="BasicAuthProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Auth {
	using System;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Net.Http.Headers;

	using RestModels.Exceptions;
	using RestModels.Parsers;

	/// <summary>
	///     Auth provider that uses basic authentication to authorize the user
	/// </summary>
	/// <typeparam name="TModel">The type of model that this API handles</typeparam>
	/// <typeparam name="TUser">The type of the authenticated user context</typeparam>
	public class BasicAuthProvider<TModel, TUser> : IAuthProvider<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The delegate to use to get a reference to an authenticated user context.
		/// </summary>
		private readonly Func<string, string, Task<TUser>> AuthDelegate;

		/// <summary>
		///     Initializes a new instance of the <see cref="BasicAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="authDelegate">
		///     The delegate that, when called with the decoded username and password, will return a
		///     reference to an authenticated user context, or throw if the credentials are invalid
		/// </param>
		public BasicAuthProvider(Func<string, string, Task<TUser>> authDelegate) => this.AuthDelegate = authDelegate;

		/// <summary>
		///     Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="parsed">The models parsed from the request body, if any</param>
		/// <returns>The currently authenticated user context</returns>
		public async Task<TUser> AuthenticateAsync(HttpContext context, ParseResult<TModel>[] parsed) {
			string AuthHeader = context.Request.Headers[HeaderNames.Authorization];

			byte[] DecodedCredentialBytes = Convert.FromBase64String(AuthHeader.Substring(6)); // "Basic "
			string DecodedCredentials = Encoding.UTF8.GetString(DecodedCredentialBytes);
			int SeparatorIndex = DecodedCredentials.IndexOf(":", StringComparison.Ordinal);
			if (SeparatorIndex == -1)
				throw new AuthFailedException("Malformed credentials for HTTP Basic Auth");

			return await this.AuthDelegate(
				DecodedCredentials.Substring(0, SeparatorIndex),
				DecodedCredentials.Substring(SeparatorIndex + 1));
		}

		/// <summary>
		///     Gets whether or not the given request can be authenticated for
		/// </summary>
		/// <param name="requestContext">The current request context</param>
		/// <param name="parsedModel">The models parsed from the request body, if any</param>
		/// <returns>
		///     <c>true</c> if this request contains an Authorization header that starts with "Basic", <c>false</c> otherwise.
		/// </returns>
		public async Task<bool> CanAuthAsync(HttpRequest requestContext, ParseResult<TModel>[]? parsedModel) =>
			requestContext.Headers.ContainsKey(HeaderNames.Authorization)
			&& requestContext.Headers[HeaderNames.Authorization].Count == 1
			&& requestContext.Headers[HeaderNames.Authorization][0].StartsWith("Basic ");
	}
}