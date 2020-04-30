﻿// -----------------------------------------------------------------------
// <copyright file="DeleteOperation.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Operations {
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;

	using RestModels.Operations;
	using RestModels.Parsers;

	/// <summary>
	///     An operation that will delete a model in an EntityFramework context
	/// </summary>
	/// <typeparam name="TModel">The type of model to delete</typeparam>
	/// <typeparam name="TContext">The type of EntityFramework database context to use</typeparam>
	public class DeleteOperation<TModel, TContext> : IOperation<TModel>
		where TModel : class where TContext : DbContext {
		/// <summary>
		///     Deletes the models specified in the request body
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="dataset">The filtered dataset to operate on</param>
		/// <param name="parsed">The parsed request body, if any</param>
		/// <param name="user">The current user context, if any</param>
		/// <returns>The affected models</returns>
		public async Task<IEnumerable<TModel>> OperateAsync(
			HttpContext context,
			IQueryable<TModel> dataset,
			ParseResult<TModel>[] parsed,
			object user) {
			TContext DatabaseContext = context.RequestServices.GetRequiredService<TContext>();
			DatabaseContext.Set<TModel>().RemoveRange(dataset);
			await DatabaseContext.SaveChangesAsync();
			return dataset;
		}
	}
}