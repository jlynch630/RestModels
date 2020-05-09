// -----------------------------------------------------------------------
// <copyright file="ModelJsonConverter.cs" company="John Lynch">
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

	/// <summary>
	///     JSON converter for RestModels API objects
	/// </summary>
	/// <typeparam name="TModel">The type of model to convert</typeparam>
	public class ModelJsonConverter<TModel> : JsonConverter<TModel>
		where TModel : class {
		/// <summary>
		///     The list of properties that should be included when serializing
		/// </summary>
		private readonly List<PropertyInfo> IncludedReturnProperties;

		/// <summary>
		///     Initializes a new instance of the <see cref="ModelJsonConverter{TModel}" /> class.
		/// </summary>
		/// <param name="includedReturnProperties">The list of properties that should be included when serializing the model</param>
		internal ModelJsonConverter(List<PropertyInfo> includedReturnProperties) =>
			this.IncludedReturnProperties = includedReturnProperties;

		/// <summary>
		///     Reads the model from a JSON reader. Not yet implemented
		/// </summary>
		/// <param name="reader">The JSON reader to read the model from</param>
		/// <param name="typeToConvert">The type to convert the read data into</param>
		/// <param name="options">Any options to use when deserializing the JSON</param>
		/// <returns>The deserialized object</returns>
		public override TModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
			throw new NotImplementedException();

		/// <summary>
		///     Writes the given model object to a JSON writer
		/// </summary>
		/// <param name="writer">The JSON writer to write the <paramref name="value" /> to</param>
		/// <param name="value">The value to write</param>
		/// <param name="options">Any options to use when serializing the JSON</param>
		public override void Write(Utf8JsonWriter writer, TModel value, JsonSerializerOptions options) {
			writer.WriteStartObject();

			// there are four attributes we want to support
			// jsonignore -- easy just ignore the object (done!)
			// jsonconvert -- the converter to use, should also check `options`
			// jsonpropertyname -- easy just change the name (done!)
			// jsonextensiondata -- extra properties are written into here. this might be a pain (done!)
			IEnumerable<PropertyInfo> Properties = typeof(TModel).GetProperties()
				.Where(p => p.CanRead && p.GetCustomAttribute<JsonIgnoreAttribute>() == null);
			foreach (PropertyInfo Property in Properties) {
				if (!this.IncludedReturnProperties.Contains(Property)) continue;
				if (Property.GetCustomAttribute<JsonExtensionDataAttribute>() != null) {
					this.WriteExtensionData(Property, writer, value, options);
					continue;
				}

				string Name = Property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
				              ?? options.PropertyNamingPolicy?.ConvertName(Property.Name) ?? Property.Name;
				writer.WritePropertyName(Name);

				ModelJsonProperty<TModel>? JsonProperty = (ModelJsonProperty<TModel>?)Activator.CreateInstance(
					typeof(ModelJsonProperty<,>).MakeGenericType(typeof(TModel), Property.PropertyType), Property);

				if (JsonProperty == null) throw new JsonException();
				JsonProperty.Write(writer, value, options);
			}

			writer.WriteEndObject();
		}

		/// <summary>
		///     Writes any extra values in a property on <typeparamref name="TModel" /> marked with the
		///     <see cref="JsonExtensionDataAttribute" /> to the given JSON writer
		/// </summary>
		/// <param name="property">The property marked to store extra values</param>
		/// <param name="writer">The JSON writer to write the extra properties to</param>
		/// <param name="value">The model that the property is on</param>
		/// <param name="options">Any options for the JSON serializer</param>
		private void WriteExtensionData(
			PropertyInfo property,
			Utf8JsonWriter writer,
			TModel value,
			JsonSerializerOptions options) {
			// two possible types for this guy
			if (property.PropertyType == typeof(IDictionary<string, JsonElement>)) {
				IDictionary<string, JsonElement>? Data =
					(IDictionary<string, JsonElement>?)property.GetGetMethod()?.Invoke(value, null);
				if (Data == null) return;
				foreach ((string Key, JsonElement JsonElement) in Data) {
					writer.WritePropertyName(Key);
					JsonElement.WriteTo(writer);
				}

				return;
			}

			IDictionary<string, object>? ObjectData =
				(IDictionary<string, object>?)property.GetGetMethod()?.Invoke(value, null);
			if (ObjectData == null) return;
			foreach ((string Key, object Object) in ObjectData) {
				writer.WritePropertyName(Key);
				JsonSerializer.Serialize(writer, Object, options);
			}
		}
	}

	/// <summary>
	///     A JSON property on a model
	/// </summary>
	/// <typeparam name="TModel">The model the property is on</typeparam>
	internal abstract class ModelJsonProperty<TModel> {
		/// <summary>
		///     Writes the property to a JSON writer
		/// </summary>
		/// <param name="writer">The writer to write the value of the property to</param>
		/// <param name="model">The model that contains the property</param>
		/// <param name="options">Options for the JSON serializer</param>
		public abstract void Write(Utf8JsonWriter writer, TModel model, JsonSerializerOptions options);
	}

	/// <summary>
	///     A JSON property on a model
	/// </summary>
	/// <typeparam name="TModel">The model the property is on</typeparam>
	/// <typeparam name="TProperty">The type of property to serialize</typeparam>
	internal class ModelJsonProperty<TModel, TProperty> : ModelJsonProperty<TModel> {
		/// <summary>
		///     The <see cref="JsonConverter" /> to use to serialize the property.
		/// </summary>
		private readonly JsonConverter<TProperty>? Converter;

		/// <summary>
		///     The property being serialized
		/// </summary>
		private readonly PropertyInfo Property;

		/// <summary>
		///     Initializes a new instance of the <see cref="ModelJsonProperty{TModel, TProperty}" /> class
		/// </summary>
		/// <param name="property">The property being serialized</param>
		public ModelJsonProperty(PropertyInfo property) {
			this.Property = property;
			this.Converter = (JsonConverter<TProperty>?)property.GetCustomAttribute<JsonConverterAttribute>()
				?.CreateConverter(property.PropertyType);
		}

		/// <summary>
		///     Writes the property to a JSON writer
		/// </summary>
		/// <param name="writer">The writer to write the value of the property to</param>
		/// <param name="model">The model that contains the property</param>
		/// <param name="options">Options for the JSON serializer</param>
		public override void Write(Utf8JsonWriter writer, TModel model, JsonSerializerOptions options) {
			TProperty Value =
				(TProperty)(this.Property.GetGetMethod()?.Invoke(model, null) ?? throw new JsonException());

			JsonConverter<TProperty>? Converter =
				this.Converter ?? (JsonConverter<TProperty>?)options.GetConverter(this.Property.PropertyType);

			if (this.Converter != null) this.Converter.Write(writer, Value, options);
			else JsonSerializer.Serialize(writer, Value, options);
		}
	}
}