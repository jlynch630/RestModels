// -----------------------------------------------------------------------
// <copyright file="IModelJsonConverter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results.Json {
	using System.Text.Json;

	/// <summary>
	///		Interface that defines a JSON converter that can convert API objects
	/// </summary>
	public interface IModelJsonConverter {
		/// <summary>
		///     Writes the given model object to a JSON writer
		/// </summary>
		/// <param name="writer">The JSON writer to write the <paramref name="value" /> to</param>
		/// <param name="value">The value to write</param>
		/// <param name="options">Any options to use when serializing the JSON</param>
		void WriteObject(Utf8JsonWriter writer, object value, JsonSerializerOptions options);
	}
}