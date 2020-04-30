// -----------------------------------------------------------------------
// <copyright file="DelegateCondition.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Conditions {
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Parsers;

	/// <summary>
	///     Condition that uses a delegate to check if it has been met
	/// </summary>
	/// <typeparam name="TModel">The type of model in the dataset</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public class DelegateCondition<TModel, TUser> : ICondition<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The delegate to use to check the status of the condition
		/// </summary>
		private readonly Func<HttpContext, IQueryable<TModel>, ParseResult<TModel>[], TUser, Task<bool>> ConditionDelegate;

		/// <summary>
		///     Initializes a new instance of the <see cref="DelegateCondition{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="conditionDelegate">The delegate to use to check the status of the condition</param>
		public DelegateCondition(Func<HttpContext, IQueryable<TModel>, ParseResult<TModel>[], TUser, Task<bool>> conditionDelegate) =>
			this.ConditionDelegate = conditionDelegate;

		/// <summary>
		///     Gets a message indicating why the condition might have failed
		/// </summary>
		public string FailureMessage { get; } = "A required condition was not met";

		/// <summary>
		///     Verifies that the current request meets a condition
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="dataset">The current dataset to be filtered</param>
		/// <param name="parsed">The parsed request body, if any</param>
		/// <param name="user">The current user context, if any</param>
		/// <returns><code>true</code> if the request should continue, <code>false</code> otherwise</returns>
		public Task<bool> VerifyAsync(HttpContext context, IQueryable<TModel> dataset, ParseResult<TModel>[] parsed, TUser user) =>
			this.ConditionDelegate(context, dataset, parsed, user);
	}
}