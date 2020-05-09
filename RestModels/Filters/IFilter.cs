// -----------------------------------------------------------------------
// <copyright file="IFilter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Filters {
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Context;
	using RestModels.Parsers;

	/// <summary>
	///     Filter for model datasets
	/// </summary>
	/// <typeparam name="TModel">The type of model being filtered</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public interface IFilter<TModel, in TUser> where TModel : class where TUser : class {
		/// <summary>
		///     Filters the model dataset by some condition
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The current dataset to be filtered</param>
		/// <returns>The filtered dataset</returns>
		Task<IQueryable<TModel>> FilterDataAsync(
			IApiContext<TModel, TUser> context,
			IQueryable<TModel> dataset);
	}

	/// <summary>
	///     Filter for model datasets
	/// </summary>
	/// <typeparam name="TModel">The type of model being filtered</typeparam>
	public interface IFilter<TModel> : IFilter<TModel, object> where TModel : class { }
}