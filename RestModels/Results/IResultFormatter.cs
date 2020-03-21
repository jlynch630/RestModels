// -----------------------------------------------------------------------
// <copyright file="IResultFormatter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using RestModels.Options;

	/// <summary>
	///     Formatter for API results
	/// </summary>
	/// <typeparam name="TModel">The type of model to format</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public interface IResultFormatter<in TModel, in TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Formats the API result
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="outStream">
		///     The stream to write out to, equal to the <see cref="HttpResponse.Body" /> of the
		///     <paramref name="context" />
		/// </param>
		/// <param name="data">The dataset to format</param>
		/// <param name="user">The current authenticated user context</param>
		/// <param name="options">Options for formatting the result</param>
		/// <returns>When the result has been sent</returns>
		Task FormatResultAsync(HttpContext context, Stream outStream, IEnumerable<TModel> data, TUser user, FormattingOptions options);
	}
}