﻿// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.Conditions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Conditions;
	using RestModels.Parsers;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Require(
			Func<HttpContext, IQueryable<TModel>, ParseResult<TModel>[], TUser, bool> condition) {
			this.RequireAsync(async (c, d, p, u) => condition(c, d, p, u));
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Require(Func<IQueryable<TModel>, bool> condition) {
			this.Require((c, d, p, u) => condition(d));
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the parsed body meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the parsed body meets a condition</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireInput(Func<ParseResult<TModel>[], bool> condition) {
			this.Require((c, d, p, u) => condition(p));
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the parsed body meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the parsed body meets a condition</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireInputAsync(Func<ParseResult<TModel>[], Task<bool>> condition) {
			this.RequireAsync((c, d, p, u) => condition(p));
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Require(Func<HttpContext, IQueryable<TModel>, bool> condition) {
			this.Require((c, d, p, u) => condition(c, d));
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireAsync(
			Func<HttpContext, IQueryable<TModel>, ParseResult<TModel>[], TUser, Task<bool>> condition) {
			this.AddCondition(new DelegateCondition<TModel, TUser>(condition));
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireAsync(Func<IQueryable<TModel>, Task<bool>> condition) {
			this.RequireAsync((c, d, p, u) => condition(d));
			return this;
		}

		/// <summary>
		///     Adds a requirement to this route that will ensure the request meets the given condition
		/// </summary>
		/// <param name="condition">The delegate to use to determine if the request meets a condition</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireAsync(Func<HttpContext, IQueryable<TModel>, Task<bool>> condition) {
			this.RequireAsync((c, d, p, u) => condition(c, d));
			return this;
		}

		/// <summary>
		///     Ensures that there are at least a certain number of elements in the dataset
		/// </summary>
		/// <param name="count">The number of elements that should be in the dataset at a minimum</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireAtLeast(int count) => this.Require(d => d.Count() >= count);

		/// <summary>
		///     Ensures that there are exactly a certain number of elements in the dataset
		/// </summary>
		/// <param name="count">The number of elements that should be in the dataset</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireExactly(int count) =>
			this.Require(d => d.Count() == count);


		/// <summary>
		///     Ensures that there are at least a certain number of elements in the parsed body
		/// </summary>
		/// <param name="count">The number of elements that should be in the dataset at a minimum</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireInputHasAtLeast(int count) => this.RequireInput(p => p.Length >= count);

		/// <summary>
		///     Ensures that there are exactly a certain number of elements in the parsed dataset
		/// </summary>
		/// <param name="count">The number of elements that should be in the dataset</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireInputHasExactly(int count) =>
			this.RequireInput(p => p.Length == count);

		/// <summary>
		///     Ensures that there is exactly one element in the dataset
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireExactlyOne() => this.RequireExactly(1);

		/// <summary>
		///     Ensures that there is at least one element in the dataset
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireNonEmpty() => this.RequireAtLeast(1);

		/// <summary>
		///     Ensures that there is exactly one element in the parsed body
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireExactlyOneInput() => this.RequireInputHasExactly(1);

		/// <summary>
		///     Ensures that there is at least one element in the parsed body
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> RequireNonEmptyInput() => this.RequireInputHasAtLeast(1);
	}
}