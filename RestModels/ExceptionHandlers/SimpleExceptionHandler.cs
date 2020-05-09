// -----------------------------------------------------------------------
// <copyright file="SimpleExceptionHandler.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.ExceptionHandlers {
	using System;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Exceptions;

	/// <summary>
	///     An exception handler that writes the message of the exception to the response and returns a 400 status code for
	///     expected exceptions or 500 for unexpected ones.
	/// </summary>
	public class SimpleExceptionHandler : IExceptionHandler {
		/// <summary>
		///     Handles API exceptions
		/// </summary>
		/// <param name="exception">The exception that was thrown</param>
		/// <param name="context">The current request context</param>
		/// <param name="hasNext"><c>true</c> if there is another middleware registered for this route, <c>false</c> otherwise</param>
		/// <returns>
		///     <code>true</code> if the request should continue and attempt to use the next middleware registered for this
		///     route, <code>false</code> to halt request execution, <code>null</code> to continue with the next exception handler.
		/// </returns>
		public async Task<bool?> HandleException(Exception exception, HttpContext context, bool hasNext) {
			context.Response.ContentType = "text/plain";
			switch (exception) {
				case ConditionFailedException _:
				case ParsingFailedException _:
				case InvalidParserException _:
				case OperationFailedException _:
					context.Response.StatusCode = StatusCodes.Status400BadRequest;
					break;
				case AuthFailedException _:
					if (hasNext)
						return null; // don't handle that here cause we could want to pass-through to next middleware
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					break;
				default:
					context.Response.StatusCode = StatusCodes.Status500InternalServerError;
					await context.Response.WriteAsync("An unexpected error occurred");
					return false;
			}

			string Message = exception.Message;
			if (exception.InnerException != null)
				Message += $"\r\nReason: {exception.InnerException.Message}";
			await context.Response.WriteAsync(Message);
			return false;
		}
	}
}