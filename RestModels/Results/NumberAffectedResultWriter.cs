// -----------------------------------------------------------------------
// <copyright file="NumberAffectedResultWriter.cs" company="John Lynch">
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

	using RestModels.Context;
	using RestModels.Options;

	/// <summary>
	///     A result writer that simply writes the number of models affected by the request
	/// </summary>
	/// <typeparam name="TModel">The type of model to format</typeparam>
	public class NumberAffectedResultWriter<TModel> : IResultWriter<TModel>
		where TModel : class {
		/// <summary>
		///		The text to use to format the response. All instances of "{0}" will be replaced with the
		///		number of models affected by the request
		/// </summary>
		private readonly string TemplateText;

		/// <summary>Initializes a new instance of the <see cref="NumberAffectedResultWriter{TModel}" /> class.</summary>
		/// <param name="templateText">
		///     The text to use to format the response. All instances of "{0}" will be replaced with the
		///     number of models affected by the request
		/// </param>
		public NumberAffectedResultWriter(string templateText = "{0}") => this.TemplateText = templateText;

		/// <summary>
		///		Gets whether or not this <see cref="IResultWriter{TModel, TUser}"/> can write a result for the given request
		/// </summary>
		/// <param name="request">The request to test if a result can be written for it</param>
		/// <returns><see langword="true"/></returns>
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
			await context.HttpResponse.WriteAsync(String.Format(this.TemplateText, data?.Count() ?? 0));
		}
	}
}