// -----------------------------------------------------------------------
// <copyright file="OperationFailedException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when a create, update, delete, or other operation has failed
	/// </summary>
	public class OperationFailedException : Exception {
		/// <summary>
		///     Initializes a new instance of the <see cref="OperationFailedException" /> class
		/// </summary>
		public OperationFailedException() { }

		/// <summary>
		///     Initializes a new instance of the <see cref="OperationFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public OperationFailedException(string message)
			: base(message) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="OperationFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public OperationFailedException(string message, Exception inner)
			: base(message, inner) { }
	}
}