using System;
using System.Collections.Generic;
using System.Text;

namespace RestModels.Parsers {
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore.Metadata.Internal;
	using Microsoft.Net.Http.Headers;
	using RestModels.Exceptions;
	using RestModels.Options;

	/// <summary>
	///		Body parser that accepts JSON request bodies
	/// </summary>
	/// <typeparam name="TModel">The type of model to parse</typeparam>
	public class JsonBodyParser<TModel> : IBodyParser<TModel> where TModel : class {
		private readonly Dictionary<string, PropertyInfo> PropertyMap;

		/// <summary>
		///		Initializes a new instance of the <see cref="JsonBodyParser{TModel}"/> class
		/// </summary>
		public JsonBodyParser() {
			this.PropertyMap = this.CreatePropertyMap();
		}

		/// <summary>
		///     Parses a request body
		/// </summary>
		/// <param name="body">The data of the request body</param>
		/// <param name="options">Options for the parser</param>
		/// <param name="requestContext">The context for the HTTP request</param>
		/// <returns>The parsed object</returns>
		public TModel[] Parse(byte[] body, ParserOptions options, HttpRequest requestContext) {
			if (requestContext.Headers[HeaderNames.ContentType] != "application/json")
				throw new InvalidParserException("Wrong content-type specified for JSON body");
			
			using JsonDocument Document = JsonDocument.Parse(body);
			JsonElement Root = Document.RootElement;
			if (Root.ValueKind != JsonValueKind.Array) return new[] { this.ParseModel(Root, options) };
			
			if (options.ParseArrays) throw new ParsingFailedException("This parser does not accept arrays");
			return Root.EnumerateArray().Select(e => this.ParseModel(e, options)).ToArray();
		}

		private TModel ParseModel(JsonElement model, ParserOptions options) {
			if (model.ValueKind != JsonValueKind.Object)
				throw new ParsingFailedException($"Json parser expected object but got {model.ValueKind}");

			TModel Model = (TModel)typeof(TModel).GetConstructor(Type.EmptyTypes)?.Invoke(null);
			Dictionary<PropertyInfo, object> Values =
				new Dictionary<PropertyInfo, object>(options.DefaultPropertyValues);
			List<PropertyInfo> RequiredLeft = new List<PropertyInfo>(options.RequiredParseProperties);

			foreach (JsonProperty Property in model.EnumerateObject()) {
				// check
				// default values check
				// ignored values check 
				// required values check
				PropertyInfo Matching = this.PropertyMap.ContainsKey(Property.Name) ? this.PropertyMap[Property.Name] : null;
				if (Matching == null) continue; // todo: option to throw if extra properties

				if (options.IgnoredParseProperties.Any(p => p.Name == Matching.Name)) continue;

				RequiredLeft.RemoveAll(p => p.Name == Matching.Name);

				Values[Matching] = JsonSerializer.Deserialize(Property.Value.GetRawText(), Matching.PropertyType);
			}

			foreach ((PropertyInfo PropertyInfo, object Value) in Values) {
				PropertyInfo.GetSetMethod().Invoke(Model, new[] { Value });
			}

			return Model;
		}

		private Dictionary<string, PropertyInfo> CreatePropertyMap() {
			// todo this is a little hard to read perhaps
			return new Dictionary<string, PropertyInfo>(
				typeof(TModel).GetProperties().Select(
					p => new KeyValuePair<string, PropertyInfo>(
						p.GetCustomAttribute<JsonPropertyNameAttribute>(false)?.Name ?? p.Name,
						p)));
		}
	}
}
