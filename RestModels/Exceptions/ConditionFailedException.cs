// -----------------------------------------------------------------------
// <copyright file="ConditionFailedException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when a condition check has failed, indicating that request execution should halt
	/// </summary>
	public class ConditionFailedException : ApiException {
		/// <summary>
		///     Initializes a new instance of the <see cref="ConditionFailedException" /> class
		/// </summary>
		public ConditionFailedException() : base(ErrorCodes.ConditionFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ConditionFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public ConditionFailedException(string message)
			: base(message, ErrorCodes.ConditionFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ConditionFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public ConditionFailedException(string message, Exception inner)
			: base(message, inner, ErrorCodes.ConditionFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ConditionFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public ConditionFailedException(string message, int errorCode)
			: base(message, errorCode) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ConditionFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public ConditionFailedException(string message, Exception inner, int errorCode)
			: base(message, inner, errorCode) { }
	}
}