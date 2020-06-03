// -----------------------------------------------------------------------
// <copyright file="IPostOpAction.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Actions {
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using RestModels.Context;

	/// <summary>
	///     An action that is run after the operation occurs
	/// </summary>
	/// <typeparam name="TModel">The type of model that has been operated on</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public interface IPostOpAction<TModel, in TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Runs this post-operation action
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The data that resulted from the operation</param>
		/// <returns>When the action is complete</returns>
		Task RunAsync(IApiContext<TModel, TUser> context, IEnumerable<TModel> dataset);
	}
}