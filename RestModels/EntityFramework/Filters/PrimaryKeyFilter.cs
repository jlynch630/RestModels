// -----------------------------------------------------------------------
// <copyright file="PrimaryKeyFilter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Filters {
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.Extensions.DependencyInjection;

	using RestModels.Exceptions;
	using RestModels.Filters;
	using RestModels.ParameterRetrievers;
	using RestModels.Parsers;

	/// <summary>
	///     A filter that filters the dataset by the primary key of the model.
	/// </summary>
	/// <typeparam name="TModel">The type of model being filtered</typeparam>
	/// <typeparam name="TContext">The context to use to get information about the primary key</typeparam>
	public class PrimaryKeyFilter<TModel, TContext> : IFilter<TModel>
		where TModel : class where TContext : DbContext {
		/// <summary>
		///     The parameter names for the primary keys, in the order of the keys
		/// </summary>
		private readonly ParameterRetriever[] Parameters;

		/// <summary>
		///     The primary key of the model
		/// </summary>
		private IKey? PrimaryKey;

		/// <summary>
		///     Initializes a new instance of the <see cref="PrimaryKeyFilter{TModel,TContext}" /> class.
		/// </summary>
		/// <param name="parameters">The parameters for the primary keys, in the order of the keys</param>
		public PrimaryKeyFilter(ParameterRetriever[] parameters) => this.Parameters = parameters;

		/// <summary>
		///     Filters the model dataset by the primary key obtained from the request context
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="dataset">The current dataset to be filtered</param>
		/// <param name="parsed">The parsed request body, if any</param>
		/// <param name="user">The current user context, if any</param>
		/// <returns>The filtered dataset</returns>
		public async Task<IQueryable<TModel>> FilterDataAsync(
			HttpContext context,
			IQueryable<TModel> dataset,
			ParseResult<TModel>[] parsed,
			object user) {
			TContext DatabaseContext = context.RequestServices.GetRequiredService<TContext>();

			// assign if not null. what a genius idea
			this.PrimaryKey ??= DatabaseContext.Model.FindRuntimeEntityType(typeof(TModel)).FindPrimaryKey();

			// step one is just get the keys
			object[] PrimaryKeyValues = this.Parameters.Select(
				(param, i) => {
					// retrieve value
					string ParameterValue = param.GetValue(context.Request)
					                        ?? throw new ConditionFailedException("No value provided for primary key");
					Type KeyType = this.PrimaryKey.Properties[i].PropertyInfo.PropertyType;

					// and parse
					return ParameterResolver.ParseParameter(ParameterValue, KeyType);
				}).ToArray();

			// so that ef can still use sql queries, let's generate an expression tree for it to use
			ParameterExpression ModelParameter = Expression.Parameter(typeof(TModel));

			// generate expressions. boils down to: ModelParameter.Property == PrimaryKeyValues[i]
			Expression[] ComparisonExpressions = this.PrimaryKey.Properties.Select(
				(property, i) => (Expression)Expression.Equal(
					Expression.Property(ModelParameter, property.PropertyInfo),
					Expression.Constant(PrimaryKeyValues[i]))).ToArray();

			// && them all together
			Expression AggregateExpression = ComparisonExpressions.Aggregate(Expression.AndAlso);

			// and create the lambda
			Expression<Func<TModel, bool>> FilterExpression =
				Expression.Lambda<Func<TModel, bool>>(AggregateExpression, ModelParameter);

			return dataset.Where(FilterExpression);
		}
	}
}