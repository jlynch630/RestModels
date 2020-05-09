// -----------------------------------------------------------------------
// <copyright file="JsonResultWriter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Text.Json;
	using System.Text.Json.Serialization;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.Net.Http.Headers;

	using RestModels.Context;
	using RestModels.Options;

	/// <summary>
	///     Result writer that formats data in a JSON format using System.Text.Json
	/// </summary>
	/// <typeparam name="TModel">The type of serialized model</typeparam>
	public class JsonResultWriter<TModel> : IResultWriter<TModel>
		where TModel : class {
		/// <summary>
		///		Options for the JSON serializer
		/// </summary>
		private readonly JsonSerializerOptions Options;

		/// <summary>
		///		Initializes a new instance of the <see cref="JsonResultWriter{TModel}"/> class
		/// </summary>
		/// <param name="options">Options for the JSON serializer</param>
		public JsonResultWriter(JsonSerializerOptions? options = null) => this.Options = options ?? new JsonSerializerOptions();

		/// <summary>
		///		Gets whether or not this <see cref="IResultWriter{TModel, TUser}"/> can write a result for the given request
		/// </summary>
		/// <param name="request">The request to test if a result can be written for it</param>
		/// <returns><code>true</code></returns>
		public async Task<bool> CanWriteAsync(HttpRequest request) => true;

		/// <summary>
		///     Formats the API result
		/// </summary>
		/// <param name="context">The current API context</param>
		/// <param name="data">The dataset to format</param>
		/// <param name="formatOptions">Options for formatting the result</param>
		/// <returns>When the result has been sent</returns>
		public async Task WriteResultAsync(
			IApiContext<TModel, object> context,
			IEnumerable<TModel> data,
			FormattingOptions formatOptions) {
			// set content type first, then actually write the json
			context.HttpResponse.ContentType = "application/json";

			if (data == null) {
				await context.HttpResponse.WriteAsync("null");
				return;
			}

			TModel[] FullDataset = data.ToArray();


			JsonSerializerOptions Options = this.Options;
			if (formatOptions.IncludedReturnProperties != null) Options = this.CreateCustomOptions(formatOptions);

			byte[] ResultString;
			if (context.Response == null) {
				bool ReturnObject = formatOptions.StripArrayIfSingleElement && FullDataset.Length == 1;
				ResultString = ReturnObject
					               ? JsonSerializer.SerializeToUtf8Bytes(FullDataset[0], Options)
					               : JsonSerializer.SerializeToUtf8Bytes(FullDataset, Options);
			}
			else {
				// set the data on the response object
				context.Response.Populate(FullDataset, formatOptions.StripArrayIfSingleElement);

				// and serialize
				ResultString = JsonSerializer.SerializeToUtf8Bytes(context.Response, context.Response.GetType(), Options);
			}

			await context.HttpResponse.Body.WriteAsync(ResultString);
		}

		/// <summary>
		///		Creates a clone of the given serialization options with an additional <see cref="ModelJsonConverter{TModel}"/>
		/// </summary>
		/// <param name="formatOptions">Options containing the included return properties</param>
		/// <returns>The new <see cref="JsonSerializerOptions"/></returns>
		private JsonSerializerOptions CreateCustomOptions(FormattingOptions formatOptions) {
			JsonSerializerOptions NewOptions = new JsonSerializerOptions() {
				AllowTrailingCommas = this.Options.AllowTrailingCommas,
				DefaultBufferSize = this.Options.DefaultBufferSize,
				DictionaryKeyPolicy = this.Options.DictionaryKeyPolicy,
				Encoder = this.Options.Encoder,
				IgnoreNullValues = this.Options.IgnoreNullValues,
				IgnoreReadOnlyProperties = this.Options.IgnoreReadOnlyProperties,
				MaxDepth = 64,//this.Options.MaxDepth,
				PropertyNameCaseInsensitive = this.Options.PropertyNameCaseInsensitive,
				PropertyNamingPolicy = this.Options.PropertyNamingPolicy,
				ReadCommentHandling = this.Options.ReadCommentHandling,
				WriteIndented = this.Options.WriteIndented
			};
			foreach (JsonConverter Existing in this.Options.Converters)
				NewOptions.Converters.Add(Existing);
			NewOptions.Converters.Add(new ModelJsonConverter<TModel>(formatOptions.IncludedReturnProperties));
			return NewOptions;
		}
	}
}