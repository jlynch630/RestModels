// -----------------------------------------------------------------------
// <copyright file="ParseResult.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Parsers {
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	///     The result of parsing a request body
	/// </summary>
	/// <typeparam name="TModel">The type of model being parsed</typeparam>
	public class ParseResult<TModel> {
		/// <summary>Initializes a new instance of the <see cref="ParseResult{TModel}"/> class.</summary>
		/// <param name="parsedModel">The model that was parsed</param>
		/// <param name="presentProperties">The properties that were present on this model in the request body</param>
		public ParseResult(TModel parsedModel, IEnumerable<PropertyInfo> presentProperties) {
			this.ParsedModel = parsedModel;
			this.PresentProperties = presentProperties.ToArray();
		}

		/// <summary>
		///     Gets or sets the model that was parsed
		/// </summary>
		public TModel ParsedModel { get; set; }

		/// <summary>
		///     Gets or sets a list of the properties that were present on this model in the request body
		/// </summary>
		public PropertyInfo[] PresentProperties { get; set; }

		/// <summary>
		///		Implicitly casts this <see cref="ParseResult{TModel}"/> to the <typeparamref cref="TModel"/> it contains.
		/// </summary>
		/// <param name="result">The <see cref="ParseResult{TModel}"/> to cast</param>
		public static implicit operator TModel(ParseResult<TModel> result) => result.ParsedModel;
	}
}