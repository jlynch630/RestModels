// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.BodyParsers.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System.Text.Json;

	using RestModels.Parsers;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Sets this route up to parse JSON request bodies
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ParseJson() => this.ParseJson(null);

		/// <summary>
		///     Sets this route up to parse JSON request bodies
		/// </summary>
		/// <param name="options">The options for the JSON deserializer</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ParseJson(JsonSerializerOptions options) =>
			this.AddBodyParser(new JsonBodyParser<TModel>(options));

		/// <summary>
		///     Sets this route up to parse JSON request bodies as a single object or an array
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ParseJsonArrays() {
			this.AcceptArrays();
			return this.ParseJson();
		}

		/// <summary>
		///     Sets this route up to parse JSON request bodies as a single object or an array
		/// </summary>
		/// <param name="options">The options for the JSON deserializer</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ParseJsonArrays(JsonSerializerOptions options) {
			this.AcceptArrays();
			return this.ParseJson(options);
		}

		/// <summary>
		///     Sets this route up to parse XML request bodies
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ParseXml() => this.AddBodyParser<XmlBodyParser<TModel>>();

		/// <summary>
		///     Sets this route up to parse XML and JSON request bodies
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ParseXmlAndJson() {
			this.ParseJson();
			this.ParseXml();
			return this;
		}

		/// <summary>
		///     Sets this route up to parse XML and JSON request bodies as a single object or an array
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ParseXmlAndJsonArrays() {
			this.AcceptArrays();
			return this.ParseXmlAndJson();
		}

		/// <summary>
		///     Sets this route up to parse XML request bodies as a single object or an array
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> ParseXmlArrays() {
			this.AcceptArrays();
			return this.ParseXml();
		}
	}
}