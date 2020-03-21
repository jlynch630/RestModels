// -----------------------------------------------------------------------
// <copyright file="JsonResultWriter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Text.Json;
	using System.Text.Json.Serialization;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.Net.Http.Headers;

	using RestModels.Options;

	/// <summary>
	///     Result writer that formats data in a JSON format using System.Text.Json
	/// </summary>
	/// <typeparam name="TModel">The type of serialized model</typeparam>
	public class JsonResultWriter<TModel> : IResultWriter<TModel>
		where TModel : class {
		/// <summary>
		///     Formats the API result
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="data">The dataset to format</param>
		/// <param name="user">The current authenticated user context</param>
		/// <param name="options">Options for formatting the result</param>
		/// <returns>When the result has been sent</returns>
		public async Task WriteResultAsync(
			HttpContext context,
			IEnumerable<TModel> data,
			object user,
			FormattingOptions options) {
			// set content type first, then actually write the json
			context.Response.Headers[HeaderNames.ContentType] = "application/json";

			if (data == null) {
				await context.Response.WriteAsync("null");
				return;
			}

			TModel[] FullDataset = data.ToArray();

			bool ReturnObject = options.StripArrayIfSingleElement && FullDataset.Length == 1;
			byte[] ResultString;
			if (options.IncludedReturnProperties == null) 
				ResultString = ReturnObject
				                      ? JsonSerializer.SerializeToUtf8Bytes(FullDataset[0])
				                      : JsonSerializer.SerializeToUtf8Bytes(FullDataset);
			else {
				ResultString = ReturnObject
					               ? JsonSerializer.SerializeToUtf8Bytes(this.Transform(FullDataset[0], options.IncludedReturnProperties))
					               : JsonSerializer.SerializeToUtf8Bytes(FullDataset.Select(t => this.Transform(t, options.IncludedReturnProperties)));
			}

			await context.Response.Body.WriteAsync(ResultString);
		}

		/// <summary>
		///		Transforms the input model and returns an object with only the included properties
		/// </summary>
		/// <param name="input">The input model</param>
		/// <param name="included">The properties of <typeparamref name="TModel"/> that should be included</param>
		/// <returns>The transformed model</returns>
		private Dictionary<string, object> Transform(TModel input, List<PropertyInfo> included) {
			Dictionary<string, object> Transformed = new Dictionary<string, object>();
			foreach (PropertyInfo Property in included)
				Transformed[Property.Name] = Property.GetGetMethod().Invoke(input, null);
			return Transformed;
		}
	}
}