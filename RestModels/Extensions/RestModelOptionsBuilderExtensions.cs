﻿// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilderExtensions.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Extensions {
	using System;
	using System.Threading.Tasks;

	using RestModels.Auth;
	using RestModels.ExceptionHandlers;
	using RestModels.Exceptions;
	using RestModels.Options.Builder;

	/// <summary>
	///     Extension methods for the <see cref="RestModelOptionsBuilder{TModel,TUser}" /> class.
	/// </summary>
	public static class RestModelOptionsBuilderExtensions {
		/// <summary>
		///     Adds an auth provider to this route that authenticates with basic auth
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="handler">
		///     The handler which, when given the username and password, will return <c>true</c> if the value is valid and
		///     <c>false</c> otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthBasic<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			Func<string, string, bool> handler)
			where TModel : class {
			return builder.AuthBasicAsync(async (u, p) => handler(u, p));
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with basic auth
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="username">
		///     The expected value of the username. Authorization will succeed if both this and <paramref name="password" /> value
		///     equals
		///     the actual values and fail otherwise
		/// </param>
		/// <param name="password">
		///     The expected value of the password. Authorization will succeed if both this and <paramref name="username" /> value
		///     equals
		///     the actual values and fail otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthBasic<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			string username,
			string password)
			where TModel : class {
			return builder.AuthBasic((u, p) => u == username && p == password);
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with basic auth
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="handler">
		///     The handler which, when given the username and password, will return <c>true</c> if the value is valid and
		///     <c>false</c> otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthBasicAsync<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			Func<string, string, Task<bool>> handler)
			where TModel : class {
			return builder.AuthBasicAsync(
				async (u, p) => {
					if (await handler(u, p)) return (NoUser)null;
					throw new AuthFailedException();
				});
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a header
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterName">The name of the header to authenticate with</param>
		/// <param name="handler">
		///     The handler which, when given the header value, will return <c>true</c> if the value is valid and
		///     <c>false</c> otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthHeader<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			string parameterName,
			Func<string, bool> handler)
			where TModel : class {
			return builder.AuthHeaderAsync(parameterName, async s => handler(s));
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a header
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterName">The name of the header to authenticate with</param>
		/// <param name="value">
		///     The expected value of the parameter. Authorization will succeed if the parameter value equals
		///     <paramref name="value" /> and fail otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthHeader<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			string parameterName,
			string value)
			where TModel : class {
			return builder.AuthHeader(parameterName, s => s == value);
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a header
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterName">The name of the header to authenticate with</param>
		/// <param name="handler">
		///     The handler which, when given the header value, will return <c>true</c> if the value is valid and
		///     <c>false</c> otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthHeaderAsync<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			string parameterName,
			Func<string, Task<bool>> handler)
			where TModel : class {
			return builder.AuthHeaderAsync(
				parameterName,
				async s => {
					if (await handler(s)) return (NoUser)null;
					throw new AuthFailedException();
				});
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a query parameter
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterName">The name of the parameter to authenticate with</param>
		/// <param name="handler">
		///     The handler which, when given the parameter value, will return <c>true</c> if the value is valid
		///     and <c>false</c> otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthQuery<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			string parameterName,
			Func<string, bool> handler)
			where TModel : class {
			return builder.AuthQueryAsync(parameterName, async s => handler(s));
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a query parameter
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterName">The name of the parameter to authenticate with</param>
		/// <param name="value">
		///     The expected value of the parameter. Authorization will succeed if the parameter value equals
		///     <paramref name="value" /> and fail otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthQuery<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			string parameterName,
			string value)
			where TModel : class {
			return builder.AuthQuery(parameterName, s => s == value);
		}

		/// <summary>
		///     Adds an auth provider to this route that authenticates with a query parameter
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <param name="parameterName">The name of the parameter to authenticate with</param>
		/// <param name="handler">
		///     The handler which, when given the parameter value, will return <c>true</c> if the value is valid
		///     and <c>false</c> otherwise
		/// </param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, NoUser> AuthQueryAsync<TModel>(
			this RestModelOptionsBuilder<TModel, NoUser> builder,
			string parameterName,
			Func<string, Task<bool>> handler)
			where TModel : class {
			return builder.AuthQueryAsync(
				parameterName,
				async s => {
					if (await handler(s)) return (NoUser)null;
					throw new AuthFailedException();
				});
		}

		/// <summary>
		///     Sets this route up to catch all exceptions and halt request execution after one is thrown.
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> CatchAllExceptions<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			builder.CatchExceptions();
			builder.FailOnInvalidAuth();
			return builder;
		}

		/// <summary>
		///     Sets this route up to catch all exceptions except for <see cref="AuthFailedException" /> exceptions to enable
		///     middleware pass-through
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> CatchExceptions<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			builder.AddExceptionHandler<SimpleExceptionHandler>();
			return builder;
		}

		/// <summary>
		///     Sets this route up to halt request execution if authentication fails
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> FailOnInvalidAuth<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			builder.AddExceptionHandler<AuthFailedExceptionHandler>();
			return builder;
		}

		/// <summary>
		///     Resets this <see cref="RestModelOptionsBuilder{TModel, TUser}" />, clearing all lists and resetting all values
		///     except for the route pattern
		/// </summary>
		/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
		/// <typeparam name="TUser">The type of authenticated user context</typeparam>
		/// <param name="builder">The options builder to perform the operation on</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public static RestModelOptionsBuilder<TModel, TUser> Reset<TModel, TUser>(
			this RestModelOptionsBuilder<TModel, TUser> builder)
			where TModel : class where TUser : class {
			builder.ClearAuthProviders();
			builder.ClearBodyParsers();
			builder.ClearConditions();
			builder.ClearExceptionHandlers();
			builder.ClearFilters();
			builder.ClearOperation();
			builder.ClearRequestMethods();
			builder.ClearResultWriter();
			return builder;
		}
	}
}