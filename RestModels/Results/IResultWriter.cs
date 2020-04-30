﻿// -----------------------------------------------------------------------
// <copyright file="IResultWriter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using RestModels.Options;

	/// <summary>
	///     Writer for API results
	/// </summary>
	/// <typeparam name="TModel">The type of model to format</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public interface IResultWriter<in TModel, in TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///		Gets whether or not this <see cref="IResultWriter{TModel, TUser}"/> can write a result for the given request
		/// </summary>
		/// <param name="request">The request to test if a result can be written for it</param>
		/// <returns><code>true</code> if a result can be written for <paramref name="request"/>, <code>false</code> otherwise</returns>
		Task<bool> CanWriteAsync(HttpRequest request);

		/// <summary>
		///     Formats the API result
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="data">The dataset to format</param>
		/// <param name="user">The current authenticated user context</param>
		/// <param name="options">Options for formatting the result</param>
		/// <returns>When the result has been sent</returns>
		Task WriteResultAsync(HttpContext context, IEnumerable<TModel> data, TUser user, FormattingOptions options);
	}

	/// <summary>
	///     Writer for API results
	/// </summary>
	/// <typeparam name="TModel">The type of model to format</typeparam>
	public interface IResultWriter<in TModel> : IResultWriter<TModel, object> where TModel : class { }
}