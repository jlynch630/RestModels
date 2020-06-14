// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.Actions.cs" company="John Lynch">
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
    using System.Threading.Tasks;

	using RestModels.Actions;
	using RestModels.Context;
    using RestModels.Exceptions;
    using RestModels.Filters;
    using RestModels.ParameterRetrievers;
    using RestModels.Parsers;
    using RestModels.Responses.Attributes;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds a post-operation action to this route that will run after the operation is executed
		/// </summary>
		/// <param name="handler">
		///     The action to run after the operation, which will be passed the current API context and operation
		///     result
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>
		///		This action will be run after every operation and can be used to log data, notify other parts of the app, or modify the HTTP response
		///		<para>
		///			Note that if you would like to omit/include properties from the API response or set response parameters, use the <see cref="RestModelOptionsBuilder{TModel, TUser}.Omit(Expression{Func{TModel, object}})" />, <see cref="RestModelOptionsBuilder{TModel, TUser}.Include(Expression{Func{TModel, object}})" />, and <see cref="RestModelOptionsBuilder{TModel, TUser}.WriteResponseValue(string, object)" /> methods respectively.
		///		</para>
		/// </remarks>
		public RestModelOptionsBuilder<TModel, TUser> After(
			Action<IApiContext<TModel, TUser>, IEnumerable<TModel>> handler) {
			return this.AfterAsync(async (c, d) => handler(c, d));
		}

		/// <summary>
		///     Adds a post-operation action to this route that will run after the operation is executed
		/// </summary>
		/// <param name="handler">
		///     The action to run after the operation, which will be passed the current API context and operation
		///     result
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		/// <remarks>
		///		This action will be run after every operation and can be used to log data, notify other parts of the app, or modify the HTTP response
		///		<para>
		///			Note that if you would like to omit/include properties from the API response or set response parameters, use the <see cref="RestModelOptionsBuilder{TModel, TUser}.Omit(Expression{Func{TModel, object}})" />, <see cref="RestModelOptionsBuilder{TModel, TUser}.Include(Expression{Func{TModel, object}})" />, and <see cref="RestModelOptionsBuilder{TModel, TUser}.WriteResponseValue(string, object)" /> methods respectively.
		///		</para>
		/// </remarks>
		public RestModelOptionsBuilder<TModel, TUser> AfterAsync(
			Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, Task> handler) =>
			this.AddPostOpAction(new DelegatePostOpAction<TModel, TUser>(handler));

		/// <summary>
		///     Adds a pre-operation action to this route that will run before the operation is executed
		/// </summary>
		/// <param name="handler">
		///     The action to run before the operation, which will be passed the current API context and model
		///     dataset
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Before(
			Action<IApiContext<TModel, TUser>, IQueryable<TModel>> handler) {
			return this.BeforeAsync(async (c, d) => handler(c, d));
		}

		/// <summary>
		///     Adds a pre-operation action to this route that will run before the operation is executed
		/// </summary>
		/// <param name="handler">
		///     The action to run before the operation, which will be passed the current API context and model
		///     dataset
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> BeforeAsync(
			Func<IApiContext<TModel, TUser>, IQueryable<TModel>, Task> handler) =>
			this.AddPreOpAction(new DelegatePreOpAction<TModel, TUser>(handler));

		/// <summary>
		///     Sets a response value for this request using an asynchronous handler. The response value to set is determined by the given attribute type
		/// </summary>
		/// <typeparam name="TAttribute">The type of attribute on the property to set the response value of</typeparam>
		/// <param name="handler">The delegate which will return what to set the value of the property defined by the <typeparamref name="TAttribute"/> attribute</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteResponseValueAsync<TAttribute>(
			Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, Task<object>> handler) where TAttribute : Attribute {
			return this.AfterAsync(async (c, d) => {
				if (c.Response == null) return;
				object Output = await handler(c, d);
				if (Output is string StringOutput) c.Response.SetString<TAttribute>(StringOutput);
				else c.Response.Set<TAttribute>(Output);
			});
		}

		/// <summary>
		///     Sets a response value for this request using a synchronous handler. The response value to set is determined by the given attribute type
		/// </summary>
		/// <typeparam name="TAttribute">The type of attribute on the property to set the response value of</typeparam>
		/// <param name="handler">The delegate which will return what to set the value of the property defined by the <typeparamref name="TAttribute"/> attribute</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteResponseValue<TAttribute>(
			Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, object> handler) where TAttribute : Attribute {
			return this.WriteResponseValueAsync<TAttribute>(async (c, d) => handler(c, d));
		}

		/// <summary>
		///     Sets a response value for this request to a predefined value. The response value to set is determined by the given attribute type
		/// </summary>
		/// <typeparam name="TAttribute">The type of attribute on the property to set the response value of</typeparam>
		/// <param name="value">The value to set on the property defined by the <typeparamref name="TAttribute"/> attribute</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteResponseValue<TAttribute>(object value) where TAttribute : Attribute {
			return this.WriteResponseValue<TAttribute>((c, d) => value);
		}

		/// <summary>
		///     Sets a response value for this request using an asynchronous handler. The response value to set is determined by the given attribute type
		/// </summary>
		/// <param name="name">The name of the response value to set</param>
		/// <param name="handler">The delegate which will return what to set the value of the property defined by <paramref name="name"/></param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteResponseValueAsync(string name, Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, Task<object>> handler) {
			return this.AfterAsync(async (c, d) => {
				if (c.Response == null) return;
				object Output = await handler(c, d);
				if (Output is string StringOutput) c.Response.SetString(name, StringOutput);
				else c.Response.Set(name, Output);
			});
		}

		/// <summary>
		///     Sets a response value for this request using a synchronous handler. The response value to set is determined by the given attribute type
		/// </summary>
		/// <param name="name">The name of the response value to set</param>
		/// <param name="handler">The delegate which will return what to set the value of the property defined by <paramref name="name"/></param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteResponseValue(string name,  Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, object> handler) {
			return this.WriteResponseValueAsync(name, async (c, d) => handler(c, d));
		}

		/// <summary>
		///     Sets a response value for this request to a predefined value. The response value to set is determined by the given attribute type
		/// </summary>
		/// <param name="name">The name of the response value to set</param>
		/// <param name="value">The value to set on the property defined by <paramref name="name"/></param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteResponseValue(string name, object value) {
			return this.WriteResponseValue(name, (c, d) => value);
		}

		/// <summary>
		///     Sets the response value defined by the <see cref="ModelCountAttribute"/> to the size of the resulting dataset after the operation, if any.
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteResponseCount() {
			return this.WriteResponseValue<ModelCountAttribute>((c, d) => d.Count());
		}

		/// <summary>
		///     Sets the response value defined by the <see cref="ApiVersionAttribute"/> to the current API version.
		/// </summary>
		/// <param name="version">The current API version</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteVersionValue(string version) {
			return this.WriteResponseValue<ApiVersionAttribute>(version);
		}

		/// <summary>
		///     Sets the response header X-Api-Version to the current API version.
		/// </summary>
		/// <param name="version">The current API version</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteVersionHeader(string version) {
			return this.SetHeader("X-Api-Version", version);
		}

		/// <summary>
		///     Sets the response value defined by the <see cref="ApiVersionAttribute"/> and the X-Api-Version header to the current API version.
		/// </summary>
		/// <param name="version">The current API version</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteVersion(string version) {
			this.WriteVersionValue(version);
			return this.WriteVersionHeader(version);
		}


		/// <summary>
		///     Sets the response value defined by the <see cref="DeprecationNoticeAttribute"/> to the given deprecation message, or a boilerplate one if none is specified.
		/// </summary>
		/// <param name="message">The deprecation message to display to API consumers</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Deprecate(string? message) {
			return this.WriteResponseValue<DeprecationNoticeAttribute>(message ?? "This API version has been deprecated.");
		}

		/// <summary>
		///     Sets a response header for this request using an asynchronous handler.
		/// </summary>
		/// <param name="name">The name of the header to set</param>
		/// <param name="handler">The delegate which will return the value to set the header to</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetHeaderAsync(string name, Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, Task<string>> handler) {
			return this.AfterAsync(async (c, d) => {
				c.HttpResponse.Headers.Add(name, await handler(c, d));
			});
		}

		/// <summary>
		///     Sets a response header for this request using a synchronous handler.
		/// </summary>
		/// <param name="name">The name of the response value to set</param>
		/// <param name="handler">The delegate which will return the value to set the header to</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetHeader(string name, Func<IApiContext<TModel, TUser>, IEnumerable<TModel>, string> handler) {
			return this.SetHeaderAsync(name, async (c, d) => handler(c, d));
		}

		/// <summary>
		///     Sets a response header for this request to a predefined value.
		/// </summary>
		/// <param name="name">The name of the response value to set</param>
		/// <param name="value">The value to set for the header</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetHeader(string name, string value) {
			return this.SetHeader(name, (c, d) => value);
		}

		/// <summary>
		///     Sets a value on parsed models before the operation occurs.
		/// </summary>
		/// <param name="property">The property to set the value on</param>
		/// <param name="valueGetter">A handler to use to get the value when requested</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SetValueAsync(
			PropertyInfo property,
			Func<IApiContext<TModel, TUser>, Task<object?>> valueGetter) {
				return this.BeforeAsync(async (c, d) => {
					if (c.Parsed == null) throw new ApiException("Cannot set value on route without parsed model");
					foreach (ParseResult<TModel> Model in c.Parsed)
						property.GetSetMethod()?.Invoke(Model.ParsedModel, new[] {await valueGetter(c)});
				});
			}

        /// <summary>
        ///     Sets a value on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="valueGetter">A handler to use to get the value when requested</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValue(
            PropertyInfo property,
            Func<IApiContext<TModel, TUser>, object?> valueGetter)
			=> this.SetValueAsync(property, async (c) => valueGetter(c));

		/// <summary>
        ///     Sets a value on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="valueGetter">A handler to use to get the value when requested</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValue(
            Expression<Func<TModel, object>> property,
            Func<IApiContext<TModel, TUser>, object?> valueGetter)
			=> this.SetValue(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(property), valueGetter);

		/// <summary>
        ///     Sets a value on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="valueGetter">A handler to use to get the value when requested</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValueAsync(
            Expression<Func<TModel, object>> property,
            Func<IApiContext<TModel, TUser>, Task<object?>> valueGetter)
			=> this.SetValueAsync(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(property), valueGetter);

		/// <summary>
        ///     Sets a value on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="valueGetter">A parameter retriever to use to get the value for a request</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValue(
            PropertyInfo property,
            ParameterRetriever retriever) {
			Type PropertyType = property.PropertyType;
			return this.SetValue(property, (c) => ParameterResolver.ParseParameter(retriever.GetValue(c.Request), PropertyType));
		}

		/// <summary>
        ///     Sets a value on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="valueGetter">A parameter retriever to use to get the value for a request</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValue(
            Expression<Func<TModel, object?>> property,
            ParameterRetriever retriever)
			=> this.SetValue(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(property), retriever);

		/// <summary>
        ///     Sets a value obtained from a query parameter on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="parameterName">The name of the query parameter whose value <paramref name="property"/> will be set to</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValueQuery(
            PropertyInfo property,
            string parameterName)
			=> this.SetValue(property, new QueryParameterRetriever(parameterName));

			/// <summary>
        ///     Sets a value obtained from a query parameter on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="parameterName">The name of the query parameter whose value <paramref name="property"/> will be set to</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValueQuery(
            Expression<Func<TModel, object?>> property,
            string parameterName)
			=> this.SetValueQuery(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(property), parameterName);

		/// <summary>
        ///     Sets a value obtained from a route value on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="parameterName">The name of the route value whose value <paramref name="property"/> will be set to</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValueRoute(
            PropertyInfo property,
            string parameterName)
			=> this.SetValue(property, new RouteValueRetriever(parameterName));

		/// <summary>
        ///     Sets a value obtained from a route value on parsed models before the operation occurs.
        /// </summary>
        /// <param name="property">The property to set the value on</param>
        /// <param name="parameterName">The name of the route value whose value <paramref name="property"/> will be set to</param>
        /// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
        public RestModelOptionsBuilder<TModel, TUser> SetValueRoute(
            Expression<Func<TModel, object?>> property,
            string parameterName)
			=> this.SetValueRoute(RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(property), parameterName);
    }
}