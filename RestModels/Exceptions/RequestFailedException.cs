// -----------------------------------------------------------------------
// <copyright file="RequestFailedException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when request execution has failed
	/// </summary>
	public class RequestFailedException : Exception {
		/// <summary>
		///     Initializes a new instance of the <see cref="RequestFailedException" /> class
		/// </summary>
		public RequestFailedException() { }

		/// <summary>
		///     Initializes a new instance of the <see cref="RequestFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public RequestFailedException(string message)
			: base(message) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="RequestFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public RequestFailedException(string message, Exception inner)
			: base(message, inner) { }
	}
}