// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.Template.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Auth;
	using RestModels.Conditions;
	using RestModels.ExceptionHandlers;
	using RestModels.Exceptions;
	using RestModels.Filters;
	using RestModels.Models;
	using RestModels.Operations;
	using RestModels.Parsers;
	using RestModels.Results;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Template<TException>(Func<Exception, HttpContext, bool, Task<bool?>> handler) where TException : Exception {
			this.Catch<TException>(new DelegateExceptionHandler(handler));
			return this;
		}
	}
}