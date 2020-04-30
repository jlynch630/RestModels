// -----------------------------------------------------------------------
// <copyright file="HeaderRetriever.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.ParameterRetrievers {
	using Microsoft.AspNetCore.Http;

	/// <summary>
	///     Parameter retriever that retrieves a header by name
	/// </summary>
	public class HeaderRetriever : ParameterRetriever {
		/// <summary>
		///     Initializes a new instance of the <see cref="HeaderRetriever" /> class.
		/// </summary>
		/// <param name="name">The name of the header to retrieve</param>
		/// <param name="defaultValue">The default value of the header if none is specified</param>
		public HeaderRetriever(string name, string? defaultValue = null)
			: base(name, defaultValue) { }

		/// <summary>
		///     Gets the value of the header for the given request
		/// </summary>
		/// <param name="request">The HTTP request context</param>
		/// <returns>The retrieved header value</returns>
		public override string? GetValue(HttpRequest request) =>
			request.Headers.ContainsKey(this.ParameterName)
				? request.Headers[this.ParameterName].ToString()
				: this.DefaultValue;
	}
}