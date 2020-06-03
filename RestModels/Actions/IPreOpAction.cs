// -----------------------------------------------------------------------
// <copyright file="IPreOpAction.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Actions {
	using System.Linq;
	using System.Threading.Tasks;

	using RestModels.Context;

	/// <summary>
	///     An action that is run before the operation occurs
	/// </summary>
	/// <typeparam name="TModel">The type of model that will be operated on</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public interface IPreOpAction<TModel, in TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Runs this pre-operation action
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The dataset that the operation will be performed on</param>
		/// <returns>When the action is complete</returns>
		Task RunAsync(IApiContext<TModel, TUser> context, IQueryable<TModel> dataset);
	}
}