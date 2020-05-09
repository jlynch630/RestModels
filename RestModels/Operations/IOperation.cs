// -----------------------------------------------------------------------
// <copyright file="IOperation.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Operations {
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Context;
	using RestModels.Parsers;

	/// <summary>
	///     Operation on model datasets, like create, update, or delete.
	/// </summary>
	/// <typeparam name="TModel">The type of model being operated on</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public interface IOperation<TModel, in TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///    Performs some action on a model dataset and returns the models that were affected
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The filtered dataset to operate on</param>
		/// <returns>The affected models</returns>
		Task<IEnumerable<TModel>> OperateAsync(
			IApiContext<TModel, TUser> context,
			IQueryable<TModel> dataset);
	}

	/// <summary>
	///     Operation on model datasets, like create, update, or delete.
	/// </summary>
	/// <typeparam name="TModel">The type of model being operated on</typeparam>
	public interface IOperation<TModel> : IOperation<TModel, object>
		where TModel : class { }
}