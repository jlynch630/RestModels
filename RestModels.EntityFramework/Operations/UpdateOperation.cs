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
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;

	using RestModels.Context;
	using RestModels.Exceptions;
	using RestModels.Filters;
	using RestModels.Operations;
	using RestModels.ParameterRetrievers;
	using RestModels.Parsers;

	/// <summary>
	///     An operation that will update models in an EntityFramework context
	/// </summary>
	/// <typeparam name="TModel">The type of model to update</typeparam>
	/// <typeparam name="TContext">The type of EntityFramework database context to use</typeparam>
	public class UpdateOperation<TModel, TContext> : IOperation<TModel>
		where TModel : class where TContext : DbContext {
		/// <summary>
		///     A delegate that will retrieve the primary key(s) for a given model
		/// </summary>
		private readonly ParameterRetriever[]? ParameterDelegates;

		/// <summary>
		///     The properties to compare to determine which model to update
		/// </summary>
		private readonly PropertyInfo[] Properties;

		/// <summary>
		///     Initializes a new instance of the <see cref="UpdateOperation{TModel,TContext}" /> class.
		/// </summary>
		/// <param name="properties">The properties to compare when determining which model to update</param>
		/// <param name="parameterDelegates">
		///     A delegate that will retrieve the values of the given properties for a request. If
		///     null or omitted, the properties are assumed to be present in the parsed model
		/// </param>
		public UpdateOperation(PropertyInfo[] properties, ParameterRetriever[]? parameterDelegates = null) {
			this.ParameterDelegates = parameterDelegates;
			this.Properties = properties;
		}

		/// <summary>
		///     Updates the models specified in the request body by their primary key
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="dataset">The filtered dataset to operate on</param>
		/// <returns>The affected models</returns>
		public async Task<IEnumerable<TModel>> OperateAsync(
			IApiContext<TModel, object> context,
			IQueryable<TModel> dataset) {
			TContext DatabaseContext = context.Services.GetRequiredService<TContext>();

			// todo: make better
			List<TModel> UpdatedList = new List<TModel>();
			if (context.Parsed == null) throw new OperationFailedException("Must have a parsed body to update");

			foreach (ParseResult<TModel> Result in context.Parsed) {
				// by default assume the properties were parsed with the rest of the model
				// todo: this is the ugliest thing i have ever seen
				object?[] Values = this.ParameterDelegates == null
					                   ? this.Properties.Select(p => p.GetGetMethod()?.Invoke(Result.ParsedModel, null))
						                   .ToArray()
					                   : this.Properties.Zip(this.ParameterDelegates).Select(
						                   v => ParameterResolver.ParseParameter(
							                   v.Second.GetValue(context.Request),
							                   v.First.PropertyType)).ToArray();

				if (Values.Any(v => v == null))
					throw new OperationFailedException(
						"Missing expected value for update operation",
						new ArgumentNullException(
							this.Properties[Array.FindIndex(Values, v => v == null)].Name,
							"Parameter was null"));

				// so that ef can still use sql queries, let's generate an expression tree for it to use
				ParameterExpression ModelParameter = Expression.Parameter(typeof(TModel));

				// generate expressions. boils down to: ModelParameter.Property == Values[i]
				Expression[] ComparisonExpressions = this.Properties.Zip(Values).Select(
					v => (Expression)Expression.Equal(
						Expression.Property(ModelParameter, v.First),
						Expression.Constant(v.Second))).ToArray();

				// && them all together
				Expression AggregateExpression = ComparisonExpressions.Aggregate(Expression.AndAlso);

				// and create the lambda
				Expression<Func<TModel, bool>> FilterExpression =
					Expression.Lambda<Func<TModel, bool>>(AggregateExpression, ModelParameter);

				IEnumerable<TModel> ExistingModels = dataset.Where(FilterExpression);

				foreach (TModel Existing in ExistingModels) {
					foreach (PropertyInfo Updated in Result.PresentProperties) {
						object? NewValue = Updated.GetGetMethod()?.Invoke(Result.ParsedModel, null);
						Updated.GetSetMethod()?.Invoke(Existing, new[] { NewValue });
					}

					UpdatedList.Add(Existing);
					DatabaseContext.Set<TModel>().Update(Existing);
				}
			}

			await DatabaseContext.SaveChangesAsync();
			return UpdatedList;
		}
	}
}