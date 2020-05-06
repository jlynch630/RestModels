// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Extensions {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Microsoft.AspNetCore.Components.Routing;
	using Microsoft.AspNetCore.Routing;
	using Microsoft.AspNetCore.Routing.Template;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.Extensions.DependencyInjection;

	using RestModels.EntityFramework.Filters;
	using RestModels.EntityFramework.Operations;
	using RestModels.EntityFramework.Options;
	using RestModels.Exceptions;
	using RestModels.Filters;
	using RestModels.Operations;
	using RestModels.Options.Builder;
	using RestModels.ParameterRetrievers;

	/// <summary>
	///     Extension methods for the <see cref="RestModelOptionsBuilder{TModel,TUser}" /> class
	/// </summary>
	public static partial class RestModelOptionsBuilderExtensions {
		/// <summary>
		///     Ensures that the primary key of the <typeparamref name="TModel" /> will not be parsed from the request body
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> IgnorePrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			foreach (IProperty Property in RestModelOptionsBuilderExtensions.GetKeyProperties(builder))
				builder.Ignore(Property.PropertyInfo);
			return builder;
		}

		/// <summary>
		///     Ensures that the primary key of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> IncludePrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			foreach (IProperty Property in RestModelOptionsBuilderExtensions.GetKeyProperties(builder))
				builder.Include(Property.PropertyInfo);
			return builder;
		}


		/// <summary>
		///     Ensures that the primary key of the <typeparamref name="TModel" /> will be omitted from the response body
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> OmitPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			foreach (IProperty Property in RestModelOptionsBuilderExtensions.GetKeyProperties(builder))
				builder.Omit(Property.PropertyInfo);
			return builder;
		}


		/// <summary>
		///     Filter's this route's dataset by a request parameter that is compared to the primary key of the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameters">The parameters to filter by</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> FilterByPrimaryKey<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params ParameterRetriever[] parameters)
			where TModel : class where TUser : class {
			IFilter<TModel, TUser> Filter = (IFilter<TModel, TUser>)Activator.CreateInstance(
				typeof(PrimaryKeyFilter<,>).MakeGenericType(typeof(TModel), builder.GetContextType()),
				new object[] { parameters })!;
			return builder.AddFilter(Filter);
		}

		/// <summary>
		///     Filter's this route's dataset by query parameters compared to the primary key of the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterNames">The names of the query parameters to filter by</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> FilterByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params string[] parameterNames)
			where TModel : class where TUser : class =>
			RestModelOptionsBuilderExtensions.FilterByPrimaryKey<TModel, TUser, QueryParameterRetriever>(
				builder,
				parameterNames);

		/// <summary>
		///     Filter's this route's dataset by query parameters compared to the primary key of the model. The query parameters'
		///     names will be camelCased versions of their C# property name
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> FilterByPrimaryKeyQuery<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class =>
			builder.FilterByPrimaryKeyQuery(RestModelOptionsBuilderExtensions.GetKeyNames(builder));

		/// <summary>
		///     Filter's this route's dataset by route values compared to the primary key of the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterNames">The names of the query parameters to filter by</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> FilterByPrimaryKeyRoute<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder,
			params string[] parameterNames)
			where TModel : class where TUser : class =>
			RestModelOptionsBuilderExtensions.FilterByPrimaryKey<TModel, TUser, RouteValueRetriever>(
				builder,
				parameterNames);

		/// <summary>
		///     Filter's this route's dataset by route values compared to the primary key of the model. The query parameters' names
		///     will be camelCased versions of their C# property name
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> FilterByPrimaryKeyRoute<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class =>
			builder.FilterByPrimaryKeyRoute(RestModelOptionsBuilderExtensions.GetKeyNames(builder));

		/// <summary>
		///     Filter's this route's dataset by a query parameter that is compared to the primary key of the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <typeparam name="TRetriever">The type of parameter retriever to use</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterNames">The names of the query parameters to filter by</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		private static RestModelOptionsBuilder<TModel, TUser> FilterByPrimaryKey<TModel, TUser, TRetriever>(
			RestModelOptionsBuilder<TModel, TUser> builder,
			params string[] parameterNames)
			where TModel : class where TUser : class where TRetriever : ParameterRetriever {
			IEnumerable<ParameterRetriever> ParameterRetrievers = parameterNames.Select(
				n => (ParameterRetriever)Activator.CreateInstance(typeof(TRetriever), n, null)!);
			return builder.FilterByPrimaryKey(ParameterRetrievers.ToArray());
		}

		/// <summary>
		///		Gets camelCased versions of the names of the primary key properties
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The builder containing the type of database context to get the keys for</param>
		/// <returns>camelCased versions of the primary key property names, in order</returns>
		private static string[] GetKeyNames<TModel, TUser>(RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			IEnumerable<IProperty> KeyProperties = RestModelOptionsBuilderExtensions.GetKeyProperties(builder);

			IEnumerable<string> Names =
				KeyProperties.Select(p => RestModelOptionsBuilderExtensions.CamelCase(p.Name));
			return Names.ToArray();
		}

		/// <summary>
		///		camelCases a string
		/// </summary>
		/// <param name="str">The string to camelCase</param>
		/// <returns>The string in camelCase format</returns>
		private static string CamelCase(string str) {
			// little more complex to deal with ACRONYMCase --> acronymCase
			int LastUppercase = 0;
			while (LastUppercase < str.Length) {
				char Char = str[LastUppercase];
				if (Char >= 'a' && Char <= 'z')
					break;
				LastUppercase++;
			}

			LastUppercase--; // last uppercase is one before first lowercase

			if (LastUppercase < 1) LastUppercase = 1; // NotAnAcronym or alreadyCamelCase
			return str.Substring(0, LastUppercase).ToLower() + str.Substring(LastUppercase);
		}

		/// <summary>
		///		Gets the properties that make up the primary key for the model
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The builder containing the type of database context to get the keys for</param>
		/// <returns>The properties that make up the primary key, in order</returns>
		private static IEnumerable<IProperty> GetKeyProperties<TModel, TUser>(RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			Type ContextType = builder.GetContextType();
			EntityFrameworkRestModelOptionsBuilder<TModel, TUser> EntityBuilder =
				(EntityFrameworkRestModelOptionsBuilder<TModel, TUser>)builder;
			using IServiceScope Scope = EntityBuilder.App.ApplicationServices.CreateScope();
			DbContext Context = (DbContext)Scope.ServiceProvider.GetService(ContextType);
			IKey Key = Context.Model.FindRuntimeEntityType(typeof(TModel)).FindPrimaryKey();

			return Key.Properties;
		}

		/// <summary>
		///     Creates a new operation for the specified builder
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="operationType">The type of operation to create</param>
		/// <param name="builder">The builder to create the operation for</param>
		/// <returns>The created operation</returns>
		private static IOperation<TModel, TUser> NewOperation<TModel, TUser>(
			RestModelOptionsBuilder<TModel, TUser> builder, Type operationType)
			where TModel : class where TUser : class {
			IOperation<TModel, TUser> Operation =
				(IOperation<TModel, TUser>)Activator.CreateInstance(
					operationType.MakeGenericType(typeof(TModel), builder.GetContextType()))!;
			return Operation;
		}

		/// <summary>
		///		Gets the context type for the given instance of <see cref="EntityFrameworkRestModelOptionsBuilder{TModel, TUser}"/>
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The builder to get the context type for</param>
		/// <returns>The context type for the given builder</returns>
		private static Type GetContextType<TModel, TUser>(this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			if (!(builder is EntityFrameworkRestModelOptionsBuilder<TModel, TUser> EntityBuilder))
				throw new OptionsException(
					"This builder method can only be called on an instance of the EntityFrameworkRestModelOptionsBuilder type. If you intended to build this API using EntityFramework, make sure to call UseEntityFrameworkRestModels instead of UseRestModels");
			return EntityBuilder.ContextType;
		}

		/// <summary>
		///     Creates an update operation using the given properties and parameter retrievers
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder containing the database context type</param>
		/// <param name="properties">The properties to initialize the operation with</param>
		/// <param name="retrievers">The retrievers to initialize the operation with. May be null.</param>
		/// <returns>The created update operation</returns>
		private static IOperation<TModel, TUser> CreateUpdateOperation<TModel, TUser>(
			RestModelOptionsBuilder<TModel, TUser> builder,
			PropertyInfo[] properties,
			ParameterRetriever[]? retrievers)
			where TModel : class where TUser : class {
			return (IOperation<TModel, TUser>)Activator.CreateInstance(
				typeof(UpdateOperation<,>).MakeGenericType(typeof(TModel), builder.GetContextType()),
				new object?[] { properties, retrievers })!;
		}

		/// <summary>
		///     Extracts a property from an expression
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TProperty">The type of the property to extract</typeparam>
		/// <param name="propertyExpression">The expression that refers to a property</param>
		/// <returns>The extracted property</returns>
		private static PropertyInfo ExtractProperty<TModel, TProperty>(
			Expression<Func<TModel, TProperty>> propertyExpression)
			where TModel : class {
			MemberExpression MemberExpression;
			if (propertyExpression.Body is UnaryExpression Body)
				MemberExpression = (MemberExpression)Body.Operand;
			else MemberExpression = (MemberExpression)propertyExpression.Body;

			return (PropertyInfo)MemberExpression.Member;
		}
	}
}