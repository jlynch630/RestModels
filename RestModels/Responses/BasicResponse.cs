// -----------------------------------------------------------------------
// <copyright file="BasicResponse.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Responses {
	using RestModels.Responses.Attributes;

	/// <summary>
	///     A basic API response
	/// </summary>
	/// <typeparam name="TModel">The type of model included in the API response</typeparam>
	public class BasicResponse<TModel> : Response<TModel>
		where TModel : class {
		/// <summary>
		///     Gets or sets a string which, when present, indicates the version of this API
		/// </summary>
		[ApiVersion]
		public string? ApiVersion { get; set; }

		/// <summary>
		///     Gets or sets an integer which, when present, indicates the number of elements included in the response
		/// </summary>
		[ModelCount]
		public int Count { get; set; }

		/// <summary>
		///     Gets or sets a string which, when present, indicates that this API version has been deprecated
		/// </summary>
		[DeprecationNotice]
		public string? DeprecationNotice { get; set; }

		/// <summary>
		///     Gets or sets the element to include in the API response
		/// </summary>
		public TModel? Element { get; set; }

		/// <summary>
		///     Gets or sets the elements to include in the API response
		/// </summary>
		public TModel[]? Elements { get; set; }

		/// <summary>
		///     Gets or sets an integer which, when present, indicates the current page when using pagination
		/// </summary>
		[CurrentPage]
		public int Page { get; set; }

		/// <summary>
		///     Gets or sets an integer which, when present, indicates the total number of pages when using pagination
		/// </summary>
		[TotalPages]
		public int Pages { get; set; }
	}
}