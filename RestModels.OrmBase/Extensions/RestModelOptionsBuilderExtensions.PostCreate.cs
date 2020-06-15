// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilderExtensions.PostCreate.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace - keep it all together
namespace RestModels.Extensions {
	using System;

	using RestModels.Exceptions;
	using RestModels.Operations;
	using RestModels.Options.Builder;
	using RestModels.OrmBase.Options;

	/// <summary>
	///     Extension methods for the <see cref="RestModelOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static partial class RestModelOptionsBuilderExtensions {
		/// <summary>
		///     Sets this route up to handle a POST creation request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PostCreate<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			if (!(builder is IOrmRestModelOptionsBuilder<TModel, TUser> OrmBuilder))
				throw new OptionsException(
					$"PostCreate may only be called on a builder that inherits from {nameof(IOrmRestModelOptionsBuilder<TModel, TUser>)}");

			IOperation<TModel, TUser> CreateOperation = OrmBuilder.GetCreateOperation();
			return builder.SetupPost(
				routePattern,
				o => {
					o.IgnorePrimaryKey();
					o.UseOperation(CreateOperation);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a POST creation request with the same route pattern using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PostCreate<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PostCreate(string.Empty, optionsHandler);
	}
}