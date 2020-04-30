// -----------------------------------------------------------------------
// <copyright file="QueryParameterRetriever.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.ParameterRetrievers {
	using Microsoft.AspNetCore.Http;

	/// <summary>
	///     Parameter retriever that retrieves a query parameter by name
	/// </summary>
	public class QueryParameterRetriever : ParameterRetriever {
		/// <summary>
		///     Initializes a new instance of the <see cref="QueryParameterRetriever" /> class.
		/// </summary>
		/// <param name="name">The name of the query parameter to retrieve</param>
		/// <param name="defaultValue">The default value of the parameter if none is specified</param>
		public QueryParameterRetriever(string name, string? defaultValue = null)
			: base(name, defaultValue) { }

		/// <summary>
		///     Gets the value of the query parameter for the given request
		/// </summary>
		/// <param name="request">The HTTP request context</param>
		/// <returns>The retrieved parameter value</returns>
		public override string? GetValue(HttpRequest request) =>
			request.Query.ContainsKey(this.ParameterName)
				? request.Query[this.ParameterName].ToString()
				: this.DefaultValue;
	}
}