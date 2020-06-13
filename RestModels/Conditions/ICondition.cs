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

	using RestModels.Context;
	using RestModels.Parsers;

	/// <summary>
	///     A condition that ensures a request context meets a specific requirement
	/// </summary>
	/// <typeparam name="TModel">The type of model in the dataset</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public interface ICondition<TModel, in TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Gets a message indicating why the condition might have failed
		/// </summary>
		string? FailureMessage { get; }

		/// <summary>
		///     Verifies that the current request meets a condition
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The current dataset to be filtered</param>
		/// <returns><see langword="true"/> if the request should continue, <see langword="false"/> otherwise</returns>
		Task<bool> VerifyAsync(
			IApiContext<TModel, TUser> context,
			IQueryable<TModel> dataset);
	}
}