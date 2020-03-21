// -----------------------------------------------------------------------
// <copyright file="ParserOptions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options {
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	///     Options for parsing HTTP bodies
	/// </summary>
	public class ParserOptions {
		/// <summary>
		///     Gets or sets a list of default values for parsed properties
		/// </summary>
		public Dictionary<PropertyInfo, object> DefaultPropertyValues { get; set; }

		/// <summary>
		///     Gets or sets a list of properties that are ignored if their values are set
		/// </summary>
		public List<PropertyInfo> IgnoredParseProperties { get; set; }

		/// <summary>
		///     Gets or sets a list of properties that are required when parsing
		/// </summary>
		public List<PropertyInfo> RequiredParseProperties { get; set; }

		/// <summary>
		///		Makes a copy of these <see cref="ParserOptions"/>
		/// </summary>
		/// <returns>A copy of these <see cref="ParserOptions"/></returns>
		public ParserOptions Copy() {
			return new ParserOptions {
				                         DefaultPropertyValues =
					                         new Dictionary<PropertyInfo, object>(this.DefaultPropertyValues),
				                         IgnoredParseProperties = new List<PropertyInfo>(this.IgnoredParseProperties),
				                         RequiredParseProperties = new List<PropertyInfo>(this.RequiredParseProperties)
			                         };
		}
	}
}