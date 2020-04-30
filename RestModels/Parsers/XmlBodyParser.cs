// -----------------------------------------------------------------------
// <copyright file="XmlBodyParser.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Parsers {
	using System.IO;
	using System.Threading.Tasks;
	using System.Xml;
	using System.Xml.Serialization;
	using Microsoft.AspNetCore.Http;

	using RestModels.Exceptions;
	using RestModels.Options;

	/// <summary>
	///     Parser for an XML request body
	/// </summary>
	/// <typeparam name="TModel">The type to parse to</typeparam>
	public class XmlBodyParser<TModel> : IBodyParser<TModel>
		where TModel : class {
		/// <summary>
		///     Parses an XML request body
		/// </summary>
		/// <param name="body">The data of the request body</param>
		/// <param name="options">Options for the parser</param>
		/// <param name="context">The context for the HTTP request</param>
		/// <returns>The parsed models</returns>
		public async Task<ParseResult<TModel>[]> Parse(byte[] body, ParserOptions options, HttpContext context) {
		
			/*
			options.DefaultPropertyValues
				options.IgnoredParseProperties
					options.RequiredParseProperties
						options.ParseArrays
						*/

			////await using MemoryStream Stream = new MemoryStream(body);
			////return new[] { new ParseResult(new XmlSerializer(typeof(TModel)).Deserialize(Stream), null) };
			/*
			Stream.Seek(0, SeekOrigin.Begin);
			using XmlReader Reader = XmlReader.Create(Stream);
			Reader.MoveToContent();
			*/
			return null;
		}

		/// <summary>
		///		Gets whether or not the request body can be parsed by this <see cref="IBodyParser{TModel}"/>
		/// </summary>
		/// <param name="context">The context for the HTTP request</param>
		/// <returns><c>true</c> if the request body can be parsed by this <see cref="IBodyParser{TModel}"/>, <c>false</c> otherwise</returns>
		public async Task<bool> CanParse(HttpContext context) => context.Request.ContentType == "application/xml";
	}
}