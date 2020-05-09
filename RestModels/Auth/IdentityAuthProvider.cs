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

	using RestModels.Context;
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
		private readonly string? Policy;

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
		public IdentityAuthProvider(string? policy) : this(policy, null) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="IdentityAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="roles">The roles that the <see cref="TUser"/> must be in to authenticate</param>
		public IdentityAuthProvider(IEnumerable<string>? roles) : this(null, roles) {}

		/// <summary>
		///     Initializes a new instance of the <see cref="IdentityAuthProvider{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="policy">The policy that the <see cref="TUser"/> must fulfill to authenticate</param>
		/// <param name="roles">The roles that the <see cref="TUser"/> must be in to authenticate</param>
		public IdentityAuthProvider(string? policy, IEnumerable<string>? roles) {
			this.Policy = policy;
			this.Roles = roles?.ToArray() ?? new string[0];
		}

		/// <summary>
		///     Authenticates the given request context, and returns the authenticated user
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>The currently authenticated user context</returns>
		public async Task<TUser> AuthenticateAsync(IApiContext<TModel, TUser> context) {
			SignInManager<TUser> SignInManager = context.Services.GetRequiredService<SignInManager<TUser>>();
			UserManager<TUser> UserManager = context.Services.GetRequiredService<UserManager<TUser>>();

			if (context.User == null || !SignInManager.IsSignedIn(context.HttpContext.User))
				throw new AuthFailedException("Failed to authenticate user with Identity auth");

			if (this.Policy != null) {
				IAuthorizationService AuthService = context.Services.GetRequiredService<IAuthorizationService>();
				AuthorizationResult Result = await AuthService.AuthorizeAsync(context.HttpContext.User, this.Policy);
				if (!Result.Succeeded)
					throw new AuthFailedException("User does not fulfill required policy for this route");
			}

			if (this.Roles.Any(r => !context.HttpContext.User.IsInRole(r)))
				throw new AuthFailedException("User not in required role for this route");

			return await UserManager.GetUserAsync(context.HttpContext.User);
		}

		/// <summary>
		///     Gets whether or not the given request can be authenticated for
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>
		///     <c>true</c> always
		/// </returns>
		public async Task<bool> CanAuthAsync(IApiContext<TModel, TUser> context) => true;
	}
}