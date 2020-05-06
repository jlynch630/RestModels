// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	using Microsoft.AspNetCore.Builder;

	using RestModels.Auth;
	using RestModels.Conditions;
	using RestModels.Exceptions;
	using RestModels.Filters;
	using RestModels.Models;
	using RestModels.Operations;
	using RestModels.Parsers;
	using RestModels.Results;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The children options being built off of this one
		/// </summary>
		private readonly List<RestModelOptionsBuilder<TModel, TUser>> Children =
			new List<RestModelOptionsBuilder<TModel, TUser>>();

		/// <summary>
		///     The options object that's being built
		/// </summary>
		private readonly RestModelOptions<TModel, TUser> Options;

		/// <summary>
		///     Initializes a new instance of the <see cref="RestModelOptionsBuilder{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="existing">Starting options for this builder</param>
		public RestModelOptionsBuilder(RestModelOptions<TModel, TUser>? existing = null) =>
			this.Options = existing ?? new RestModelOptions<TModel, TUser>();

		/// <summary>
		///     Initializes a new instance of the <see cref="RestModelOptionsBuilder{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="baseRoute">The base route for these options</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		public RestModelOptionsBuilder(string baseRoute, Action<IEndpointConventionBuilder>? routeOptionsHandler)
			: this() {
			this.Options.RoutePattern = RestModelOptionsBuilder<TModel, TUser>.FixRoute(baseRoute);
			this.Options.RouteOptionsHandler = routeOptionsHandler;
		}

		/// <summary>
		///     The route pattern for this builder
		/// </summary>
		public string RoutePattern => this.Options.RoutePattern;

		/// <summary>
		///     Sets whether or not to accept arrays of <typeparamref name="TModel" /> as the request body
		/// </summary>
		/// <param name="accept">A value indicating whether or not to accept arrays as the request body</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AcceptArrays(bool accept = true) {
			this.Options.ParserOptions.ParseArrays = accept;
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route
		/// </summary>
		/// <param name="authProvider">The auth provider to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddAuthProvider(IAuthProvider<TModel, TUser> authProvider) {
			if (this.Options.AuthProviders == null)
				this.Options.AuthProviders = new List<IAuthProvider<TModel, TUser>>();
			this.Options.AuthProviders.Add(authProvider);
			return this;
		}

		/// <summary>
		///     Adds an auth provider to this route
		/// </summary>
		/// <typeparam name="TProvider">The type of auth provider to add</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddAuthProvider<TProvider>()
			where TProvider : IAuthProvider<TModel, TUser>, new() {
			this.AddAuthProvider(new TProvider());
			return this;
		}

		/// <summary>
		///     Adds a body parser to this route
		/// </summary>
		/// <param name="bodyParser">The body parser to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddBodyParser(IBodyParser<TModel> bodyParser) {
			if (this.Options.BodyParsers == null) this.Options.BodyParsers = new List<IBodyParser<TModel>>();
			this.Options.BodyParsers.Add(bodyParser);
			return this;
		}

		/// <summary>
		///     Adds a new body parser to this route
		/// </summary>
		/// <typeparam name="TParser">The type of body parser to add</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddBodyParser<TParser>()
			where TParser : IBodyParser<TModel>, new() {
			this.AddBodyParser(new TParser());
			return this;
		}

		/// <summary>
		///     Adds a condition to this route, which much be met for the request to succeed
		/// </summary>
		/// <param name="condition">The condition to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddCondition(ICondition<TModel, TUser> condition) {
			this.Options.Conditions.Add(condition);
			return this;
		}

		/// <summary>
		///     Adds a filter to this route, which will filter the dataset retrieved
		/// </summary>
		/// <param name="filter">The filter to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddFilter(IFilter<TModel, TUser> filter) {
			this.Options.Filters.Add(filter);
			return this;
		}

		/// <summary>
		///     Adds a request method that this route supports
		/// </summary>
		/// <param name="requestMethod">The request method to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddRequestMethod(string requestMethod) {
			if (this.Options.RequestMethods == null) this.Options.RequestMethods = new HashSet<string>();
			this.Options.RequestMethods.Add(requestMethod);
			return this;
		}

		/// <summary>
		///     Clears all of the auth providers registered for this <see cref="RestModelOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ClearAuthProviders() {
			this.Options.AuthProviders = null;
			return this;
		}

		/// <summary>
		///     Clears all of the body parsers registered for this <see cref="RestModelOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ClearBodyParsers() {
			this.Options.BodyParsers = null;
			return this;
		}

		/// <summary>
		///     Clears all of the conditions registered for this <see cref="RestModelOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ClearConditions() {
			this.Options.Conditions.Clear();
			return this;
		}

		/// <summary>
		///     Clears all of the exception handlers registered for this <see cref="RestModelOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ClearExceptionHandlers() {
			this.Options.ExceptionHandlers.Clear();
			return this;
		}

		/// <summary>
		///     Clears all of the filters registered for this <see cref="RestModelOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ClearFilters() {
			this.Options.Filters.Clear();
			return this;
		}

		/// <summary>
		///     Clears the operation registered for this <see cref="RestModelOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ClearOperation() {
			this.Options.Operation = null;
			return this;
		}

		/// <summary>
		///     Clears all of the request methods registered for this <see cref="RestModelOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ClearRequestMethods() {
			this.Options.RequestMethods = null;
			return this;
		}

		/// <summary>
		///     Clears the result writer registered for this <see cref="RestModelOptionsBuilder{TModel, TUser}" />
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ClearResultWriter() {
			this.Options.ResultWriter = null;
			return this;
		}

		/// <summary>
		///     Sets a default value for a property of the <typeparamref name="TModel" /> parsed from the request body
		/// </summary>
		/// <param name="property">The property to set the default for</param>
		/// <param name="defaultValue">
		///     A delegate that will retrieve the default value for the given <paramref name="property" />
		/// </param>
		/// <typeparam name="TProperty">The type of the property for to set the default for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Default<TProperty>(
			PropertyInfo property,
			Func<TProperty> defaultValue) {
			// double lambda looks silly but we can't cast Func<TProperty> to Func<object> without making it constrained to reference types
			this.Options.ParserOptions.DefaultPropertyValues.Add(property, () => defaultValue());
			return this;
		}

		/// <summary>
		///     Sets a default value for a property of the <typeparamref name="TModel" /> parsed from the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to set a default for</param>
		/// <param name="defaultValue">
		///     A delegate that will retrieve the default value for the property defined by
		///     <paramref name="propertyExpression" />
		/// </param>
		/// <typeparam name="TProperty">The type of the property for to set the default for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Default<TProperty>(
			Expression<Func<TModel, TProperty>> propertyExpression,
			Func<TProperty> defaultValue) {
			PropertyInfo Info = RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(propertyExpression);

			// double lambda looks silly but we can't cast Func<TProperty> to Func<object> without making it constrained to reference types
			// todo: would there ever be a non-reference type in a model though?
			this.Options.ParserOptions.DefaultPropertyValues.Add(Info, () => defaultValue());
			return this;
		}

		/// <summary>
		///     Sets a default value for a property of the <typeparamref name="TModel" /> parsed from the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to set a default for</param>
		/// <param name="defaultValue">The default value for the property defined by <paramref name="propertyExpression" /></param>
		/// <typeparam name="TProperty">The type of the property for to set the default for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Default<TProperty>(
			Expression<Func<TModel, TProperty>> propertyExpression,
			TProperty defaultValue) {
			PropertyInfo Info = RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(propertyExpression);
			this.Options.ParserOptions.DefaultPropertyValues.Add(Info, () => defaultValue);
			return this;
		}

		/// <summary>
		///     Maps another route to the same route pattern
		/// </summary>
		/// <returns>The new <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object</returns>
		/// <remarks>
		///     This method works just like
		///     <see cref="MapRoute(string, Action{RestModelOptionsBuilder{TModel, TUser}}, Action{IEndpointConventionBuilder})" />
		///     , but returns the newly created <see cref="RestModelOptionsBuilder{TModel, TUser}" />, rather than the one used to
		///     create the route.
		/// </remarks>
		public RestModelOptionsBuilder<TModel, TUser> FlatMap() => this.FlatMap(this.Options.RoutePattern, null);

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <returns>The new <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object</returns>
		/// <remarks>
		///     This method works just like
		///     <see cref="MapRoute(string, Action{RestModelOptionsBuilder{TModel, TUser}}, Action{IEndpointConventionBuilder})" />
		///     , but returns the newly created <see cref="RestModelOptionsBuilder{TModel, TUser}" />, rather than the one used to
		///     create the route.
		/// </remarks>
		public RestModelOptionsBuilder<TModel, TUser> FlatMap(string? pattern) => this.FlatMap(pattern, null);

		/// <summary>
		///     Maps another route to the same route pattern
		/// </summary>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>The new <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object</returns>
		/// <remarks>
		///     This method works just like
		///     <see cref="MapRoute(string, Action{RestModelOptionsBuilder{TModel, TUser}}, Action{IEndpointConventionBuilder})" />
		///     , but returns the newly created <see cref="RestModelOptionsBuilder{TModel, TUser}" />, rather than the one used to
		///     create the route.
		/// </remarks>
		public RestModelOptionsBuilder<TModel, TUser> FlatMap(Action<IEndpointConventionBuilder>? routeOptionsHandler) =>
			this.FlatMap(this.Options.RoutePattern, routeOptionsHandler);

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>The new <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object</returns>
		/// <remarks>
		///     This method works just like
		///     <see cref="MapRoute(string, Action{RestModelOptionsBuilder{TModel, TUser}}, Action{IEndpointConventionBuilder})" />
		///     , but returns the newly created <see cref="RestModelOptionsBuilder{TModel, TUser}" />, rather than the one used to
		///     create the route.
		/// </remarks>
		public RestModelOptionsBuilder<TModel, TUser> FlatMap(
			string? pattern,
			Action<IEndpointConventionBuilder>? routeOptionsHandler) {
			// create a copy of the options and use that for the new route
			RestModelOptions<TModel, TUser> Copy = this.Options.Copy();
			Copy.RoutePattern += RestModelOptionsBuilder<TModel, TUser>.FixRoute(pattern);
			if (routeOptionsHandler != null) Copy.RouteOptionsHandler = routeOptionsHandler;
			RestModelOptionsBuilder<TModel, TUser> Child = this.CreateChild(Copy);
			this.Children.Add(Child);

			return Child;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will not be parsed from the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to be ignored</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Ignore(Expression<Func<TModel, object>> propertyExpression) {
			this.Options.ParserOptions.IgnoredParseProperties.Add(
				RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(propertyExpression));
			return this;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will not be parsed from the request body
		/// </summary>
		/// <param name="property">The property to be ignored</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Ignore(PropertyInfo property) {
			// todo: add to other methods
			if (property.DeclaringType != null && typeof(TModel) != property.DeclaringType
			                                   && !property.DeclaringType.IsSubclassOf(typeof(TModel)))
				throw new OptionsException("Cannot ignore a property that doesn't belong to the model class");
			this.Options.ParserOptions.IgnoredParseProperties.Add(property);
			return this;
		}

		/// <summary>
		///     Ensures that all properties of the <typeparamref name="TModel" /> will not be parsed from the request body
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> IgnoreAll() {
			this.Options.ParserOptions.IgnoredParseProperties.AddRange(typeof(TModel).GetProperties().Where(p => p.CanWrite));
			return this;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to be included</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Include(Expression<Func<TModel, object>> propertyExpression) {
			return this.Include(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(propertyExpression));
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <param name="property">The property to be ignored</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Include(PropertyInfo property) {
			if (property.DeclaringType != null && typeof(TModel) != property.DeclaringType
			                                   && !property.DeclaringType.IsSubclassOf(typeof(TModel)))
				throw new OptionsException("Cannot include a property that doesn't belong to the model class");

			if (this.Options.FormattingOptions.IncludedReturnProperties == null)
				this.Options.FormattingOptions.IncludedReturnProperties = new List<PropertyInfo>();

			this.Options.FormattingOptions.IncludedReturnProperties.Add(property);
			return this;
		}

		/// <summary>
		///     Ensures that all properties of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> IncludeAll() {
			this.Options.FormattingOptions.IncludedReturnProperties = null;
			return this;
		}

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <param name="optionsHandler">The options for this route</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> MapRoute(
			string pattern,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			optionsHandler(this.FlatMap(pattern));
			return this;
		}

		/// <summary>
		///     Maps another route to the same route pattern
		/// </summary>
		/// <param name="optionsHandler">The options for this route</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> MapRoute(
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler) {
			optionsHandler(this.FlatMap());
			return this;
		}

		/// <summary>
		///     Maps another route to the same route pattern
		/// </summary>
		/// <param name="optionsHandler">The options for this route</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> MapRoute(
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder> routeOptionsHandler) {
			optionsHandler(this.FlatMap(routeOptionsHandler));
			return this;
		}

		/// <summary>
		///     Maps a route to a route pattern
		/// </summary>
		/// <param name="pattern">The pattern to match with</param>
		/// <param name="optionsHandler">The options for this route</param>
		/// <param name="routeOptionsHandler">ASP.NET core specific route options</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> MapRoute(
			string pattern,
			Action<RestModelOptionsBuilder<TModel, TUser>> optionsHandler,
			Action<IEndpointConventionBuilder> routeOptionsHandler) {
			optionsHandler(this.FlatMap(pattern, routeOptionsHandler));
			return this;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will not be included in the response body
		/// </summary>
		/// <param name="property">The property to be omitted</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Omit(PropertyInfo property) {
			if (this.Options.FormattingOptions.IncludedReturnProperties == null)
				this.IncludeAll();

			this.Options.FormattingOptions.IncludedReturnProperties?.Remove(property);
			return this;
		}

		/// <summary>
		///     Ensures that a property of the <typeparamref name="TModel" /> will not be included in the response body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to be omitted</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Omit(Expression<Func<TModel, object>> propertyExpression) {
			return this.Omit(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(propertyExpression));
		}

		/// <summary>
		///     Ensures that no properties of the <typeparamref name="TModel" /> will be included in the response body
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> OmitAll() {
			this.Options.FormattingOptions.IncludedReturnProperties = new List<PropertyInfo>();
			return this;
		}

		/// <summary>
		///     Makes a property of the <typeparamref name="TModel" /> optional in the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to make optional</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Optional(Expression<Func<TModel, object>> propertyExpression) {
			return this.Optional(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(propertyExpression));
		}

		/// <summary>
		///     Makes a property of the <typeparamref name="TModel" /> optional in the request body
		/// </summary>
		/// <param name="property">The property to make optional</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Optional(PropertyInfo property) {
			this.Options.ParserOptions.RequiredParseProperties.Remove(property);
			return this;
		}

		/// <summary>
		///     Requires a property of the <typeparamref name="TModel" /> to be present in the request body
		/// </summary>
		/// <param name="propertyExpression">An expression that returns the property to require</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireProperty(
			Expression<Func<TModel, object>> propertyExpression) {
			return this.RequireProperty(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(propertyExpression));
		}

		/// <summary>
		///     Requires all properties of the <typeparamref name="TModel" /> to be present in the request body
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireAllProperties() {
			this.Options.ParserOptions.RequiredParseProperties.AddRange(typeof(TModel).GetProperties().Where(p => p.CanWrite));
			return this;
		}

		/// <summary>
		///     Requires a property of the <typeparamref name="TModel" /> to be present in the request body
		/// </summary>
		/// <param name="property">The property to require</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireProperty(PropertyInfo property) {
			this.Options.ParserOptions.RequiredParseProperties.Add(property);
			return this;
		}

		/// <summary>
		///     Sets whether or not to write the result as a <typeparamref name="TModel" /> instead of as a collection of
		///     <typeparamref name="TModel" /> if there is only a single element in the result array
		/// </summary>
		/// <param name="strip">
		///     A value indicating whether or not to strip the array from a result body if there is only a single
		///     element in the result array
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> StripArrayIfSingleResult(bool strip = true) {
			this.Options.FormattingOptions.StripArrayIfSingleElement = strip;
			return this;
		}

		/// <summary>
		///     Sets the model provider to use for this route
		/// </summary>
		/// <param name="modelProvider">The model provider to use</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> UseModelProvider(IModelProvider<TModel, TUser> modelProvider) {
			this.Options.ModelProvider = modelProvider;
			return this;
		}

		/// <summary>
		///     Sets the operation to use for this route, like create, update, or delete
		/// </summary>
		/// <param name="operation">The operation to use</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> UseOperation(IOperation<TModel, TUser> operation) {
			this.Options.Operation = operation;
			return this;
		}

		/// <summary>
		///     Sets the result writer to use for this route
		/// </summary>
		/// <param name="resultWriter">The result writer to use</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> UseResultWriter(IResultWriter<TModel, TUser> resultWriter) {
			this.Options.ResultWriter = resultWriter;
			return this;
		}

		/// <summary>
		///     Sets the result writer to use for this route
		/// </summary>
		/// <typeparam name="TWriter">The type of result writer to use</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> UseResultWriter<TWriter>()
			where TWriter : IResultWriter<TModel, TUser>, new() {
			this.Options.ResultWriter = new TWriter();
			return this;
		}

		/// <summary>
		///     Resets this <see cref="RestModelOptionsBuilder{TModel, TUser}" />, clearing all lists and resetting all values
		///     except for the route pattern
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Reset() {
			this.ClearAuthProviders();
			this.ClearBodyParsers();
			this.ClearConditions();
			this.ClearExceptionHandlers();
			this.ClearFilters();
			this.ClearOperation();
			this.ClearRequestMethods();
			this.ClearResultWriter();
			return this;
		}

		/// <summary>
		///		Creates a child instance of the <see cref="RestModelOptionsBuilder{TModel, TUser}"/> type sharing the given base options. When overriden in a derived class, this method can be used to ensure that the entire tree of <see cref="RestModelOptionsBuilder{TModel, TUser}"/> objects share the same derived type
		/// </summary>
		/// <param name="baseOptions">The base options for the new instance</param>
		/// <returns>The new <see cref="RestModelOptionsBuilder{TModel, TUser}"/> instance</returns>
		public virtual RestModelOptionsBuilder<TModel, TUser> CreateChild(RestModelOptions<TModel, TUser>? baseOptions) {
			return new RestModelOptionsBuilder<TModel, TUser>(baseOptions);
		}

		/// <summary>
		///     Builds these options and all of this builder's child options. Not intended for application use.
		/// </summary>
		/// <returns>
		///     A list of all of the options created by this builder, including its own, in the order they were added, keyed
		///     to the route it's for
		/// </returns>
		public Dictionary<string, List<RestModelOptions<TModel, TUser>>> BuildAll() {
			// create options
			// this is not the prettiest thing i've ever written
			List<RestModelOptions<TModel, TUser>> MyOptions = new List<RestModelOptions<TModel, TUser>> { this.Options };
			Dictionary<string, List<RestModelOptions<TModel, TUser>>> AllOptions =
				new Dictionary<string, List<RestModelOptions<TModel, TUser>>> {
					                                                              {
						                                                              this.Options.RoutePattern,
						                                                              MyOptions
					                                                              }
				                                                              };
			foreach (RestModelOptionsBuilder<TModel, TUser> Builder in this.Children)
			foreach ((string Key, List<RestModelOptions<TModel, TUser>> OptionsList) in Builder.BuildAll())
				if (AllOptions.ContainsKey(Key)) AllOptions[Key].AddRange(OptionsList);
				else AllOptions.Add(Key, OptionsList);

			return AllOptions;
		}

		/// <summary>
		///     Extracts a property from an expression
		/// </summary>
		/// <param name="propertyExpression">The expression that refers to a property</param>
		/// <typeparam name="TProperty">The type of the property to extract</typeparam>
		/// <returns>The extracted property</returns>
		private static PropertyInfo ExtractProperty<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression) {
			MemberExpression MemberExpression;
			if (propertyExpression.Body is UnaryExpression Body)
				MemberExpression = (MemberExpression)Body.Operand;
			else MemberExpression = (MemberExpression)propertyExpression.Body;

			return (PropertyInfo)MemberExpression.Member;
		}

		/// <summary>
		///     Fixes a route string provided through user input
		/// </summary>
		/// <param name="routeString">The inputted route string</param>
		/// <returns>The route string, ensuring that it ends with a / and starts with a letter</returns>
		private static string FixRoute(string? routeString) {
			if (routeString == null) return "";
			if (routeString.StartsWith("/")) routeString = routeString.Substring(1);
			if (routeString.Length != 0 && !routeString.EndsWith("/")) routeString += "/";
			return routeString;
		}
	}
}