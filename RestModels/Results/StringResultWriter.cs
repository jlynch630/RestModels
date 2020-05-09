// -----------------------------------------------------------------------
// <copyright file="StringResultWriter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Context;
	using RestModels.Options;

	/// <summary>
	///     A result writer that simply writes a static string
	/// </summary>
	/// <typeparam name="TModel">The type of model managed by the API</typeparam>
	public class StringResultWriter<TModel> : IResultWriter<TModel> where TModel : class {
		/// <summary>
		///     The text to respond with
		/// </summary>
		private readonly string ResponseText;

		/// <summary>
		///     Initializes a new instance of the <see cref="StringResultWriter{TModel}" /> class.
		/// </summary>
		/// <param name="responseText">The text to respond with</param>
		public StringResultWriter(string responseText) => this.ResponseText = responseText;

		/// <summary>
		///     Gets whether or not this <see cref="IResultWriter{TModel, TUser}" /> can write a result for the given request
		/// </summary>
		/// <param name="request">The request to test if a result can be written for it</param>
		/// <returns>
		///     <code>true</code>
		/// </returns>
		public async Task<bool> CanWriteAsync(HttpRequest request) => true;

		/// <summary>
		///     Formats the API result
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="data">The dataset to format</param>
		/// <param name="options">Options for formatting the result</param>
		/// <returns>When the result has been sent</returns>
		public async Task WriteResultAsync(
			IApiContext<TModel, object> context,
			IEnumerable<TModel> data,
			FormattingOptions options) {
			context.HttpResponse.ContentType = "text/plain";
			await context.HttpResponse.WriteAsync(this.ResponseText);
		}
	}
}