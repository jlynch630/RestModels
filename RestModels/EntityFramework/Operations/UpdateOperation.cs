// -----------------------------------------------------------------------
// <copyright file="UpdateOperation.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Operations {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.Extensions.DependencyInjection;

	using RestModels.Operations;
	using RestModels.Parsers;

	/// <summary>
	///     An operation that will update models in an EntityFramework context
	/// </summary>
	/// <typeparam name="TModel">The type of model to update</typeparam>
	/// <typeparam name="TContext">The type of EntityFramework database context to use</typeparam>
	public class UpdateOperation<TModel, TContext> : IOperation<TModel>
		where TModel : class where TContext : DbContext {
		/// <summary>
		///     True to only update the values given in the request body, false to update all values
		/// </summary>
		private readonly bool PartialUpdate;

		/// <summary>
		///     The primary key of the model
		/// </summary>
		private IKey PrimaryKey;

		/// <summary>
		///		A delegate that will retrieve the primary key(s) for a given model
		/// </summary>
		private Func<HttpContext, ParseResult<TModel>, object[]> PrimaryKeyDelegate;

		/// <summary>
		///     Initializes a new instance of the <see cref="UpdateOperation{TModel,TContext}" /> class.
		/// </summary>
		/// <param name="partialUpdate">True to only update the values given in the request body, false to update all values</param>
		/// <param name="primaryKeyDelegate">A delegate that will retrieve the primary key(s) for a given model</param>
		public UpdateOperation(bool partialUpdate, Func<HttpContext, ParseResult<TModel>, object[]> primaryKeyDelegate = null) {
			this.PartialUpdate = partialUpdate;
			this.PrimaryKeyDelegate = primaryKeyDelegate;
		}

		/// <summary>
		///     Updates the models specified in the request body by their primary key
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
			IEnumerable<TModel> UpdatedModels;

			if (this.PartialUpdate) {
				// for each model, go through, grab the existing, and only change the parsed properties
				if (this.PrimaryKey == null)
					this.PrimaryKey = DatabaseContext.Model.FindRuntimeEntityType(typeof(TModel)).FindPrimaryKey();

				List<TModel> UpdatedList = new List<TModel>();
				foreach (ParseResult<TModel> Result in parsed) {
					// by default assume the primary key was parsed with the rest of the model
					object[] KeyValues = this.PrimaryKeyDelegate == null
						                     ? this.PrimaryKey.Properties.Select(
								                     p => p.PropertyInfo.GetGetMethod().Invoke(
									                     Result.ParsedModel,
									                     null))
							                     .ToArray()
						                     : this.PrimaryKeyDelegate(context, Result);
					TModel Existing = await DatabaseContext.Set<TModel>().FindAsync(KeyValues);

					foreach (PropertyInfo Updated in Result.PresentProperties) {
						object NewValue = Updated.GetGetMethod().Invoke(Result.ParsedModel, null);
						Updated.GetSetMethod().Invoke(Existing, new[] { NewValue });
					}

					UpdatedList.Add(Existing);
					DatabaseContext.Set<TModel>().Update(Existing);
				}

				UpdatedModels = UpdatedList;
			}
			else {
				UpdatedModels = parsed.Select(p => p.ParsedModel).ToArray();
				DatabaseContext.Set<TModel>().UpdateRange(UpdatedModels);
			}

			await DatabaseContext.SaveChangesAsync();
			return UpdatedModels;
		}
	}
}