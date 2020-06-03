// -----------------------------------------------------------------------
// <copyright file="ResponseValueAttribute.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Responses.Attributes {
	using System;

	/// <summary>
	///     Attribute that indicates the property is a response value and can be dynamically set by name
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ResponseValueAttribute : Attribute {
		/// <summary>
		///     Initializes a new instance of the <see cref="ResponseValueAttribute" /> class
		/// </summary>
		/// <param name="name">The name to use to refer to this response value</param>
		public ResponseValueAttribute(string name) => this.Name = name;

		/// <summary>
		///     Gets the name to use to refer to this response value
		/// </summary>
		public string Name { get; }
	}
}