// -----------------------------------------------------------------------
// <copyright file="WritingFailedException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when writing a result has failed
	/// </summary>
	public class WritingFailedException : ApiException {
		/// <summary>
		///     Initializes a new instance of the <see cref="WritingFailedException" /> class
		/// </summary>
		public WritingFailedException() : base(ErrorCodes.WritingFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="WritingFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public WritingFailedException(string message)
			: base(message, ErrorCodes.WritingFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="WritingFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public WritingFailedException(string message, Exception inner)
			: base(message, inner, ErrorCodes.WritingFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="WritingFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public WritingFailedException(string message, int errorCode)
			: base(message, errorCode) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="WritingFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public WritingFailedException(string message, Exception inner, int errorCode)
			: base(message, inner, errorCode) { }
	}
}