// -----------------------------------------------------------------------
// <copyright file="ApiContext.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Context {
	using System;

	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection;

	using RestModels.Parsers;
	using RestModels.Responses;

	/// <summary>
	///     Context for an API request
	/// </summary>
	/// <typeparam name="TModel">The type of model being managed by the AP</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public class ApiContext<TModel, TUser> : IDisposable, IApiContext<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Gets the service scope for this request
		/// </summary>
		private readonly IServiceScope Scope;

		/// <summary>
		///     Initializes a new instance of the <see cref="ApiContext{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="httpContext">The current <see cref="HttpContext" /> for this request</param>
		/// <param name="response">The response for this API call</param>
		public ApiContext(HttpContext httpContext, Response<TModel>? response) {
			this.HttpContext = httpContext;
			this.Response = response;
			this.Scope = httpContext.RequestServices.CreateScope();
			this.Services = this.Scope.ServiceProvider;
		}

		/// <summary>
		///     Gets the current <see cref="HttpContext" /> for this request
		/// </summary>
		public HttpContext HttpContext { get; }

		/// <summary>
		///     Gets the current HTTP response context. Shortcut to <see cref="HttpContext.Response" />
		/// </summary>
		public HttpResponse HttpResponse => this.HttpContext.Response;

		/// <summary>
		///     Gets the models that have been parsed by the body parser. This may be null if the body has not been parsed yet or
		///     there are no body parsers registered for this route.
		/// </summary>
		public ParseResult<TModel>[]? Parsed { get; internal set; }

		/// <summary>
		///     Gets the current request context. Shortcut to <see cref="HttpContext.Request" />
		/// </summary>
		public HttpRequest Request => this.HttpContext.Request;

		/// <summary>
		///     Gets the response for this API call. If this is null, the model itself will be serialized instead.
		/// </summary>
		public Response<TModel>? Response { get; }

		/// <summary>
		///     Gets a service provider for this API context
		/// </summary>
		public IServiceProvider Services { get; }

		/// <summary>
		///     Gets the authenticated user context for this route. This may be null if authorization has not yet occurred or there
		///     are no auth providers registered for this route.
		/// </summary>
		public TUser? User { get; internal set; }

		/// <summary>
		///     Disposes the <see cref="ApiContext{TModel, TUser}" />
		/// </summary>
		public void Dispose() {
			this.Scope.Dispose();
		}
	}
}