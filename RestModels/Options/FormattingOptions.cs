// -----------------------------------------------------------------------
// <copyright file="FormattingOptions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options {
	using System.Collections.Generic;
	using System.Reflection;
	using Microsoft.SqlServer.Server;
	using RestModels.Results;

	/// <summary>
	///     Options to be used with an <see cref="IResultWriter{TModel,TUser}" />
	/// </summary>
	public class FormattingOptions {
		/// <summary>
		///     Gets or sets a value indicating whether the data should be returned formatted as a single element if the dataset
		///     only has one element in it.
		/// </summary>
		/// <remarks>
		///     For example, in JSON, this would be the difference between returning [{ ... }] and { ... }
		/// </remarks>
		public bool StripArrayIfSingleElement { get; set; }

		/// <summary>
		///     Gets or sets a list of properties that should be included in the result
		/// </summary>
		public List<PropertyInfo> IncludedReturnProperties { get; set; }

		/// <summary>
		///		Makes a copy of these <see cref="FormattingOptions"/>
		/// </summary>
		/// <returns>A copy of these <see cref="FormattingOptions"/></returns>
		public FormattingOptions Copy() {
			return new FormattingOptions {
				                             StripArrayIfSingleElement = this.StripArrayIfSingleElement,
				                             IncludedReturnProperties =
					                             this.IncludedReturnProperties == null
						                             ? null
						                             : new List<PropertyInfo>(this.IncludedReturnProperties)
			                             };
		}
	}
}