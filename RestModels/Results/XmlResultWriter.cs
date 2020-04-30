// -----------------------------------------------------------------------
// <copyright file="XmlResultWriter.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Results {
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;
	using System.Xml.Serialization;

	using Microsoft.AspNetCore.Http;
	using Microsoft.Net.Http.Headers;

	using RestModels.Options;

	/// <summary>
	///     Result writer that formats data in an XML format using System.Xml.Serialization
	/// </summary>
	/// <typeparam name="TModel">The type of serialized model</typeparam>
	public class XmlResultWriter<TModel> : IResultWriter<TModel>
		where TModel : class {
		/// <summary>
		///     A list of all of the public properties on <typeparamref cref="TModel" />
		/// </summary>
		private readonly PropertyInfo[] Properties;

		/// <summary>
		///     Initializes a new instance of the <see cref="XmlResultWriter{TModel}" /> class
		/// </summary>
		public XmlResultWriter() => this.Properties = typeof(TModel).GetProperties();

		/// <summary>
		///		Gets whether or not this <see cref="IResultWriter{TModel, TUser}"/> can write a result for the given request
		/// </summary>
		/// <param name="request">The request to test if a result can be written for it</param>
		/// <returns><code>true</code></returns>
		public async Task<bool> CanWriteAsync(HttpRequest request) => true;

		/// <summary>
		///     Formats the API result
		/// </summary>
		/// <param name="context">The current request context</param>
		/// <param name="data">The dataset to format</param>
		/// <param name="user">The current authenticated user context</param>
		/// <param name="options">Options for formatting the result</param>
		/// <returns>When the result has been sent</returns>
		public async Task WriteResultAsync(
			HttpContext context,
			IEnumerable<TModel> data,
			object user,
			FormattingOptions options) {
			// set content type first, then actually write the xml
			context.Response.ContentType = "application/xml";

			if (data == null) {
				await context.Response.WriteAsync("null");
				return;
			}

			TModel[] FullDataset = data.ToArray();

			bool ReturnObject = options.StripArrayIfSingleElement && FullDataset.Length == 1;
			XmlAttributeOverrides Overrides = new XmlAttributeOverrides();

			if (options.IncludedReturnProperties != null) {
				IEnumerable<PropertyInfo> OmittedProperties = this.Properties.Except(options.IncludedReturnProperties);
				foreach (PropertyInfo OmittedProperty in OmittedProperties) {
					XmlAttributes IgnoreAttributes = new XmlAttributes { XmlIgnore = true };
					IgnoreAttributes.XmlElements.Add(new XmlElementAttribute(OmittedProperty.Name));
					Overrides.Add(OmittedProperty.DeclaringType, OmittedProperty.Name, IgnoreAttributes);
				}
			}

			// cannot use stream as synchronous io is disallowed
			await using StringWriter Writer = new StringWriter();
			
			if (ReturnObject)
				new XmlSerializer(typeof(TModel), Overrides).Serialize(Writer, FullDataset[0]);
			else new XmlSerializer(typeof(TModel[]), Overrides).Serialize(Writer, FullDataset);

			await context.Response.WriteAsync(Writer.ToString());
		}
	}
}