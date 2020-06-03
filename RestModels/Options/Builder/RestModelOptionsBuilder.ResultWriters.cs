// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.ResultWriters.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System.Text.Json;
	using Microsoft.Net.Http.Headers;
	using RestModels.Results;
	using RestModels.Results.Json;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Writes all API outputs using JSON
		/// </summary>
		/// <param name="options">Options for the JSON serializer</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteJson(JsonSerializerOptions options) =>
			this.UseResultWriter(new JsonResultWriter<TModel>(options));

		/// <summary>
		///     Writes all API outputs using JSON
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteJson() => this.WriteJson(null);

		/// <summary>
		///     Writes API outputs using JSON or XML depending on the Accept header, the "format" query parameter, then the
		///     Content-Type header, defaulting to JSON.
		/// </summary>
		/// <param name="options">Options for the JSON serializer</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteJsonOrXml(JsonSerializerOptions options = null) {
			string[] MimeTypes = { "application/json", "application/xml" };
			JsonResultWriter<TModel> Json = new JsonResultWriter<TModel>(options);
			XmlResultWriter<TModel> Xml = new XmlResultWriter<TModel>();

			HeaderDependentResultWriter<TModel, TUser> ContentTypeWriter = new HeaderDependentResultWriter<TModel, TUser>(HeaderNames.ContentType, MimeTypes, new IResultWriter<TModel, TUser>[] { Json, Xml }, 0);
			QueryDependentResultWriter<TModel, TUser> QueryDependentResultWriter = new QueryDependentResultWriter<TModel, TUser>("format", new[] {"json", "xml"}, new IResultWriter<TModel, TUser>[] {Json, Xml, ContentTypeWriter }, 2);
			AcceptDependentResultWriter<TModel, TUser> AcceptDependentResultWriter = new AcceptDependentResultWriter<TModel, TUser>(MimeTypes, new IResultWriter<TModel, TUser>[] { Json, Xml, QueryDependentResultWriter}, 2);
			return this.UseResultWriter(AcceptDependentResultWriter);
		}


		/// <summary>
		///     Writes all API outputs using XML
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteXml() => this.UseResultWriter<XmlResultWriter<TModel>>();

		/// <summary>
		///		Writes the number of affected elements in all API outputs
		/// </summary>
		/// <param name="templateString">The template string. All instances of "{0}" will be replaced with the number of affected elements</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteNumberAffected(string templateString = "{0}") =>
			this.UseResultWriter(new NumberAffectedResultWriter<TModel>(templateString));

		/// <summary>
		///		Writes a string for all API outputs
		/// </summary>
		/// <param name="str">The string to return</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> WriteString(string str) =>
			this.UseResultWriter(new StringResultWriter<TModel>(str));
	}
}