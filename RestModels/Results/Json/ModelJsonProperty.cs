// -----------------------------------------------------------------------
// <copyright file="ModelJsonProperty.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results.Json {
	using System.Reflection;
	using System.Text.Json;
	using System.Text.Json.Serialization;

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