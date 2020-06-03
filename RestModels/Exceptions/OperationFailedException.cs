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
	public class OperationFailedException : ApiException {
		/// <summary>
		///     Initializes a new instance of the <see cref="OperationFailedException" /> class
		/// </summary>
		public OperationFailedException() : base(ErrorCodes.OperationFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="OperationFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public OperationFailedException(string message)
			: base(message, ErrorCodes.OperationFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="OperationFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public OperationFailedException(string message, Exception inner)
			: base(message, inner, ErrorCodes.OperationFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="OperationFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public OperationFailedException(string message, int errorCode)
			: base(message, errorCode) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="OperationFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public OperationFailedException(string message, Exception inner, int errorCode)
			: base(message, inner, errorCode) { }
	}
}