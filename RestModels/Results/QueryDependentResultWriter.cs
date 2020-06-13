// -----------------------------------------------------------------------
// <copyright file="QueryDependentResultWriter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using Microsoft.AspNetCore.Http;

	/// <summary>
	///     A <see cref="RequestDependentResultWriter{TModel, TUser}" /> that uses a query parameter to determine which result
	///     writer to use
	/// </summary>
	/// <typeparam name="TModel">The type of model to format</typeparam>
	/// <typeparam name="TUser">The type of user context</typeparam>
	public class QueryDependentResultWriter<TModel, TUser> : RequestDependentResultWriter<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     The name of the query parameter to use to determine the result writer for the request
		/// </summary>
		private readonly string ParameterName;

		/// <summary>
		///     Initializes a new instance of the <see cref="QueryDependentResultWriter{TModel, TUser}" /> class.
		/// </summary>
		/// <param name="paramName">The name of the query parameter to use to determine the result writer for the request</param>
		/// <param name="values">The values of the query parameter that should determine which result writer to use</param>
		/// <param name="writers">The result writers to use, indexed in the same order as <paramref name="values" /></param>
		/// <param name="defaultIndex">
		///     The index of the default result writer to use, or -1 if an error should be thrown if no
		///     values match
		/// </param>
		/// <param name="caseSensitive"><see langword="true"/> if the parameter values are case sensitive, <see langword="false"/> otherwise</param>
		public QueryDependentResultWriter(
			string paramName,
			string[] values,
			IResultWriter<TModel, TUser>[] writers,
			int defaultIndex = -1,
			bool caseSensitive = false)
			: base(values, writers, defaultIndex, caseSensitive) =>
			this.ParameterName = paramName;

		/// <summary>
		///     Gets the value of the query parameter this <see cref="RequestDependentResultWriter{TModel, TUser}" /> switches on
		/// </summary>
		/// <param name="request">The request context to use to get the parameter value</param>
		/// <returns>The value of the query parameter to switch on</returns>
		protected override string GetRequestParameterValue(HttpRequest request) => request.Query[this.ParameterName];
	}
}