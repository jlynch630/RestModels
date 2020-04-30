// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Extensions {
	using System;
	using Microsoft.EntityFrameworkCore;
	using RestModels.EntityFramework.Operations;
	using RestModels.EntityFramework.Options;
	using RestModels.Operations;
	using RestModels.Options.Builder;

	/// <summary>
	///     Extension methods for the <see cref="RestModelOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static class RestModelOptionsBuilderExtensions {
		/// <summary>
		///     Sets this route up to handle a POST creation request using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string variableName,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TUser : class {
			IOperation<TModel, TUser> CreateOperation =
				RestModelOptionsBuilderExtensions.NewOperation<TModel, TUser, CreateOperation<TModel, DbContext>>(builder);
			return builder.Post(
				routePattern,
				(o) => {
					o.UseOperation(CreateOperation);
					optionsHandler(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a POST creation request using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> Post<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TUser : class {
			IOperation<TModel, TUser> CreateOperation =
				RestModelOptionsBuilderExtensions.NewOperation<TModel, TUser, CreateOperation<TModel, DbContext>>(builder);
			return builder.Post(
				routePattern,
				(o) => {
					o.UseOperation(CreateOperation);
					optionsHandler(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a DELETE request using Entity Framework
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>
		///		If not used with any filters, this route will drop all models in the dataset
		/// </remarks>
		public static RestModelOptionsBuilder<TModel, TUser> Delete<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler)
			where TModel : class where TUser : class {
			IOperation<TModel, TUser> DeleteOperation =
				RestModelOptionsBuilderExtensions.NewOperation<TModel, TUser, DeleteOperation<TModel, DbContext>>(builder);
			return builder.Post(
				routePattern,
				(o) => {
					o.UseOperation(DeleteOperation);
					optionsHandler(o);
				});
		}

		/// <summary>
		///		Creates a new operation for the specified builder
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <typeparam name="TOperation">The type of operation to create</typeparam>
		/// <param name="builder">The builder to create the operation for</param>
		/// <returns>The created operation</returns>
		private static IOperation<TModel, TUser> NewOperation<TModel, TUser, TOperation>(RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class where TOperation : IOperation<TModel, TUser> {
			Type ContextType = ((EntityFrameworkRestModelOptionsBuilder<TModel, TUser>)builder).ContextType;
			IOperation<TModel, TUser> Operation = (IOperation<TModel, TUser>)Activator.CreateInstance(typeof(TOperation).MakeGenericType(typeof(TModel), ContextType));
			return Operation;
		}
	}
}