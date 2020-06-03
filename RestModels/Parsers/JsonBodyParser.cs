using System;
using System.Collections.Generic;
using System.Text;

namespace RestModels.Parsers {
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Text.Json;
	using System.Text.Json.Serialization;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Net.Http.Headers;
	using RestModels.Exceptions;
	using RestModels.Options;

	/// <summary>
	///		Body parser that accepts JSON request bodies
	/// </summary>
	/// <typeparam name="TModel">The type of model to parse</typeparam>
	public class JsonBodyParser<TModel> : IBodyParser<TModel> where TModel : class {
		/// <summary>
		///		A map of a property's serialized name to its info
		/// </summary>
		private readonly Dictionary<string, PropertyInfo> PropertyMap;

		/// <summary>
		///		Options for the JSON deserializer
		/// </summary>
		private JsonSerializerOptions SerializationOptions;

		/// <summary>
		///		Initializes a new instance of the <see cref="JsonBodyParser{TModel}"/> class
		/// </summary>
		public JsonBodyParser()
			: this(null) { }

		/// <summary>
		///		Initializes a new instance of the <see cref="JsonBodyParser{TModel}"/> class
		/// </summary>
		/// <param name="serializerOptions">Options for the JSON serializer</param>
		public JsonBodyParser(JsonSerializerOptions serializerOptions) {
			this.SerializationOptions = serializerOptions ?? new JsonSerializerOptions();
			this.PropertyMap = this.CreatePropertyMap();
		}

		/// <summary>
		///		Gets whether or not the request body can be parsed by this <see cref="IBodyParser{TModel}"/>
		/// </summary>
		/// <param name="context">The context for the HTTP request</param>
		/// <returns><c>true</c> if the request body can be parsed by this <see cref="IBodyParser{TModel}"/>, <c>false</c> otherwise</returns>
		public async Task<bool> CanParse(HttpContext context) => context.Request.ContentType == "application/json";

		/// <summary>
		///     Parses a request body
		/// </summary>
		/// <param name="body">The data of the request body</param>
		/// <param name="options">Options for the parser</param>
		/// <param name="context">The context for the HTTP request</param>
		/// <returns>The parsed object</returns>
		public async Task<ParseResult<TModel>[]> Parse(byte[] body, ParserOptions options, HttpContext context) {
			using JsonDocument Document = JsonDocument.Parse(body);
			JsonElement Root = Document.RootElement;
			if (Root.ValueKind != JsonValueKind.Array) return new[] { this.ParseModel(Root, options) };
			
			if (!options.ParseArrays) throw new ParsingFailedException("This parser does not accept arrays");
			return Root.EnumerateArray().Select(e => this.ParseModel(e, options)).ToArray();
		}

		private ParseResult<TModel> ParseModel(JsonElement model, ParserOptions options) {
			if (model.ValueKind != JsonValueKind.Object)
				throw new ParsingFailedException($"Json parser expected object but got {model.ValueKind}");

			TModel Model = Activator.CreateInstance<TModel>()!;
			Dictionary<PropertyInfo, Func<object>> Values =
				new Dictionary<PropertyInfo, Func<object>>(options.DefaultPropertyValues);

			List<PropertyInfo> PresentProperties = new List<PropertyInfo>();
			List<PropertyInfo> RequiredLeft = new List<PropertyInfo>(options.RequiredParseProperties);

			foreach (JsonProperty Property in model.EnumerateObject()) {
				// check
				// default values check
				// ignored values check 
				// required values check
				PropertyInfo? Matching = this.PropertyMap.ContainsKey(Property.Name) ? this.PropertyMap[Property.Name] : null;
				if (Matching == null) continue; // todo: option to throw if extra properties

				if (options.IgnoredParseProperties.Any(p => p.Name == Matching.Name)) continue;

				RequiredLeft.RemoveAll(p => p.Name == Matching.Name);
				PresentProperties.Add(Matching);
				
				// if the property has a json converter, use that
				Type? ConverterType = Matching.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType;
				JsonConverter? Converter = (JsonConverter?)ConverterType?.GetConstructor(Type.EmptyTypes)?.Invoke(null);
				JsonSerializerOptions Options = new JsonSerializerOptions(); // todo actually use the other options
				if (Converter != null) Options.Converters.Add(Converter);

				object Deserialized = JsonSerializer.Deserialize(Property.Value.GetRawText(), Matching.PropertyType, Options);
				Values[Matching] = () => Deserialized;
			}

			if (RequiredLeft.Any())
				throw new ParsingFailedException(
					$"Failed to parse input. Required properties [{String.Join(", ", RequiredLeft.Select(p => p.Name))}] not present.");

			foreach ((PropertyInfo PropertyInfo, Func<object> Value) in Values) {
				PropertyInfo.GetSetMethod().Invoke(Model, new[] { Value() });
			}

			return new ParseResult<TModel>(Model, PresentProperties);
		}

		private Dictionary<string, PropertyInfo> CreatePropertyMap() {
			// todo this is a little hard to read perhaps
			return new Dictionary<string, PropertyInfo>(
				typeof(TModel).GetProperties().Select(
					p => new KeyValuePair<string, PropertyInfo>(
						p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? this.SerializationOptions.PropertyNamingPolicy?.ConvertName(p.Name) ?? p.Name,
						p)));
		}
	}
}
