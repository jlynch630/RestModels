// -----------------------------------------------------------------------
// <copyright file="ParameterRetriever.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.ParameterRetrievers {
	using Microsoft.AspNetCore.Http;

	/// <summary>
	///     Defines a type that can retrieve a request parameter given a request context
	/// </summary>
	public abstract class ParameterRetriever {
		/// <summary>
		///		Initializes a new instance of the <see cref="ParameterRetriever" /> class.
		/// </summary>
		/// <param name="name">The name of the parameter to retrieve</param>
		/// <param name="defaultValue">The default value of the parameter if none is specified</param>
		protected ParameterRetriever(string name, string? defaultValue = null) {
			this.ParameterName = name;
			this.DefaultValue = defaultValue;
		}

		/// <summary>
		///     Gets the default value of the parameter if none is specified
		/// </summary>
		protected string? DefaultValue { get; }

		/// <summary>
		///     Gets the name of the parameter to retrieve
		/// </summary>
		protected string ParameterName { get; }

		/// <summary>
		///     Gets the value of a parameter for the given request
		/// </summary>
		/// <param name="request">The HTTP request context</param>
		/// <returns>The retrieved parameter value</returns>
		public abstract string? GetValue(HttpRequest request);
	}
}