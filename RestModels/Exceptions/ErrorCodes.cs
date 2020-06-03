// -----------------------------------------------------------------------
// <copyright file="ErrorCodes.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Exceptions {
	/// <summary>
	///     Constants class of error codes for <see cref="ApiException" />
	/// </summary>
	public static class ErrorCodes {
		/// <summary>
		///     Generic error code indicating that the request has failed
		/// </summary>
		public const int ApiError = 1;

		/// <summary>
		///     Error code indicating that authentication has failed
		/// </summary>
		public const int AuthFailed = 2;

		/// <summary>
		///     Error code indicating that parsing has failed
		/// </summary>
		public const int ParsingFailed = 3;

		/// <summary>
		///     Error code indicating that a required condition was not met
		/// </summary>
		public const int ConditionFailed = 4;

		/// <summary>
		///     Error code indicating that the operation failed
		/// </summary>
		public const int OperationFailed = 5;

		/// <summary>
		///     Error code indicating that writing the API response failed
		/// </summary>
		public const int WritingFailed = 6;

		/// <summary>
		///     Error code indicating that authentication succeeded, but the user does not have the required permissions to access this route.
		/// </summary>
		public const int Forbidden = 7;
	}
}