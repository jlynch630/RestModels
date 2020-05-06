// -----------------------------------------------------------------------
// <copyright file="IdentityAuthProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Auth {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.DependencyInjection;
	using RestModels.Exceptions;
	using RestModels.Parsers;

	/// <summary>
	///     Auth provider that uses ASP.NET Core Identity to authorize the user
	/// </summary>
	/// <typeparam name="TModel">The type of model that this API handles</typeparam>
	/// <typeparam name="TUser">The type of the authenticated user context</typeparam>
	public class IdentityAuthProvider<TModel, TUser> : IAuthProvider<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///		The policy that the <see cref="TUser"/> must fulfill in order for this <see cref="IAuthProvider{TModel, TUser}"/> to succeed
		/// </summary>
		private readonly string Policy;

		/// <summary>
		///		The roles that the <see cref="TUser"/> must be in for this <see cref="IAuthProvider{TModel, TUser}"/> to succeed
		/// </summary>
		private readonly string[] Roles;

		/// <summary>
		///     Initializes a new instance of the <see cref="IdentityAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		public IdentityAuthProvider() : this(null, null) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="IdentityAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="policy">The policy that the <see cref="TUser"/> must fulfill to authenticate</param>
		public IdentityAuthProvider(string policy) : this(policy, null) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="IdentityAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="roles">The roles that the <see cref="TUser"/> must be in to authenticate</param>
		public IdentityAuthProvider(IEnumerable<string> roles) : this(null, roles) {}

		/// <summary>
		///     Initializes a new instance of the <see cref="IdentityAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="policy">The policy that the <see cref="TUser"/> must fulfill to authenticate</param>
		/// <param name="roles">The roles that the <see cref="TUser"/> must be in to authenticate</param>
		public IdentityAuthProvider(string policy, IEnumerable<string> roles) {
			this.Policy = policy;
			this.Roles = roles?.ToArray();
		}

		/// <summary>
		///     Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="parsed">The models parsed from the request body, if any</param>
		/// <returns>The currently authenticated user context</returns>
		public async Task<TUser> AuthenticateAsync(HttpContext context, ParseResult<TModel>[] parsed) {
			SignInManager<TUser> SignInManager = context.RequestServices.GetRequiredService<SignInManager<TUser>>();
			UserManager<TUser> UserManager = context.RequestServices.GetRequiredService<UserManager<TUser>>();

			if (context.User == null || !SignInManager.IsSignedIn(context.User))
				throw new AuthFailedException("Failed to authenticate user with Identity auth");

			if (this.Policy != null) {
				IAuthorizationService AuthService = context.RequestServices.GetRequiredService<IAuthorizationService>();
				AuthorizationResult Result = await AuthService.AuthorizeAsync(context.User, this.Policy);
				if (!Result.Succeeded)
					throw new AuthFailedException("User does not fulfill required policy for this route");
			}

			if (this.Roles != null) {
				if (this.Roles.Any(r => !context.User.IsInRole(r)))
					throw new AuthFailedException("User not in required role for this route");
			}

			return await UserManager.GetUserAsync(context.User);
		}

		/// <summary>
		///     Gets whether or not the given request can be authenticated for
		/// </summary>
		/// <param name="requestContext">The current request context</param>
		/// <param name="parsedModel">The models parsed from the request body, if any</param>
		/// <returns>
		///     <c>true</c> if this request contains the header value this <see cref="HeaderAuthProvider{TModel, TUser}"/> authenticates with
		/// </returns>
		public async Task<bool> CanAuthAsync(HttpRequest requestContext, ParseResult<TModel>[]? parsedModel) => true;
	}
}