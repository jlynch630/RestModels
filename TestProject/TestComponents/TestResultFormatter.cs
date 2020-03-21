// -----------------------------------------------------------------------
// <copyright file="TestResultFormatter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace TestProject.TestComponents {
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;

	using RestModels.Options;
	using RestModels.Results;

	public class TestResultFormatter : IResultWriter<object, object> {
		public async Task WriteResultAsync(
			HttpContext context,
			IEnumerable<object> data,
			object user,
			FormattingOptions options) {
			await context.Response.WriteAsync("Everything's workin!");
		}
	}
}