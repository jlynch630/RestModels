// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilderExtensions.Get.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace - keep it all together
namespace RestModels.Extensions {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.AspNetCore.Routing.Template;

	using RestModels.Options.Builder;
	using RestModels.ParameterRetrievers;

	/// <summary>
	///     Extension methods for the <see cref="RestModelOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static partial class RestModelOptionsBuilderExtensions {
		/// <summary>
		///     Sets this route up to handle a GET request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>This function just calls <see cref="RestModelOptionsBuilder{TModel, TUser}.SetupGet(string, Action{RestModelOptionsBuilder{TModel, TUser}}?)"/>, just makes GET requests more consistent in Entity Framework with the other requests</remarks>
		public static RestModelOptionsBuilder<TModel, TUser> Get<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string? routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class {
			return builder.SetupGet(routePattern, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a GET request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>This function simply calls <see cref="RestModelOptionsBuilder{TModel, TUser}.SetupGet(Action{RestModelOptionsBuilder{TModel, TUser}}?)"/>, makes GET requests more consistent in Entity Framework with the other requests</remarks>
		public static RestModelOptionsBuilder<TModel, TUser> Get<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class {
			return builder.SetupGet(optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a GET request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>This function simply calls <see cref="RestModelOptionsBuilder{TModel, TUser}.SetupGet()"/>, makes GET requests more consistent in Entity Framework with the other requests</remarks>
		public static RestModelOptionsBuilder<TModel, TUser> Get<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			return builder.SetupGet();
		}

		/// <summary>
		///     Sets this route up to handle a GET by primary key request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameters">The parameters that will make up the primary key to filter by</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			ParameterRetriever[] parameters,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.SetupGet(
				routePattern,
				o => {
					o.FilterByPrimaryKey(parameters);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a GET request by route values using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterNames">The names of the route value parameters to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string[] parameterNames,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.SetupGet(
				routePattern,
				o => {
					o.FilterByPrimaryKeyRoute(parameterNames);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a GET request by route values using an ORM. The names of the route values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			string UpdatedPattern = string.Empty;
			IEnumerable<string> ExistingNames =
				TemplateParser.Parse(builder.RoutePattern).Parameters.Select(p => p.Name);
			string[] KeyNames = RestModelOptionsBuilderExtensions.GetKeyNames(builder);

			foreach (string KeyName in KeyNames.Except(ExistingNames)) {
				if (!UpdatedPattern.EndsWith("/")) UpdatedPattern += "/";
				UpdatedPattern += $"{{{KeyName}}}/";
			}

			return builder.GetByPrimaryKey(UpdatedPattern, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a GET request by route values using an ORM. The names of the route values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
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
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			string UpdatedPattern = routePattern;
			string[] ExistingNames = TemplateParser.Parse(routePattern).Parameters.Select(p => p.Name).ToArray();
			string[] KeyNames = RestModelOptionsBuilderExtensions.GetKeyNames(builder);

			foreach (string KeyName in KeyNames.Except(ExistingNames)) {
				if (!UpdatedPattern.EndsWith("/")) UpdatedPattern += "/";
				UpdatedPattern += $"{{{KeyName}}}/";
			}

			return builder.GetByPrimaryKey(UpdatedPattern, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a GET request by route values using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterName">The name of the route value parameter to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string parameterName,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.GetByPrimaryKey(routePattern, new[] { parameterName }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a GET request by query parameters using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterNames">The names of the route value parameters to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string[] parameterNames,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.SetupGet(
				routePattern,
				o => {
					o.FilterByPrimaryKeyQuery(parameterNames);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a GET request by a primary key query parameter using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterName">The name of the route value parameter to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string parameterName,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.GetByPrimaryKeyQuery(routePattern, new[] { parameterName }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a GET request by query parameters using an ORM. The names of the query
		///     parameters will be camelCased names of the primary key of the entity
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.GetByPrimaryKeyQuery(
				routePattern,
				RestModelOptionsBuilderExtensions.GetKeyNames(builder),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a GET request by query parameters on the same route pattern using an ORM.
		///     The names of the query parameters will be camelCased names of the primary key of the entity
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> GetByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.GetByPrimaryKeyQuery("", RestModelOptionsBuilderExtensions.GetKeyNames(builder), optionsHandler);
	}
}