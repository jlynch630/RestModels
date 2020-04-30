app.UseRestModels<TestModel, object>(
				"/",
				options => {
					// Create: /
					// Read: /
					// Read: /{id}
					// Update: /{id}
					// Delete: /{id}
					options.UseResultWriter(new XmlResultWriter<TestModel>())
						.UseModelProvider(new EntityFrameworkModelProvider<TestModel, TestDbContext>())
						.StripArrayIfSingleResult().AddExceptionHandler(new SimpleExceptionHandler())
						.AddBodyParser(new JsonBodyParser<TestModel>())
						.MapRoute(
							string.Empty,
							opts => {
								opts.AddRequestMethod("GET").AddFilter(
									new DelegateFilter<TestModel, object>(async (c, s, p, q) => s.Take(2)));
							},
							null)
						.AddAuthProvider(
							new QueryAuthProvider<TestModel, object>(
								"key",
								async key => key == "john" ? 5 : throw new AuthFailedException("wrong creds")))
						.AddAuthProvider(
							new HeaderAuthProvider<TestModel, object>(
								"X-Api-Key",
								async key => key == "john" ? 5 : throw new AuthFailedException("wrong creds")))
						.AddAuthProvider(
							new BasicAuthProvider<TestModel, object>(
								async (username, password) =>
									username == "john" && password == "lynch"
										? 5
										: throw new AuthFailedException("wrong creds")))
						.MapRoute(
							string.Empty,
							opts => {
								opts.AddRequestMethod("POST").AddBodyParser(new XmlBodyParser<TestModel>())
									.AddBodyParser(new JsonBodyParser<TestModel>()).AcceptArrays().Ignore(p => p.Id)
									.Require(p => p.PrivateKey)
									.UseOperation(new CreateOperation<TestModel, TestDbContext>()).OmitAll()
									.Include(p => p.Id);
							},
							null)
						.MapRoute(
							"{id}",
							opts => {
								opts.AddRequestMethod("GET").AddExceptionHandler(new AuthFailedExceptionHandler())
									.AddFilter(
										new PrimaryKeyFilter<TestModel, TestDbContext>(
											new[] { "id" },
											(r, p) => r.RouteValues[p]));
							},
							null).MapRoute(
							"{id}",
							opts => {
								opts.AddRequestMethod("POST").AddBodyParser(new JsonBodyParser<TestModel>())
									.AddExceptionHandler(new AuthFailedExceptionHandler()).UseOperation(
										new UpdateOperation<TestModel, TestDbContext>(
											true,
											(c, p) => new object[] {
												                       int.Parse(c.Request.RouteValues["id"].ToString())
											                       }));
							},
							null).MapRoute(
							"{id}",
							opts => {
								opts.AddRequestMethod("DELETE").AddExceptionHandler(new AuthFailedExceptionHandler())
									.AddFilter(
										new DelegateFilter<TestModel, object>(
											async (c, d, p, u) => d.Where(
												m => m.Id == int.Parse(c.Request.RouteValues["id"].ToString()))))
									.UseOperation(new DeleteOperation<TestModel, TestDbContext>());
							},
							null).AddRequestMethod("GET");
				},
				null);