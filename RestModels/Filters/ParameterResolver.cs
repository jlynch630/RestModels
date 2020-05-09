// -----------------------------------------------------------------------
// <copyright file="ParameterResolver.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------
namespace RestModels.Filters {
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Utility class that parses parameters
	/// </summary>
	public static class ParameterResolver {
		/// <summary>
		///     A map of types to functions that convert that type from a string to an instance
		/// </summary>
		private static readonly Dictionary<Type, Func<string, object>> ParameterMap =
			new Dictionary<Type, Func<string, object>> {
				                                           { typeof(Int32), s => Int32.Parse(s) },
				                                           { typeof(Int64), s => Int64.Parse(s) },
				                                           { typeof(Single), s => Single.Parse(s) },
				                                           { typeof(Double), s => Double.Parse(s) },
				                                           { typeof(Guid), s => Guid.Parse(s) },
				                                           { typeof(Decimal), s => Decimal.Parse(s) },
				                                           { typeof(DateTime), s => DateTime.Parse(s) },
				                                           { typeof(bool), s => Boolean.Parse(s) },
				                                           { typeof(string), s => s }
			                                           };

		/// <summary>
		///     Parses a parameter
		/// </summary>
		/// <param name="input">The input received from the HTTP request data</param>
		/// <param name="keyType">The expected type of the parameter</param>
		/// <returns>The parsed primary key</returns>
		/// <exception cref="ArgumentException">
		///     If the type is not one of the types int, long, float, double, Guid, decimal,
		///     DateTime, string, or bool
		/// </exception>
		public static object? ParseParameter(string? input, Type keyType) {
			if (!ParameterResolver.ParameterMap.TryGetValue(keyType, out Func<string, object>? Output))
				throw new ArgumentException(
					"Parameter type is not one of expected int, long, float, double, Guid, decimal, DateTime, bool, string");

			return input == null ? null : (Output?.Invoke(input) ?? null);
		}
	}
}