app.UseEntityFrameworkRestModels<TestModel, TestDbContext>(options => {
					options
						.WriteXml()
						.StripArrayIfSingleResult()
						.CatchExceptions()
						.ParseJson()
						.SetupAnonymousGet(o => o.Limit(2))
						.AuthQuery("key", "john")
						.AuthHeader("X-Api-Key", "john")
						.AuthBasic("john", "lynch")
						.PostCreate(o => o
							.ParseXmlAndJsonArrays()
							.IgnorePrimaryKey()
							.RequireProperty(p => p.PrivateKey)
							.IncludePrimaryKey())
						.GetByPrimaryKey()
						.PostUpdateByPrimaryKey()
						.DeleteByPrimaryKey()
						.Get();
});