// -----------------------------------------------------------------------
// <copyright file="IBodyParser.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Parsers {
	using Microsoft.AspNetCore.Http;

	using RestModels.Options;

	/// <summary>
	///     Parser for a request body
	/// </summary>
	/// <typeparam name="TModel">The type to parse to</typeparam>
	public interface IBodyParser<out TModel> {
		/// <summary>
		///     Parses a request body
		/// </summary>
		/// <param name="body">The data of the request body</param>
		/// <param name="options">Options for the parser</param>
		/// <param name="requestContext">The context for the HTTP request</param>
		/// <returns>The parsed object</returns>
		TModel Parse(byte[] body, ParserOptions options, HttpRequest requestContext);
	}
}