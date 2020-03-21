// -----------------------------------------------------------------------
// <copyright file="ICondition.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Conditions {
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	/// <summary>
	///     A condition that ensures a request context meets a specific requirement
	/// </summary>
	/// <typeparam name="TModel">The type of model being filtered</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public interface ICondition<in TModel, in TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Gets a message indicating why the condition might have failed
		/// </summary>
		string FailureMessage { get; }

		/// <summary>
		///     Verifies that the current request meets a condition
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="dataset">The current dataset to be filtered</param>
		/// <param name="parsed">The parsed request body, if any</param>
		/// <param name="user">The current user context, if any</param>
		/// <returns><code>true</code> if the request should continue, <code>false</code> otherwise</returns>
		Task<bool> VerifyAsync(HttpContext context, IQueryable<TModel> dataset, TModel parsed, TUser user);
	}
}