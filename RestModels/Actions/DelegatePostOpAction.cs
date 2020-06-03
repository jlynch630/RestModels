// -----------------------------------------------------------------------
// <copyright file="DelegatePostOpAction.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Actions {
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using RestModels.Context;

	/// <summary>
	///     Post-operation action that uses a delegate as its action
	/// </summary>
	/// <typeparam name="TModel">The type of model that has been operated on</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public class DelegatePostOpAction<TModel, TUser> : IPostOpAction<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The action to run post-op
		/// </summary>
		private readonly Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, Task> Action;

		/// <summary>
		///     Initializes a new instance of the <see cref="DelegatePostOpAction{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="action">The action to run post-op</param>
		public DelegatePostOpAction(Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, Task> action) =>
			this.Action = action;

		/// <summary>
		///     Runs this post-operation action
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The data that resulted from the operation</param>
		/// <returns>When the action is complete</returns>
		public Task RunAsync(IApiContext<TModel, TUser> context, IEnumerable<TModel> dataset) =>
			this.Action(context, dataset);
	}
}