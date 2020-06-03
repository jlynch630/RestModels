// -----------------------------------------------------------------------
// <copyright file="ParsingFailedException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when a parser has failed to parse a request body and request execution should halt
	/// </summary>
	public class ParsingFailedException : ApiException {
		/// <summary>
		///     Initializes a new instance of the <see cref="ParsingFailedException" /> class
		/// </summary>
		public ParsingFailedException() : base(ErrorCodes.ParsingFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ParsingFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public ParsingFailedException(string message)
			: base(message, ErrorCodes.ParsingFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ParsingFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public ParsingFailedException(string message, Exception inner)
			: base(message, inner, ErrorCodes.ParsingFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ParsingFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public ParsingFailedException(string message, int errorCode)
			: base(message, errorCode) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ParsingFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public ParsingFailedException(string message, Exception inner, int errorCode)
			: base(message, inner, errorCode) { }
	}
}