// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilderExtensions.PutUpdate.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace - keep it all together
namespace RestModels.Extensions {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	using Microsoft.AspNetCore.Routing.Template;

	using RestModels.Exceptions;
	using RestModels.Operations;
	using RestModels.Options.Builder;
	using RestModels.OrmBase.Options;
	using RestModels.ParameterRetrievers;

	/// <summary>
	///     Extension methods for the <see cref="RestModelOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static partial class RestModelOptionsBuilderExtensions {
		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     route value to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="routeValues">The route value names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			string[] routeValues,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByParam(
				routePattern,
				propertyExpressions,
				routeValues.Select(p => (ParameterRetriever)new RouteValueRetriever(p)).ToArray(),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     route value to determine which elements to update. The names of the route values will be camelCase versions of the
		///     C# properties to compare. If these route values do not yet exist, they will be added to the end of the route
		///     pattern in order.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class {
			string ExistingPattern = $"{builder.RoutePattern}{(builder.RoutePattern.EndsWith("/") ? "" : "/")}";
			if (routePattern.StartsWith("/")) ExistingPattern += routePattern.Substring(1);
			else ExistingPattern += routePattern;

			string NewPattern = routePattern;
			IEnumerable<string> ExistingNames = TemplateParser.Parse(ExistingPattern).Parameters.Select(p => p.Name);
			string[] KeyNames = propertyExpressions.Select(RestModelOptionsBuilderExtensions.ExtractProperty)
				.Select(p => RestModelOptionsBuilderExtensions.CamelCase(p.Name)).ToArray();
			foreach (string KeyName in KeyNames.Except(ExistingNames)) {
				if (!NewPattern.EndsWith("/")) NewPattern += "/";
				NewPattern += $"{{{KeyName}}}/";
			}

			return builder.PutUpdateBy(NewPattern, propertyExpressions, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM at the same route pattern, comparing the
		///     given property with a route value to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="routeValues">The route value names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			string[] routeValues,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateBy("", propertyExpressions, routeValues, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     route value to determine which elements to update. The names of the route values will be camelCase versions of the
		///     C# properties to compare. If these route values do not yet exist, they will be added to the end of the route
		///     pattern in order.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PutUpdateBy("", propertyExpressions, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     route value to determine which elements to update. The names of the route values will be camelCase versions of the
		///     C# properties to compare. If these route values do not yet exist, they will be added to the end of the route
		///     pattern in order.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PutUpdateBy(propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     route value to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="routeValue">The route value names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			string routeValue,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateBy(routePattern, new[] { propertyExpression }, new[] { routeValue }, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     route value to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="routeValue">The route value names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			string routeValue,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateBy("", propertyExpression, routeValue, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM at the same route, comparing the given
		///     property with a route value to determine which elements to update. The name of the route value will be a camelCase
		///     version of the C# property to compare. If this route value does not yet exist, it will be added to the end of the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateBy(new[] { propertyExpression }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM at the same route, comparing the given
		///     property with a route value to determine which elements to update. The name of the route value will be a camelCase
		///     version of the C# property to compare. If this route value does not yet exist, it will be added to the end of the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateBy(routePattern, new[] { propertyExpression }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM at the same route, comparing the given
		///     property with a route value to determine which elements to update. The name of the route value will be a camelCase
		///     version of the C# property to compare. If this route value does not yet exist, it will be added to the end of the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateBy<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PutUpdateBy(routePattern, propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM. The property to compare is assumed to be
		///     directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateByParam(routePattern, new[] { propertyExpression }, null, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM. The property to compare is assumed to be
		///     directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByParam("", propertyExpression, null, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM. The properties to compare are assumed to
		///     be directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PutUpdateByBody(routePattern, propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM. The properties to compare are assumed to
		///     be directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PutUpdateByParam(routePattern, propertyExpressions, null, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM. The properties to compare are assumed to
		///     be directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PutUpdateByParam(propertyExpressions, null, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM. The properties to compare are assumed to
		///     be directly set in the parsed model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByBody<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PutUpdateByParam(propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="retrievers">The parameters to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByParam<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			ParameterRetriever[]? retrievers,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			if (!(builder is IOrmRestModelOptionsBuilder<TModel, TUser> OrmBuilder))
				throw new OptionsException(
					$"PutUpdate may only be called on a builder that inherits from {nameof(IOrmRestModelOptionsBuilder<TModel, TUser>)}");
			PropertyInfo[] Infos = propertyExpressions.Select(RestModelOptionsBuilderExtensions.ExtractProperty)
				.ToArray();
			IOperation<TModel, TUser> Operation = OrmBuilder.GetUpdateOperation(Infos, retrievers);

			return builder.SetupPut(
				routePattern,
				o => {
					o.UseOperation(Operation);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="retriever">The parameter to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByParam<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			ParameterRetriever? retriever,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateByParam(
				routePattern,
				new[] { propertyExpression },
				retriever == null ? null : new[] { retriever },
				optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when update the model</param>
		/// <param name="retrievers">The parameters to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByParam<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			ParameterRetriever[]? retrievers,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByParam("", propertyExpressions, retrievers, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update by primary key request using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameters">The parameters that will make up the primary key to filter by</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			ParameterRetriever[] parameters,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			if (!(builder is IOrmRestModelOptionsBuilder<TModel, TUser> OrmBuilder))
				throw new OptionsException(
					$"PutUpdate may only be called on a builder that inherits from {nameof(IOrmRestModelOptionsBuilder<TModel, TUser>)}");
			PropertyInfo[] Properties = RestModelOptionsBuilderExtensions.GetPrimaryKey(builder)
				.Select(p => p.PropertyInfo).ToArray();

			IOperation<TModel, TUser> Operation = OrmBuilder.GetUpdateOperation(Properties, parameters);

			return builder.SetupPut(
				routePattern,
				o => {
					o.UseOperation(
						Operation);
					optionsHandler?.Invoke(o);
				});
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request by route values using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterNames">The names of the route value parameters to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string[] parameterNames,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateByPrimaryKey(
				routePattern,
				parameterNames.Select(p => (ParameterRetriever)new RouteValueRetriever(p)).ToArray(),
				optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request by route values using an ORM. The names of the route
		///     values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class =>
			builder.PutUpdateByPrimaryKey(null);

		/// <summary>
		///     Sets this route up to handle a PUT update request by route values using an ORM. The names of the route
		///     values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PutUpdateByPrimaryKey("", optionsHandler);

		/// <summary>
		///     Sets a route up to handle a PUT update request by route values using an ORM. The names of the route
		///     values
		///     will be camelCased versions of the C# property name. If these route parameters do not already exist they will be
		///     added in order, e.g. /key1/key2.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			string ExistingPattern = $"{builder.RoutePattern}{(builder.RoutePattern.EndsWith("/") ? "" : "/")}";
			if (routePattern.StartsWith("/")) ExistingPattern += routePattern.Substring(1);
			else ExistingPattern += routePattern;

			string NewPattern = routePattern;
			IEnumerable<string> ExistingNames = TemplateParser.Parse(ExistingPattern).Parameters.Select(p => p.Name);
			string[] KeyNames = RestModelOptionsBuilderExtensions.GetKeyNames(builder);
			foreach (string KeyName in KeyNames.Except(ExistingNames)) {
				if (!NewPattern.EndsWith("/")) NewPattern += "/";
				NewPattern += $"{{{KeyName}}}/";
			}

			return builder.PutUpdateByPrimaryKey(NewPattern, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets a route up to handle a PUT update request by route values using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterName">The name of the route value parameter to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string parameterName,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateByPrimaryKey(routePattern, new[] { parameterName }, optionsHandler);
		}

		/// <summary>
		///     Sets a route up to handle a PUT update request by query parameters using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterNames">The names of the route value parameters to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string[] parameterNames,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateByPrimaryKey(
				routePattern,
				parameterNames.Select(p => (ParameterRetriever)new QueryParameterRetriever(p)).ToArray(),
				optionsHandler);
		}

		/// <summary>
		///     Sets a route up to handle a PUT update request by a primary key query parameter using an ORM
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="parameterName">The name of the route value parameter to use</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			string parameterName,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateByPrimaryKeyQuery(routePattern, new[] { parameterName }, optionsHandler);
		}

		/// <summary>
		///     Sets a route up to handle a PUT update request by query parameters using an ORM. The names of the
		///     query
		///     parameters will be camelCased names of the primary key of the entity
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByPrimaryKeyQuery(
				routePattern,
				RestModelOptionsBuilderExtensions.GetKeyNames(builder),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request by query parameters on the same route pattern using Entity
		///     Framework.
		///     The names of the query parameters will be camelCased names of the primary key of the entity
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByPrimaryKeyQuery(
				"",
				RestModelOptionsBuilderExtensions.GetKeyNames(builder),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     query parameter to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="queryParameters">The query parameter names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			string[] queryParameters,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByParam(
				routePattern,
				propertyExpressions,
				queryParameters.Select(p => (ParameterRetriever)new QueryParameterRetriever(p)).ToArray(),
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     query parameter to determine which elements to update. The names of the query parameters will be camelCase versions
		///     of the
		///     C# properties to compare.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class {
			string[] KeyNames = propertyExpressions.Select(RestModelOptionsBuilderExtensions.ExtractProperty)
				.Select(p => RestModelOptionsBuilderExtensions.CamelCase(p.Name)).ToArray();
			return builder.PutUpdateByQuery(routePattern, propertyExpressions, KeyNames, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM at the same route pattern, comparing the
		///     given property with a query parameter to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="queryParameters">The query parameter names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			string[] queryParameters,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByQuery("", propertyExpressions, queryParameters, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     query parameter to determine which elements to update. The names of the query parameters will be camelCase versions
		///     of the
		///     C# properties to compare.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>>[] propertyExpressions,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler)
			where TModel : class where TUser : class =>
			builder.PutUpdateByQuery("", propertyExpressions, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     query parameter to determine which elements to update. The names of the query parameters will be camelCase versions
		///     of the
		///     C# properties to compare.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PutUpdateByQuery(propertyExpressions, null);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     query parameter to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="queryParameter">The query parameter names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			string queryParameter,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByQuery(
				routePattern,
				new[] { propertyExpression },
				new[] { queryParameter },
				optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM, comparing the given property with a
		///     query parameter to determine which elements to update
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="queryParameter">The query parameter names to use to get the property values for a request</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			string queryParameter,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class =>
			builder.PutUpdateByQuery("", propertyExpression, queryParameter, optionsHandler);

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM at the same route, comparing the given
		///     property with a query parameter to determine which elements to update. The name of the query parameter will be a
		///     camelCase
		///     version of the C# property to compare. If this query parameter does not yet exist, it will be added to the end of
		///     the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateByQuery(new[] { propertyExpression }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM at the same route, comparing the given
		///     property with a query parameter to determine which elements to update. The name of the query parameter will be a
		///     camelCase
		///     version of the C# property to compare. If this query parameter does not yet exist, it will be added to the end of
		///     the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpression">An expression representing the property to compare when updating the model</param>
		/// <param name="optionsHandler">A handler for the route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			Expression<Func<TModel, object>> propertyExpression,
			Action<RestModelOptionsBuilder<TModel, TUser>>? optionsHandler = null)
			where TModel : class where TUser : class {
			return builder.PutUpdateByQuery(routePattern, new[] { propertyExpression }, optionsHandler);
		}

		/// <summary>
		///     Sets this route up to handle a PUT update request using an ORM at the same route, comparing the given
		///     property with a query parameter to determine which elements to update. The name of the query parameter will be a
		///     camelCase
		///     version of the C# property to compare. If this query parameter does not yet exist, it will be added to the end of
		///     the
		///     route pattern.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="routePattern">The route pattern to set up the request for</param>
		/// <param name="propertyExpressions">Expressions representing the properties to compare when updating the model</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> PutUpdateByQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			string routePattern,
			params Expression<Func<TModel, object>>[] propertyExpressions)
			where TModel : class where TUser : class =>
			builder.PutUpdateByQuery(routePattern, propertyExpressions, null);
	}
}