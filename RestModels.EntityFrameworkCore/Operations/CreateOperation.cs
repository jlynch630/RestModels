// -----------------------------------------------------------------------
// <copyright file="CreateOperation.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Operations {
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;

	using RestModels.Context;
	using RestModels.Operations;

	/// <summary>
	///     An operation that will create a model in an EntityFramework context
	/// </summary>
	/// <typeparam name="TModel">The type of model to create</typeparam>
	/// <typeparam name="TContext">The type of EntityFramework database context to use</typeparam>
	public class CreateOperation<TModel, TContext> : IOperation<TModel>
		where TModel : class where TContext : DbContext {
		/// <summary>
		///     Creates the models specified in the request body
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The filtered dataset to operate on</param>
		/// <returns>The affected models</returns>
		public async Task<IEnumerable<TModel>> OperateAsync(
			IApiContext<TModel, object> context,
			IQueryable<TModel> dataset) {
			TContext DatabaseContext = context.Services.GetRequiredService<TContext>();
			IEnumerable<TModel> Models = context.Parsed.Select(p => p.ParsedModel).ToArray();
			DatabaseContext.Set<TModel>().AddRange(Models); // docs say not to use async method here
			await DatabaseContext.SaveChangesAsync();

			return Models;
		}
	}
}