// -----------------------------------------------------------------------
// <copyright file="RestModelOptionsBuilder.Filters.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace RestModels.Options.Builder {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Http;
	using RestModels.Exceptions;
	using RestModels.Filters;
	using RestModels.ParameterRetrievers;
	using RestModels.Parsers;

	/// <summary>
	///     Builder for <see cref="RestModelOptions{TModel, TUser}" />
	/// </summary>
	/// <typeparam name="TModel">The model type that the API is being built for</typeparam>
	/// <typeparam name="TUser">The type of authenticated user context</typeparam>
	public partial class RestModelOptionsBuilder<TModel, TUser>
		where TModel : class where TUser : class {
		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> FilterAsync(Func<HttpContext, IQueryable<TModel>, ParseResult<TModel>[], TUser, Task<IQueryable<TModel>>> filter) {
			this.AddFilter(new DelegateFilter<TModel, TUser>(filter));
			return this;
		}

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Filter(Func<HttpContext, IQueryable<TModel>, ParseResult<TModel>[], TUser, IQueryable<TModel>> filter) {
			this.AddFilter(new DelegateFilter<TModel, TUser>(async (c, d, p, u) => filter(c, d, p, u)));
			return this;
		}

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> FilterAsync(Func<IQueryable<TModel>, Task<IQueryable<TModel>>> filter) {
			this.FilterAsync((c, d, p, u) => filter(d));
			return this;
		}

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Filter(Func<IQueryable<TModel>, IQueryable<TModel>> filter) {
			this.Filter((c, d, p, u) => filter(d));
			return this;
		}

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> FilterAsync(Func<HttpContext, IQueryable<TModel>, Task<IQueryable<TModel>>> filter) {
			this.FilterAsync((c, d, p, u) => filter(c, d));
			return this;
		}

		/// <summary>
		///     Adds a filter to this route that will filter the dataset to operate on using the given delegate
		/// </summary>
		/// <param name="filter">The delegate to use to filter results</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Filter(Func<HttpContext, IQueryable<TModel>, IQueryable<TModel>> filter) {
			this.Filter((c, d, p, u) => filter(c, d));
			return this;
		}

		/// <summary>
		///		Skips over a given number of elements in the dataset
		/// </summary>
		/// <param name="count">The number of elements in the dataset to skip over</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Skip(int count) => this.Filter(d => d.Skip(count));

		/// <summary>
		///     Limits this route's dataset to a certain number of elements
		/// </summary>
		/// <param name="count">The number of elements to limit the result to</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Limit(int count) => this.Filter(d => d.Take(count));

		/// <summary>
		///     Limits this route's dataset to a certain number of elements, as determined by a query parameter
		/// </summary>
		/// <param name="paramName">The name of the query parameter that determines the number of elements in the dataset</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> LimitQuery(string paramName) {
			this.Filter(
				(c, d) => {
					if (!c.Request.Query.ContainsKey(paramName)) return d;
					int Limit = Int32.Parse(c.Request.Query[paramName]);
					if (Limit < 0)
						throw new ArgumentOutOfRangeException(paramName, Limit, "Limit must be non-negative");
					return d.Take(Limit);
				});
			return this;
		}

		/// <summary>
		///     Skips a certain number of elements in this route's dataset, as determined by a query parameter
		/// </summary>
		/// <param name="paramName">The name of the query parameter that determines the number of elements in the dataset to skip</param>
		/// <param name="multiplier">The number to multiply the value of <paramref name="paramName"/> by before skipping, for pagination</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> SkipQuery(string paramName, int multiplier = 1) {
			this.Filter(
				(c, d) => {
					if (!c.Request.Query.ContainsKey(paramName)) return d;
					int Skip = Int32.Parse(c.Request.Query[paramName]);
					if (Skip < 0)
						throw new ArgumentOutOfRangeException(paramName, Skip, "Skip value must be non-negative");
					return d.Skip(Skip * multiplier);
				});
			return this;
		}

		/// <summary>
		///		Enables zero-indexed pagination of the API output using the "page" and "count" query parameters with infinite maximum page sizes
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Paginate() => this.Paginate("page", "count");

		/// <summary>
		///     Enables zero-indexed pagination of the API output with infinite maximum page sizes
		/// </summary>
		/// <param name="pageParamName">The name of the parameter that defines what page to return, starting from zero</param>
		/// <param name="countParamName">The name of the parameter that defines how many elements to return</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Paginate(string pageParamName, string countParamName) =>
			this.Paginate(pageParamName, countParamName, 0);

		/// <summary>
		///     Enables zero-indexed pagination of the API output
		/// </summary>
		/// <param name="pageParamName">The name of the parameter that defines what page to return, starting from zero</param>
		/// <param name="countParamName">The name of the parameter that defines how many elements to return</param>
		/// <param name="maxPageSize">The maximum number of elements to return in a page</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Paginate(string pageParamName, string countParamName, int maxPageSize) {
			if (maxPageSize < 0)
				throw new ArgumentOutOfRangeException(nameof(maxPageSize), maxPageSize, "Max page size must be non-negative. Use overload without parameter to allow infinite page sizes.");
			this.Filter(
				(c, d) => {
					int Page = 0;
					int PageCount = 0;
					if (c.Request.Query.ContainsKey(pageParamName))
						Page = Int32.Parse(c.Request.Query[pageParamName]);
					if (c.Request.Query.ContainsKey(countParamName))
						PageCount = Int32.Parse(c.Request.Query[countParamName]);
					if (Page < 0)
						throw new ArgumentOutOfRangeException(pageParamName, Page, "Page must be non-negative");
					if (PageCount < 0)
						throw new ArgumentOutOfRangeException(countParamName, PageCount, "Count must be non-negative");
					if (maxPageSize != 0 && PageCount > maxPageSize) PageCount = maxPageSize;
					if (maxPageSize == 0 && PageCount == 0) return d.Skip(Page * PageCount);
					return d.Skip(Page * PageCount).Take(PageCount);
				});
			return this;
		}

		/// <summary>
		///     Enables zero-indexed pagination of the API output
		/// </summary>
		/// <param name="pageParamName">The name of the query parameter that defines what page to return</param>
		/// <param name="pageSize">The number of elements to include in each page</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> Paginate(string pageParamName, int pageSize) {
			this.SkipQuery(pageParamName, pageSize);
			this.Limit(pageSize);
			return this;
		}


		/// <summary>
		///     Limits this route's dataset to a certain number of elements, as determined by a query parameter
		/// </summary>
		/// <param name="paramName">The name of the query parameter that determines the number of elements in the dataset</param>
		/// <param name="max">The maximum number of elements to return</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> LimitQuery(string paramName, int max) {
			this.LimitQuery(paramName);
			this.Limit(max);
			return this;
		}


		/// <summary>
		///     Limits this route's dataset to only one element
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> LimitOne() => this.Limit(1);

		/// <summary>
		///     Limits this route's dataset to only the first element, throwing if there are no elements in the set
		/// </summary>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> First() {
			this.Limit(1);
			this.RequireNonEmpty();
			return this;
		}

		/// <summary>
		///     Filters this route's dataset by a route parameter value
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="routeParameterName">The name of the route parameter whose value to compare with the value of <paramref name="property"/> on the dataset's <typeparamref name="TModel"/> objects</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> FilterByRouteEqual<T>(
			Expression<Func<TModel, T>> property,
			string routeParameterName) =>
			this.FilterByParameterEqual(property, new RouteValueRetriever(routeParameterName));

		/// <summary>
		///     Filters this route's dataset by a query parameter value
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="queryParameterName">The name of the query parameter whose value to compare with the value of <paramref name="property"/> on the dataset's <typeparamref name="TModel"/> objects</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> FilterByQueryEqual<T>(
			Expression<Func<TModel, T>> property,
			string queryParameterName) =>
			this.FilterByParameterEqual(property, new QueryParameterRetriever(queryParameterName));


		/// <summary>
		///     Filters this route's dataset by a parameter value
		/// </summary>
		/// <param name="property">An expression getting the property to filter for</param>
		/// <param name="retriever">A function that will get the value of the parameter for a given request</param>
		/// <typeparam name="T">The type of the parameter to filter with</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> FilterByParameterEqual<T>(Expression<Func<TModel, T>> property, ParameterRetriever retriever) {
			PropertyInfo Info = RestModelOptionsBuilder<TModel, TUser>.ExtractProperty(property);
			ParameterExpression ModelParameter = Expression.Parameter(typeof(TModel));
			MemberExpression PropertyExpression = Expression.Property(ModelParameter, Info);
			Type PropertyType = typeof(T);

			this.Filter(
				(c, d) => {
					string? ParamValue = retriever.GetValue(c.Request);
					if (ParamValue == null) throw new ConditionFailedException("Failed to parse request parameter");
					T Parsed = (T)ParameterResolver.ParseParameter(ParamValue, PropertyType);

					// ModelParameter.Property == ParamValue
					Expression ComparisonExpression = Expression.Equal(PropertyExpression, Expression.Constant(Parsed));

					// and create the lambda
					Expression<Func<TModel, bool>> FilterExpression =
						Expression.Lambda<Func<TModel, bool>>(ComparisonExpression, ModelParameter);
					return d.Where(FilterExpression);
				});
			return this;
		}

		/// <summary>
		///     Filters this route's dataset only including the elements that match the predicate
		/// </summary>
		/// <param name="predicate">The predicate to use to determine if an element should be included in the dataset or not</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> FilterWhere(Expression<Func<TModel, bool>> predicate) {
			this.Filter(d => d.Where(predicate));
			return this;
		}

		/// <summary>
		///     Filters this route's dataset only including the elements that match the predicate
		/// </summary>
		/// <param name="predicate">The predicate to use to determine if an element should be included in the dataset or not</param>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> FilterWhere(Func<HttpContext, TModel, bool> predicate) {
			this.Filter((c, d, p, u) => d.Where(m => predicate(c, m)));
			return this;
		}

		/// <summary>
		///     Orders this route's dataset in an ascending order using the given key selector
		/// </summary>
		/// <param name="predicate">The predicate to use to give they key to sort by</param>
		/// <typeparam name="TKey">The type of the key to sort by</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> OrderBy<TKey>(Expression<Func<TModel, TKey>> predicate) {
			this.Filter(d => d.OrderBy(predicate));
			return this;
		}

		/// <summary>
		///     Orders this route's dataset in an ascending order using the given key selector
		/// </summary>
		/// <param name="predicate">The predicate to use to give they key to sort by</param>
		/// <param name="comparer">The comparer to use to compare two elements in the dataset</param>
		/// <typeparam name="TKey">The type of the key to sort by</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> OrderBy<TKey>(Expression<Func<TModel, TKey>> predicate, IComparer<TKey> comparer) {
			this.Filter(d => d.OrderBy(predicate, comparer));
			return this;
		}

		/// <summary>
		///     Orders this route's dataset in an descending order using the given key selector
		/// </summary>
		/// <param name="predicate">The predicate to use to give they key to sort by</param>
		/// <typeparam name="TKey">The type of the key to sort by</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> OrderByDescending<TKey>(Expression<Func<TModel, TKey>> predicate) {
			this.Filter(d => d.OrderByDescending(predicate));
			return this;
		}

		/// <summary>
		///     Orders this route's dataset in an descending order using the given key selector
		/// </summary>
		/// <param name="predicate">The predicate to use to give they key to sort by</param>
		/// <param name="comparer">The comparer to use to compare two elements in the dataset</param>
		/// <typeparam name="TKey">The type of the key to sort by</typeparam>
		/// <returns>This <see cref="RestModelOptionsBuilder{TModel, TUser}" /> object, for chaining</returns>
		public RestModelOptionsBuilder<TModel, TUser> OrderByDescending<TKey>(Expression<Func<TModel, TKey>> predicate, IComparer<TKey> comparer) {
			this.Filter(d => d.OrderByDescending(predicate, comparer));
			return this;
		}
	}
}