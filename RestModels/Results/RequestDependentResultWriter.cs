// -----------------------------------------------------------------------
// <copyright file="RequestDependentResultWriter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Options;

	/// <summary>
	///     A result writer that chooses how to format the result based on the request
	/// </summary>
	/// <typeparam name="TModel">The type of model to format</typeparam>
	/// <typeparam name="TUser">The type of user context</typeparam>
	/// <remarks>
	///     Though perhaps this class is a little confusing at first, it helps to know that this class forms the basis for the <see cref="QueryDependentResultWriter{TModel, TUser}"/>, <see cref="HeaderDependentResultWriter{TModel, TUser}"/>, and like classes.
	/// </remarks>
	public abstract class RequestDependentResultWriter<TModel, TUser> : IResultWriter<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     True if the parameter values are case sensitive, false otherwise
		/// </summary>
		private readonly bool CaseSensitive;

		/// <summary>
		///     The index of the default result writer to use, or -1 if an error should be thrown if no values match.
		/// </summary>
		private readonly int DefaultIndex;

		/// <summary>
		///     The values of the parameters that
		/// </summary>
		private readonly string[] ParameterValues;

		/// <summary>
		///     The result writers to use, indexed in the same order as <see cref="ParameterValues" />
		/// </summary>
		private readonly IResultWriter<TModel, TUser>[] ResultWriters;

		/// <summary>
		///     Initializes a new instance of the <see cref="RequestDependentResultWriter{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="values">The values of the request property that should determine which result writer to use</param>
		/// <param name="writers">The result writers to use, indexed in the same order as <paramref name="values" /></param>
		/// <param name="defaultIndex">
		///     The index of the default result writer to use, or -1 if an error should be thrown if no
		///     values match
		/// </param>
		/// <param name="caseSensitive"><code>true</code> if the parameter values are case sensitive, <code>false</code> otherwise</param>
		protected RequestDependentResultWriter(
			string[] values,
			IResultWriter<TModel, TUser>[] writers,
			int defaultIndex = -1,
			bool caseSensitive = false) {
			this.ParameterValues = caseSensitive ? values : values.Select(v => v.ToLower()).ToArray();
			this.ResultWriters = writers;
			this.CaseSensitive = caseSensitive;
			this.DefaultIndex = defaultIndex;
		}

		/// <summary>
		///     Gets whether or not this <see cref="IResultWriter{TModel, TUser}" /> can write a result for the given request
		/// </summary>
		/// <param name="request">The request to test if a result can be written for it</param>
		/// <returns><code>true</code> if a result can be written for <paramref name="request" />, <code>false</code> otherwise</returns>
		public virtual async Task<bool> CanWriteAsync(HttpRequest request) {
			if (this.DefaultIndex >= 0) return true;

			int Index = this.GetWriterIndex(request); // todo: calling twice?
			return Index != -1;
		}

		/// <summary>
		///     Formats the API result
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="data">The dataset to format</param>
		/// <param name="user">The current authenticated user context</param>
		/// <param name="options">Options for formatting the result</param>
		/// <returns>When the result has been sent</returns>
		public virtual async Task WriteResultAsync(
			HttpContext context,
			IEnumerable<TModel> data,
			TUser user,
			FormattingOptions options) {
			int Index = this.GetWriterIndex(context.Request);

			// if default index is also -1, we wont get here because of CanWriteAsync
			if (Index == -1) Index = this.DefaultIndex;

			await this.ResultWriters[Index].WriteResultAsync(context, data, user, options);
		}

		/// <summary>
		///     Gets the value of the request parameter this <see cref="RequestDependentResultWriter{TModel, TUser}" /> switches on
		/// </summary>
		/// <param name="request">The request context to use to get the parameter value</param>
		/// <returns>The value of the request parameter to switch on</returns>
		protected abstract string GetRequestParameterValue(HttpRequest request);

		/// <summary>
		///     Gets the result writer index for the given request
		/// </summary>
		/// <param name="request">The request context to get the result writer index for</param>
		/// <returns>The index of the result writer to use for this request, or -1 if none match</returns>
		private int GetWriterIndex(HttpRequest request) {
			string Value = this.GetRequestParameterValue(request) ?? "";
			return Array.IndexOf(this.ParameterValues, this.CaseSensitive ? Value : Value.ToLower());
		}
	}
}