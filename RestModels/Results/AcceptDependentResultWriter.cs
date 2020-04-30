// -----------------------------------------------------------------------
// <copyright file="AcceptDependentResultWriter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using Microsoft.AspNetCore.Http;
	using Microsoft.Net.Http.Headers;
	using System.Linq;

	/// <summary>
	///     A <see cref="RequestDependentResultWriter{TModel, TUser}" /> that uses the Accept header to determine which result
	///     writer to use
	/// </summary>
	/// <typeparam name="TModel">The type of model to format</typeparam>
	/// <typeparam name="TUser">The type of user context</typeparam>
	public class AcceptDependentResultWriter<TModel, TUser> : RequestDependentResultWriter<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Initializes a new instance of the <see cref="AcceptDependentResultWriter{TModel,TUser}" /> class.
		/// </summary>
		/// <param name="mimeTypes">The mime types that should determine which result writer to use</param>
		/// <param name="writers">The result writers to use, indexed in the same order as <paramref name="mimeTypes" /></param>
		/// <param name="defaultIndex">
		///     The index of the default result writer to use, or -1 if an error should be thrown if no
		///     values match
		/// </param>
		public AcceptDependentResultWriter(
			string[] mimeTypes,
			IResultWriter<TModel, TUser>[] writers,
			int defaultIndex = -1)
			: base(mimeTypes, writers, defaultIndex, false) { }

		/// <summary>
		///     Gets the first usable mime-type in the Accept header
		/// </summary>
		/// <param name="request">The request context to use to get the value</param>
		/// <returns>The mime-type to use to write a result</returns>
		protected override string GetRequestParameterValue(HttpRequest request) {
			string Value = request.Headers[HeaderNames.Accept];

			if (string.IsNullOrWhiteSpace(Value)) return null;

			/*text/plain; q=0.5, text/html,
               text/x-dvi; q=0.8, text/x-c
				Verbally, this would be interpreted as "text/html and text/x-c are the preferred media types, 
				but if they do not exist, then send the text/x-dvi entity, and if that does not exist, 
				send the text/plain entity."*/
			// todo: this
			////Value.Split(',').Select(p => p.Trim().Split)
			return "";
		}
	}
}