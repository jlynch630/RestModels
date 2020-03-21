// -----------------------------------------------------------------------
// <copyright file="InvalidParserException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when a parser is deemed invalid to parse the given request
	/// </summary>
	/// <remarks>
	///     If thrown from a parser, this exception indicates that the chosen parser was invalid for the given input, but that
	///     request execution should
	///     continue with the next available parser.
	///     If thrown from middleware, this exception indicates that all parsers have failed and that request execution should
	///     halt.
	/// </remarks>
	public class InvalidParserException : Exception {
		/// <summary>
		///     Initializes a new instance of the <see cref="InvalidParserException" /> class
		/// </summary>
		public InvalidParserException() { }

		/// <summary>
		///     Initializes a new instance of the <see cref="InvalidParserException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public InvalidParserException(string message)
			: base(message) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="InvalidParserException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public InvalidParserException(string message, Exception inner)
			: base(message, inner) { }
	}
}