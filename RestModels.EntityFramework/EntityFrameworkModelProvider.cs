// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkModelProvider.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework {
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using RestModels.Context;
	using RestModels.Models;
	using RestModels.Parsers;

	/// <summary>
	///     Model provider that uses Entity Framework as a backend
	/// </summary>
	/// <typeparam name="TModel">The type of model whose dataset to provide</typeparam>
	/// <typeparam name="TContext">The type of the database context to use</typeparam>
	public class EntityFrameworkModelProvider<TModel, TContext> : IModelProvider<TModel>
		where TModel : class where TContext : DbContext {
		/// <summary>
		///     Gets an entity framework query pointing to all of the models available for the current request context
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <returns>An <see cref="IQueryable{T}" /> of all of the models available</returns>
		public async Task<IQueryable<TModel>> GetModelsAsync(IApiContext<TModel, object> context) {
			TContext DatabaseContext = context.Services.GetRequiredService<TContext>();
			return DatabaseContext.Set<TModel>();
		}
	}
}