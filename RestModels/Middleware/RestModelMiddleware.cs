﻿// -----------------------------------------------------------------------
// <copyright file="RestModelMiddleware.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Middleware {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection;
	using RestModels.Auth;
	using RestModels.Conditions;
	using RestModels.Context;
	using RestModels.ExceptionHandlers;
	using RestModels.Exceptions;
	using RestModels.Filters;
	using RestModels.Options;
	using RestModels.Parsers;
	using RestModels.Responses;

	/// <summary>
	///     Middleware for RestModels.
	///     It's not traditional ASP.NET Core middleware, but it's essentially the same thing
	/// </summary>
	/// <typeparam name="TModel">The type of model being handled by this middleware</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context, if any</typeparam>
	internal class RestModelMiddleware<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The options to use for determining the route actions
		/// </summary>
		private readonly RestModelOptions<TModel, TUser> Options;

		/// <summary>
		///     Initializes a new instance of the <see cref="RestModelMiddleware{TModel, TUser}" /> class
		/// </summary>
		/// <param name="options">The options to use for determining the route actions</param>
		public RestModelMiddleware(RestModelOptions<TModel, TUser> options) => this.Options = options;

		/// <summary>
		///     Handles an incoming request
		/// </summary>
		/// <param name="context">The http context of the current request</param>
		/// <param name="hasNext"><c>true</c> if this route has another middleware registered to fall back on, <c>false</c> otherwise</param>
		/// <returns><c>true</c> to attempt to use the next request handler, <c>false</c> to finish request execution here</returns>
		public async Task<bool> TryHandleRequest(HttpContext context, bool hasNext) {
			/**
				Order of things here
					- MATCH the route and method!
						- Done!
				 ---- Other stuff
				|
				|---- IF an exception is thrown anywhere here
						- CALL exception handlers
			**/
			try {
				Stopwatch Stopwatch = new Stopwatch();
				Stopwatch.Start();

				await this.HandleRequest(context);

				Stopwatch.Stop();
				Debug.WriteLine($"elapsed: {Stopwatch.ElapsedMilliseconds}");
				return false;
			}
			catch (Exception e) {
				if (this.Options.ExceptionHandlers.Count == 0) throw;
				foreach (IExceptionHandler Handler in this.Options.ExceptionHandlers) {
					bool? Result = await Handler.HandleException(e, context, hasNext);
					if (Result.HasValue) return Result.Value;
				}

				// by default, go to the next matching route
				return true;
			}
		}

		/// <summary>
		///     Authenticates the request using the auth providers specified by this middleware's options, if any
		/// </summary>
		/// <param name="context">The current api context</param>
		/// <returns>The authenticated user context, or null if there is none</returns>
		/// <exception cref="AuthFailedException">If none of the available authentication methods succeeded</exception>
		private async Task<TUser?> Authenticate(ApiContext<TModel, TUser> context) {
			if (this.Options.AuthProviders == null) return null;

			TUser? UserContext = null;
			bool AuthSuccess = false;
			AuthFailedException? LastException = null;
			foreach (IAuthProvider<TModel, TUser> Provider in this.Options.AuthProviders)
				try {
					if (!await Provider.CanAuthAsync(context)) continue;
					UserContext = await Provider.AuthenticateAsync(context);
					AuthSuccess = true;
					break;
				}
				catch (AuthFailedException e) {
					LastException = e;
					/* Just keep moving, by design */
				}

			return AuthSuccess
				       ? UserContext
				       : throw new AuthFailedException(
					         "No authentication providers were able to authorize the request",
							 LastException!);
		}

		/// <summary>
		///     Handles an incoming request
		/// </summary>
		/// <param name="context">The http context of the current request</param>
		/// <returns>When the request is completed</returns>
		private async Task HandleRequest(HttpContext context) {
			/**
			 *		- CHECK if we can even serialize the result
			 *		- PARSE body if there are body parsers present
			   |		- then parse the request body, and pass that along
			   |	- AUTH if there are auth providers present
			   |		- then ask them what the deal is and either pass one or all
			   |    - QUERY the dataset with our parsed data and auth
			   |	- FILTER the dbset by the filters provided
			   |	- CHECK if any conditions need to be met
			   |	- MODIFY data if there are any IOperations
			   |	- RETURN formatted output
			 
				note that the only required element should be the result writer AND the request method
				(even an empty result writer is required to return a blank result)
			*/
			// go through our body parsers to try and parse the request body. may be null
			Stopwatch Stopwatch = new Stopwatch();
			Stopwatch.Start();
			if (!await this.Options.ResultWriter!.CanWriteAsync(context.Request))
				throw new WritingFailedException("Request aborted. Cannot serialize response to request");

			// alright first we have to create the context
			Response<TModel>? ResponseWrapper = this.Options.ResponseType != null
													? (Response<TModel>?)Activator.CreateInstance(this.Options.ResponseType)
													: null;
			ApiContext<TModel, TUser> ApiContext = new ApiContext<TModel, TUser>(context, ResponseWrapper);

			ApiContext.Parsed = await this.ParseBody(ApiContext);
			//////////////////////
			Stopwatch.Stop();
			Debug.WriteLine($"Parse\t{Stopwatch.ElapsedMilliseconds}");
			Stopwatch.Restart();
			/////////////////////
			// then authenticate the request. may return null
			ApiContext.User = await this.Authenticate(ApiContext);
			//////////////////////
			Stopwatch.Stop();
			Debug.WriteLine($"Auth\t{Stopwatch.ElapsedMilliseconds}");
			Stopwatch.Restart();
			/////////////////////
			// query the dataset for models
			IQueryable<TModel> Dataset = this.Options.ModelProvider != null
				                             ? await this.Options.ModelProvider.GetModelsAsync(ApiContext)
				                             : new TModel[0].AsQueryable();
			//////////////////////
			Stopwatch.Stop();
			Debug.WriteLine($"Query\t{Stopwatch.ElapsedMilliseconds}");
			Stopwatch.Restart();
			/////////////////////
			// filter our dataset
			if (this.Options.Filters.Count > 0 && Dataset == null)
				throw new RequestFailedException("Attempt to filter dataset but no model provider was given");
			foreach (IFilter<TModel, TUser> Filter in this.Options.Filters)
				Dataset = await Filter.FilterDataAsync(ApiContext, Dataset);

			// ensure it passes all conditions
			await this.VerifyConditions(ApiContext, Dataset);
			//////////////////////
			Stopwatch.Stop();
			Debug.WriteLine($"Filter\t{Stopwatch.ElapsedMilliseconds}");
			Stopwatch.Restart();
			/////////////////////
			// then do our operation
			IEnumerable<TModel> Result = this.Options.Operation != null
				                             ? await this.Options.Operation.OperateAsync(ApiContext, Dataset)
				                             : Dataset;
			//////////////////////
			Stopwatch.Stop();
			Debug.WriteLine($"Op\t{Stopwatch.ElapsedMilliseconds}");
			Stopwatch.Restart();
			/////////////////////
			// and write the result
			await this.Options.ResultWriter!.WriteResultAsync(
				ApiContext,
				Result,
				this.Options.FormattingOptions);
			//////////////////////
			Stopwatch.Stop();
			Debug.WriteLine($"Write\t{Stopwatch.ElapsedMilliseconds}");
			/////////////////////
			ApiContext.Dispose();
		}

		/// <summary>
		///     Parses the request body using the body parsers specified by this middleware's options, if any
		/// </summary>
		/// <param name="context">The current api context</param>
		/// <returns>The parsed model, or null if there are no body parsers specified</returns>
		/// <exception cref="InvalidParserException">If none of the available parsers can properly parse the request</exception>
		private async Task<ParseResult<TModel>[]?> ParseBody(ApiContext<TModel, TUser> context) {
			if (this.Options.BodyParsers == null) return null;

			// todo: scoped services, response?
			await using MemoryStream Stream = new MemoryStream();
			await context.Request.Body.CopyToAsync(Stream);
			byte[] BodyContents = Stream.ToArray();
			ParseResult<TModel>[]? Parsed = null;
			bool ParseSuccess = false;
			foreach (IBodyParser<TModel> Parser in this.Options.BodyParsers)
				try {
					if (!await Parser.CanParse(context.HttpContext)) continue;
					Parsed = await Parser.Parse(BodyContents, this.Options.ParserOptions, context.HttpContext);
					ParseSuccess = true;
					break;
				}
				catch (InvalidParserException) {
					/* Just keep moving, by design */
				}

			return ParseSuccess ? Parsed : throw new InvalidParserException("No valid parsers found for request body");
		}

		/// <summary>
		///     Verifies that all the conditions provided in the options are met for a given request
		/// </summary>
		/// <param name="context">The current api context</param>
		/// <param name="dataset">The dataset to verify conditions for</param>
		/// <returns>When all conditions have passed</returns>
		/// <exception cref="ConditionFailedException">If any conditions fail</exception>
		private async Task VerifyConditions(
			ApiContext<TModel, TUser> context,
			IQueryable<TModel> dataset) {
			foreach (ICondition<TModel, TUser> Condition in this.Options.Conditions)
				try {
					if (!await Condition.VerifyAsync(context, dataset))
						throw new ConditionFailedException(Condition.FailureMessage ?? "Condition was not met");
				}
				catch (Exception e) when (!(e is ConditionFailedException)) {
					// wrap everything in a condition failed exception
					throw new ConditionFailedException("Condition was not met", e);
				}
		}
	}
}