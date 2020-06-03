// -----------------------------------------------------------------------
// <copyright file="Response.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Responses {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using RestModels.Filters;
	using RestModels.Responses.Attributes;

	/// <summary>
	///     An API response
	/// </summary>
	/// <typeparam name="TModel">The model that the response contains</typeparam>
	public class Response<TModel>
		where TModel : class {
		/// <summary>
		///     All of the properties on this <see cref="Response{TModel}" /> that have been set
		/// </summary>
		private readonly HashSet<PropertyInfo> SetProperties = new HashSet<PropertyInfo>();

		/// <summary>
		///     All of the public, writable properties on this <see cref="Response{TModel}" />
		/// </summary>
		private readonly PropertyInfo[] Properties;

		/// <summary>Initializes a new instance of the <see cref="Response{TModel}" /> class.</summary>
		public Response() {
			this.Properties = this.GetType().GetProperties().Where(p => p.CanWrite).ToArray();
		}

		/// <summary>
		///		Gets all of the properties on this <see cref="Response{TModel}" /> that should be included in the serialized body
		/// </summary>
		/// <returns> All of the properties on this <see cref="Response{TModel}" /> that should be included in the serialized body</returns>
		public PropertyInfo[] GetIncludedProperties() => this.SetProperties.ToArray();

		/// <summary>
		///		Gets all of the properties on this <see cref="Response{TModel}" /> that should not be included in the serialized body
		/// </summary>
		/// <returns> All of the properties on this <see cref="Response{TModel}" /> that should not be included in the serialized body</returns>
		public PropertyInfo[] GetOmittedProperties() => this.Properties.Except(this.SetProperties).ToArray();

		/// <summary>
		///		Sets the value of all properties on this <see cref="Response{TModel}"/> that have the given attribute applied to them. If no such properties exist, no action will occur.
		/// </summary>
		/// <typeparam name="TAttribute">The attribute to match on the properties to set</typeparam>
		/// <param name="value">The value to assign to that property</param>
		public void Set<TAttribute>(object value) where TAttribute : Attribute {
			PropertyInfo[] Matching = this.Properties.Where(p => p.GetCustomAttribute<TAttribute>(false) != null).ToArray();
			foreach (PropertyInfo ToSet in Matching) {
				ToSet.GetSetMethod()?.Invoke(this, new[] { value });
				this.SetProperties.Add(ToSet);
			}
		}

		/// <summary>
		///		Sets the value of all properties on this <see cref="Response{TModel}"/> that have the <see cref="ResponseValueAttribute"/> with the given name applied to them. If no such properties exist, no action will occur.
		/// </summary>
		/// <param name="name">The name given to the <see cref="ResponseValueAttribute"/> on the properties to set</param>
		/// <param name="value">The value to assign to that property</param>
		public void Set(string name, object value) {
			if (name == null) throw new ArgumentNullException(nameof(name));
			PropertyInfo[] Matching = this.Properties.Where(p => p.GetCustomAttribute<ResponseValueAttribute>()?.Name == name).ToArray();
			foreach (PropertyInfo ToSet in Matching) {
				ToSet.GetSetMethod()?.Invoke(this, new[] { value });
				this.SetProperties.Add(ToSet);
			}
		}

		/// <summary>
		///		Sets the value of all properties on this <see cref="Response{TModel}"/> that have the <see cref="ResponseValueAttribute"/> with the given name applied to them. If no such properties exist, no action will occur.
		/// </summary>
		/// <param name="name">The name given to the <see cref="ResponseValueAttribute"/> on the properties to set</param>
		/// <param name="value">The string value to assign to that property</param>
		/// <remarks>
		///		This method will attempt to convert the string value to a supported type if the type of the matching property is not string.
		/// </remarks>
		public void SetString(string name, string value) {
			if (name == null) throw new ArgumentNullException(nameof(name));
			PropertyInfo[] Matching = this.Properties.Where(p => p.GetCustomAttribute<ResponseValueAttribute>()?.Name == name).ToArray();
			foreach (PropertyInfo ToSet in Matching) {
				ToSet.GetSetMethod()?.Invoke(
					this,
					new[] { ParameterResolver.ParseParameter(value, ToSet.PropertyType) });
				this.SetProperties.Add(ToSet);
			}
		}

		/// <summary>
		///		Sets the value of all properties on this <see cref="Response{TModel}"/> that have the given attribute applied to them. If no such properties exist, no action will occur.
		/// </summary>
		/// <typeparam name="TAttribute">The attribute to match on the properties to set</typeparam>
		/// <param name="value">The string value to assign to that property</param>
		/// <remarks>
		///		This method will attempt to convert the string value to a supported type if the type of the matching property is not string.
		/// </remarks>
		public void SetString<TAttribute>(string value) where TAttribute : Attribute {
			PropertyInfo[] Matching = this.Properties.Where(p => p.GetCustomAttribute<TAttribute>(false) != null).ToArray();
			foreach (PropertyInfo ToSet in Matching) {
				ToSet.GetSetMethod()?.Invoke(
					this,
					new[] { ParameterResolver.ParseParameter(value, ToSet.PropertyType) });
				this.SetProperties.Add(ToSet);
			}
		}

		/// <summary>
		///		Populates the model property on this <see cref="Response{TModel}"/>
		/// </summary>
		/// <param name="models">The model dataset</param>
		/// <param name="shouldStrip"><c>true</c> to strip the array if there's only one element, <c>false</c> otherwise</param>
		public void Populate(TModel[] models, bool shouldStrip) {
			// todo: check for attributes like [thisisthesinglepropertyattribute]?
			PropertyInfo[] Properties = this.GetType().GetProperties();

			// either set the TModel Model property
			if (shouldStrip && models.Length == 1) {
				PropertyInfo SingleProperty = Properties.First(p => p.CanWrite && p.PropertyType == typeof(TModel));
				SingleProperty.GetSetMethod()?.Invoke(this, new object[] { models[0] });
				this.SetProperties.Add(SingleProperty);
				return;
			}

			// or the TModel[] Models property
			PropertyInfo ArrayProperty = Properties.First(p => p.CanWrite && p.PropertyType == typeof(TModel[]));
			ArrayProperty.GetSetMethod()?.Invoke(this, new object[] { models });
			this.SetProperties.Add(ArrayProperty);
		}
	}
}