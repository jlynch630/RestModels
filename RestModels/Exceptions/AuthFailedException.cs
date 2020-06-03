﻿// -----------------------------------------------------------------------
// <copyright file="AuthFailedException.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	using System;

	/// <summary>
	///     Exception thrown when an authentication provider fails to authenticate a request
	/// </summary>
	/// <remarks>
	///     If thrown from an auth provider, this exception indicates that the chosen auth provider was not able to
	///     authenticate for the given request, but that request execution should
	///     continue with the next available provider.
	///     If thrown from middleware, this exception indicates that all auth providers failed and request execution should
	///     halt.
	/// </remarks>
	public class AuthFailedException : ApiException {
		/// <summary>
		///     Initializes a new instance of the <see cref="AuthFailedException" /> class
		/// </summary>
		public AuthFailedException() : base(ErrorCodes.AuthFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="AuthFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		public AuthFailedException(string message)
			: base(message, ErrorCodes.AuthFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="AuthFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		public AuthFailedException(string message, Exception inner)
			: base(message, inner, ErrorCodes.AuthFailed) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="AuthFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public AuthFailedException(string message, int errorCode)
			: base(message, errorCode) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="AuthFailedException" /> class
		/// </summary>
		/// <param name="message">A message explaining the exception</param>
		/// <param name="inner">The inner exception that caused this one</param>
		/// <param name="errorCode">A unique error code for the error that occurred</param>
		public AuthFailedException(string message, Exception inner, int errorCode)
			: base(message, inner, errorCode) { }
	}
}