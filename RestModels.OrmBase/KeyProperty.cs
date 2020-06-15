// -----------------------------------------------------------------------
// <copyright file="KeyProperty.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.OrmBase {
	using System.Reflection;

	/// <summary>
	///     A property that makes up a part of a PrimaryKey
	/// </summary>
	public class KeyProperty {
		/// <summary>
		///     Initializes a new instance of the <see cref="KeyProperty" /> class, with the name set to the name of the given
		///     <see cref="PropertyInfo" />
		/// </summary>
		/// <param name="propertyInfo">The key property</param>
		public KeyProperty(PropertyInfo propertyInfo)
			: this(propertyInfo.Name, propertyInfo) { }

		/// <summary>
		///     Initializes a new instance of the <see cref="KeyProperty" /> class.
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <param name="propertyInfo">The key property</param>
		public KeyProperty(string name, PropertyInfo propertyInfo) {
			this.Name = name;
			this.PropertyInfo = propertyInfo;
		}

		/// <summary>
		///     Gets the name of the key property
		/// </summary>
		public string Name { get; }

		/// <summary>
		///     Gets the key property
		/// </summary>
		public PropertyInfo PropertyInfo { get; }
	}
}