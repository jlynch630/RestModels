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

	using RestModels.Options;

	/// <summary>
	///     A result writer that simply writes a static string
	/// </summary>
	public class StringResultWriter : IResultWriter<object> {
		/// <summary>
		///     The text to respond with
		/// </summary>
		private readonly string ResponseText;

		/// <summary>
		///     Initializes a new instance of the <see cref="StringResultWriter" /> class.
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
		/// <param name="context">The current request context</param>
		/// <param name="data">The dataset to format</param>
		/// <param name="user">The current authenticated user context</param>
		/// <param name="options">Options for formatting the result</param>
		/// <returns>When the result has been sent</returns>
		public async Task WriteResultAsync(
			HttpContext context,
			IEnumerable<object> data,
			object user,
			FormattingOptions options) {
			context.Response.ContentType = "text/plain";
			await context.Response.WriteAsync(this.ResponseText);
		}
	}
}