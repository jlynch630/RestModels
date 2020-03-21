// -----------------------------------------------------------------------
// <copyright file="RestModelMiddleware.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Middleware {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection;
	using RestModels.Auth;
	using RestModels.Conditions;
	using RestModels.Exceptions;
	using RestModels.Filters;
	using RestModels.Options;
	using RestModels.Parsers;

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
		///		A list of types to their constructors, for dependency injection
		/// </summary>
		private readonly Dictionary<Type,ConstructorInfo> Constructors = new Dictionary<Type, ConstructorInfo>();

		/// <summary>
		///     Initializes a new instance of the <see cref="RestModelMiddleware{TModel, TUser}" /> class
		/// </summary>
		/// <param name="options">The options to use for determining the route actions</param>
		public RestModelMiddleware(RestModelOptions<TModel, TUser> options) => this.Options = options;

		/// <summary>
		///     Handles an incoming request
		/// </summary>
		/// <param name="context">The http context of the current request</param>
		/// <returns>When the request is completed</returns>
		public async Task TryHandleRequest(HttpContext context) {
			await context.Response.WriteAsync("Hello! " + this.Options.RoutePattern);

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
				await this.HandleRequest(context);
			}
			catch (Exception e) {
				// foreach (Action<Exception> Action in this.Options.)
			}
		}

		/// <summary>
		///     Authenticates the request using the auth providers specified by this middleware's options, if any
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="parsed">The parsed model, or null if there are no body parsers specified</param>
		/// <returns>The authenticated user context, or null if there is none</returns>
		/// <exception cref="AuthFailedException">If none of the available authentication methods succeeded</exception>
		private async Task<TUser> Authenticate(HttpContext context, TModel parsed) {
			if (this.Options.AuthProviders == null) return null;

			TUser UserContext = null;
			bool AuthSuccess = false;
			foreach (IAuthProvider<TModel, TUser> Provider in this.Options.AuthProviders)
				try {
					UserContext = await Provider.AuthenticateAsync(context, parsed);
					AuthSuccess = true;
				}
				catch (AuthFailedException) {
					/* Just keep moving, by design */
				}

			// always throw on auth failed to not take that chance
			return AuthSuccess
				       ? UserContext
				       : throw new AuthFailedException(
					         "No authentication providers were able to authorize the request");
		}

		/// <summary>
		///     Handles an incoming request
		/// </summary>
		/// <param name="context">The http context of the current request</param>
		/// <returns>When the request is completed</returns>
		private async Task HandleRequest(HttpContext context) {
			/**
			 *		- PARSE body if there are body parsers present
			   |		- then parse the request body, and pass that along
			   |	- AUTH if there are auth providers present
			   |		- then ask them what the deal is and either pass one or all
			   |    - QUERY the dataset with our parsed data and auth
			   |	- FILTER the dbset by the filters provided
			   |	- CHECK if any conditions need to be met
			   |	- MODIFY data if there are any IOperations
			   |	- RETURN formatted output
			 
				note that the only required element should be the result formatter
				(even an empty result formatter is required to return a blank result)
			*/
			IServiceProvider Provider = context.RequestServices;

			// go through our body parsers to try and parse the request body. may be null
			TModel ParsedModel = await this.ParseBody(context);

			// then authenticate the request. may return null
			TUser User = await this.Authenticate(context, ParsedModel);

			// query the dataset for models
			IQueryable<TModel> Dataset = this.Options.ModelProvider != null
				                             ? await this.Options.ModelProvider.GetModelsAsync(
					                               context,
					                               ParsedModel,
					                               User)
				                             : null;

			// filter our dataset
			if (this.Options.Filters.Count > 0 && Dataset == null)
				throw new RequestFailedException("Attempt to filter dataset but no model provider was given");
			foreach (IFilter<TModel, TUser> Filter in this.Options.Filters)
				Dataset = await Filter.FilterDataAsync(context, Dataset, ParsedModel, User);

			// ensure it passes all conditions
			await this.VerifyConditions(context, Dataset, ParsedModel, User);

			// then do our operation
			IEnumerable<TModel> Result = this.Options.Operation != null
				                             ? await this.Options.Operation.OperateAsync(
					                               context,
					                               Dataset,
					                               ParsedModel,
					                               User)
				                             : Dataset;

			// and format the result
			await this.Options.ResultFormatter.FormatResultAsync(
				context,
				context.Response.Body,
				Result,
				User,
				this.Options.FormattingOptions);
		}

		/// <summary>
		///     Parses the request body using the body parsers specified by this middleware's options, if any
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <returns>The parsed model, or null if there are no body parsers specified</returns>
		/// <exception cref="InvalidParserException">If none of the available parsers can properly parse the request</exception>
		private async Task<TModel> ParseBody(HttpContext context) {
			if (this.Options.BodyParsers == null) return null;

			await using MemoryStream Stream = new MemoryStream();
			await context.Request.Body.CopyToAsync(Stream);
			byte[] BodyContents = Stream.ToArray();
			TModel Parsed = null;
			bool ParseSuccess = false;
			foreach (IBodyParser<TModel> Parser in this.Options.BodyParsers)
				try {
					Parsed = Parser.Parse(BodyContents, this.Options.ParserOptions, context.Request);
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
		/// <param name="context">The current request context</param>
		/// <param name="dataset">The dataset to verify conditions for</param>
		/// <param name="parsed">The parsed model, if any</param>
		/// <param name="user">The authenticated user context, if any</param>
		/// <returns>When all conditions have passed</returns>
		/// <exception cref="ConditionFailedException">If any conditions fail</exception>
		private async Task VerifyConditions(
			HttpContext context,
			IQueryable<TModel> dataset,
			TModel parsed,
			TUser user) {
			foreach (ICondition<TModel, TUser> Condition in this.Options.Conditions)
				try {
					if (!await Condition.VerifyAsync(context, dataset, parsed, user))
						throw new ConditionFailedException(Condition.FailureMessage ?? "Condition was not met");
				}
				catch (Exception e) when (!(e is ConditionFailedException)) {
					// wrap everything in a condition failed exception
					throw new ConditionFailedException("Condition was not met", e);
				}
		}

		private T Resolve<T>(Type type, IServiceProvider provider) {
			// todo: make faster
			ConstructorInfo Info;
			if (this.Constructors.ContainsKey(type)) Info = this.Constructors[type];
			else {
				Info = type.GetConstructors()[0];
				this.Constructors[type] = Info;
			}

			ParameterInfo[] Parameters = Info.GetParameters();
			return (T)Info.Invoke(Parameters.Select(p => provider.GetService(p.ParameterType)).ToArray());
		}
	}
}