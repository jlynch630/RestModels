// -----------------------------------------------------------------------
// <copyright file="ApiException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when an API error occurs
	/// </summary>
	/// <remarks>
	///     When this exception is thrown, it indicates that an error occurred processing the request, but that the error
	///     contains no sensitive data and should be communicated to the API consumer.
	/// </remarks>
	public class ApiException : Exception {
		/// <summary>
		///     Initializes a new instance of the <see cref="ApiException" /> class
		/// </summary>
		public ApiException() : this(ErrorCodes.ApiError) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ApiException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public ApiException(string? message)
			: this(message, ErrorCodes.ApiError) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ApiException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public ApiException(string? message, Exception? inner)
			: this(message, inner, ErrorCodes.ApiError) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ApiException" /> class
		/// </summary>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public ApiException(int errorCode)
			: this(null, errorCode) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ApiException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public ApiException(string? message, int errorCode)
			: this(message, null, errorCode) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="ApiException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public ApiException(string? message, Exception? inner, int errorCode)
			: base(message, inner) {
			this.ErrorCode = errorCode;
		}

		/// <summary>
		///		Gets or sets a unique code for the error that occurred
		/// </summary>
		public int ErrorCode { get; set; }
	}
}