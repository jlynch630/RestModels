// -----------------------------------------------------------------------
// <copyright file="EntityFrameworkRestModelOptionsBuilder.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.EntityFramework.Options {
	using System;
	using RestModels.Options.Builder;

	/// <summary>The entity framework rest model options builder.</summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public class
		EntityFrameworkRestModelOptionsBuilder<TModel, TUser> : RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///		Initializes a new instance of the <see cref="EntityFrameworkRestModelOptionsBuilder{TModel,TUser}"/> class.
		/// </summary>
		/// <param name="contextType">The type of the database context to get the set of <see cref="TModel"/> objects from</param>
		internal EntityFrameworkRestModelOptionsBuilder(Type contextType) {
			this.ContextType = contextType;
		}

		/// <summary>
		///     Gets the type of the database context to get the set of <see cref="TModel"/> objects from
		/// </summary>
		internal Type ContextType { get; }
	}
}