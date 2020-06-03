// -----------------------------------------------------------------------
// <copyright file="TotalPagesAttribute.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Responses.Attributes {
	using System;

	/// <summary>
	///     Attribute that indicates that the property is a response value that should be set to the total number of pages if
	///     using pagination
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class TotalPagesAttribute : Attribute { }
}