// -----------------------------------------------------------------------
// <copyright file="DelegateFilter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Filters {
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Context;
	using RestModels.Parsers;

	/// <summary>
	///     A dataset filter that uses a delegate to filter
	/// </summary>
	/// <typeparam name="TModel">The type of model being filtered</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public class DelegateFilter<TModel, TUser> : IFilter<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The delegate to use to filter the dataset
		/// </summary>
		private readonly Func<IApiContext<TModel, TUser>, IQueryable<TModel>, Task<IQueryable<TModel>>> FilterDelegate;

		/// <summary>
		///     Initializes a new instance of the <see cref="DelegateFilter{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="filterDelegate">The delegate to use to filter the dataset</param>
		public DelegateFilter(
			Func<IApiContext<TModel, TUser>, IQueryable<TModel>, Task<IQueryable<TModel>>> filterDelegate) =>
			this.FilterDelegate = filterDelegate;

		/// <summary>
		///     Filters the model dataset by some condition
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The current dataset to be filtered</param>
		/// <returns>The filtered dataset</returns>
		public Task<IQueryable<TModel>> FilterDataAsync(
			IApiContext<TModel, TUser> context,
			IQueryable<TModel> dataset) {
			return this.FilterDelegate(context, dataset);
		}
	}
}