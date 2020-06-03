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
	using System.Threading.Tasks;

	using RestModels.Actions;
	using RestModels.Context;
	using RestModels.Responses.Attributes;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds a post-operation action to this route that will run before the operation is executed
		/// </summary>
		/// <param name="handler">
		///     The action to run after the operation, which will be passed the current API context and operation
		///     result
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> PostOp(
			Action<IApiContext<TModel, TUser>, IEnumerable<TModel>> handler) {
			return this.PostOpAsync(async (c, d) => handler(c, d));
		}

		/// <summary>
		///     Adds a post-operation action to this route that will run before the operation is executed
		/// </summary>
		/// <param name="handler">
		///     The action to run after the operation, which will be passed the current API context and operation
		///     result
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> PostOpAsync(
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
		public RestModelOptionsBuilder<TModel, TUser> PreOp(
			Action<IApiContext<TModel, TUser>, IQueryable<TModel>> handler) {
			return this.PreOpAsync(async (c, d) => handler(c, d));
		}

		/// <summary>
		///     Adds a pre-operation action to this route that will run before the operation is executed
		/// </summary>
		/// <param name="handler">
		///     The action to run before the operation, which will be passed the current API context and model
		///     dataset
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> PreOpAsync(
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
			return this.PostOpAsync(async (c, d) => {
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
			return this.PostOpAsync(async (c, d) => {
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
			return this.PostOpAsync(async (c, d) => {
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
	}
}