// -----------------------------------------------------------------------
// <copyright file="OptionsException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when an error occurs while building a <see cref="RestModelOptions{TModel,TUser}"/>
	/// </summary>
	public class OptionsException : Exception {
		/// <summary>
		///     Initializes a new instance of the <see cref="OptionsException" /> class
		/// </summary>
		public OptionsException() { }

		/// <summary>
		///     Initializes a new instance of the <see cref="OptionsException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public OptionsException(string message)
			: base(message) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="OptionsException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public OptionsException(string message, Exception inner)
			: base(message, inner) { }
	}
}