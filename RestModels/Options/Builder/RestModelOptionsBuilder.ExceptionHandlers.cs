// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.ExceptionHandlers.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.ExceptionHandlers;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds an exception handler to this route
		/// </summary>
		/// <param name="exceptionHandler">The exception handler to add</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddExceptionHandler(IExceptionHandler exceptionHandler) {
			if (this.Options.ExceptionHandlers == null) this.Options.ExceptionHandlers = new List<IExceptionHandler>();
			this.Options.ExceptionHandlers.Add(exceptionHandler);
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route
		/// </summary>
		/// <typeparam name="THandler">The type of exception handler to add</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> AddExceptionHandler<THandler>()
			where THandler : IExceptionHandler, new() {
			this.AddExceptionHandler(new THandler());
			return this;
		}

		// what follows is 19 methods that take various lambdas for catching exceptions
		// either (Exception, HttpContext) or (Exception) parameters, either bool? or void return values, either async or not, and either specific exception or all
		// an additional method takes an IExceptionHandler for a specific method

		// (Exception, HttpContext, bool) =>, Catch(Async) methods

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch<TException>(Func<Exception, HttpContext, bool?> handler)
			where TException : Exception {
			this.CatchAsync<TException>((e, c) => Task.FromResult(handler(e, c)));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch<TException>(Action<Exception, HttpContext> handler)
			where TException : Exception {
			this.Catch<TException>(
				(e, c) => {
					handler(e, c);
					return null;
				});
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <param name="handler">
		///     The exception handler that can handle exceptions of type <typeparamref name="TException" />
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch<TException>(IExceptionHandler handler)
			where TException : Exception {
			this.CatchAsync(
				async (e, c, b) => {
					if (!(e is TException)) return null;
					return await handler.HandleException(e, c, b);
				});
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch(Func<Exception, HttpContext, bool?> handler) {
			this.CatchAsync((e, c) => Task.FromResult(handler(e, c)));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch(Action<Exception, HttpContext> handler) {
			this.Catch(
				(e, c) => {
					handler(e, c);
					return null;
				});
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch<TException>(Func<Exception, bool?> handler)
			where TException : Exception {
			this.Catch<TException>((e, c) => handler(e));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch<TException>(Action<Exception> handler)
			where TException : Exception {
			this.Catch<TException>((e, c) => handler(e));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch(Func<Exception, bool?> handler) {
			this.Catch((e, c) => handler(e));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Catch(Action<Exception> handler) {
			this.Catch((e, c) => handler(e));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync<TException>(
			Func<Exception, HttpContext, Task<bool?>> handler)
			where TException : Exception {
			return this.CatchAsync<TException>((e, h, next) => handler(e, h));
		}

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync<TException>(
			Func<Exception, HttpContext, bool, Task<bool?>> handler)
			where TException : Exception {
			this.Catch<TException>(new DelegateExceptionHandler(handler));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync<TException>(Func<Exception, HttpContext, Task> handler)
			where TException : Exception {
			this.CatchAsync<TException>(
				async (e, c) => {
					await handler(e, c);
					return null;
				});
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync(Func<Exception, HttpContext, bool, Task<bool?>> handler) {
			this.AddExceptionHandler(new DelegateExceptionHandler(handler));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync(Func<Exception, HttpContext, Task<bool?>> handler) {
			this.CatchAsync((e, c, next) => handler(e, c));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync(Func<Exception, HttpContext, Task> handler) {
			this.CatchAsync(
				async (e, c) => {
					await handler(e, c);
					return null;
				});
			return this;
		}
		
		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync<TException>(Func<Exception, Task<bool?>> handler)
			where TException : Exception {
			this.Catch<TException>((e, c) => handler(e));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles a specific type of exception
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <typeparam name="TException">The type of exception that this handler will be called for</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync<TException>(Func<Exception, Task> handler)
			where TException : Exception {
			this.CatchAsync<TException>((e, c) => handler(e));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync(Func<Exception, Task<bool?>> handler) {
			this.CatchAsync((e, c) => handler(e));
			return this;
		}

		/// <summary>
		///     Adds an exception handler to this route that handles all exceptions
		/// </summary>
		/// <param name="handler">The handler to use for a thrown exception</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> CatchAsync(Func<Exception, Task> handler) {
			this.CatchAsync((e, c) => handler(e));
			return this;
		}
	}
}