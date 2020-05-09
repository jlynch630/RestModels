using System;
using System.Collections.Generic;
using System.Text;

namespace RestModels.Responses {
	public class BasicResponse<TModel> : Response<TModel> where TModel : class {
		public TModel[]? Elements { get; set; }
		public TModel? Element { get; set; }
	}
}
