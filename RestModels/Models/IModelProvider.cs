// -----------------------------------------------------------------------
// <copyright file="IModelProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Models {
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Parsers;

	/// <summary>
	///     Provider for API models
	/// </summary>
	/// <typeparam name="TModel">The type of model being queried for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user</typeparam>
	public interface IModelProvider<TModel, in TUser> where TModel : class where TUser : class {
		/// <summary>
		///     Gets a query pointing to all of the models available for the current request context
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="parsed">The parsed request body, if any</param>
		/// <param name="user">The current user context, if any</param>
		/// <returns>An <see cref="IQueryable{T}" /> of all of the models available</returns>
		Task<IQueryable<TModel>> GetModelsAsync(HttpContext context, ParseResult<TModel>[] parsed, TUser user);
	}

	/// <summary>
	///     Provider for API models
	/// </summary>
	/// <typeparam name="TModel">The type of model being queried for</typeparam>
	public interface IModelProvider<TModel> : IModelProvider<TModel, object> where TModel : class { }
}