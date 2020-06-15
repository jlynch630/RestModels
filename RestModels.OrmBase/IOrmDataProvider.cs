using RestModels.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestModels.OrmBase {
	using System.Reflection;

	using RestModels.Context;
	using RestModels.Options.Builder;
	using RestModels.ParameterRetrievers;

	public interface IOrmDataProvider<TModel, TUser> where TModel : class where TUser : class {
		IQueryable<TModel> GetModels(IApiContext<TModel, object> context);

		List<KeyProperty> GetPrimaryKey();

		IOperation<TModel, TUser> GetCreateOperation();

		IOperation<TModel, TUser> GetUpdateOperation(PropertyInfo[] properties, ParameterRetriever[]? retrievers);

		IOperation<TModel, TUser> GetDeleteOperation();
	}
}
