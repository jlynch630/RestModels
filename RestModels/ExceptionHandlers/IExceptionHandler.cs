﻿// -----------------------------------------------------------------------
// <copyright file="IExceptionHandler.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.ExceptionHandlers {
	using System;
	using System.IO;

	using Microsoft.AspNetCore.Http;

	/// <summary>
	///     Handler for API exceptions
	/// </summary>
	public interface IExceptionHandler {
		/// <summary>
		///     Handles API exceptions
		/// </summary>
		/// <param name="exception">The exception that was thrown</param>
		/// <param name="context">The current request context</param>
		/// <param name="writeStream">
		///     The stream to write the response body out to, equal to the <see cref="HttpResponse.Body" />
		///     of the <paramref name="context" />
		/// </param>
		/// <returns>
		///     <code>true</code> if the request should continue and attempt to use the next middleware registered for this
		///     route, <code>false</code> to halt request execution, <code>null</code> to continue with the next exception handler.
		/// </returns>
		bool? HandleException(Exception exception, HttpContext context, Stream writeStream);
	}
}