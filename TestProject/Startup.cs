using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestProject {
	using Microsoft.AspNetCore.Routing;
	using Microsoft.AspNetCore.Routing.Patterns;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.AspNetCore.Http;
	using RestModels.Options;
	using RestModels.Results;

	using TestProject.TestComponents;
	using RestModels.Models;
	using RestModels.Filters;
	using Microsoft.Extensions.Options;
	using Microsoft.EntityFrameworkCore;
	using RestModels.Conditions;
	using RestModels.Parsers;
	using RestModels.ExceptionHandlers;
	using RestModels.Operations.EntityFramework;
	using RestModels.Auth;
	using RestModels.Exceptions;

	public class Startup {
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services) {
			services.AddDbContext<TestDbContext>(options => { options.UseInMemoryDatabase("test"); });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}
			app.UseRouting();
			app.UseEndpoints(endpoints => {
				endpoints.MapGet("/a", async context => {
					await context.Response.WriteAsync($"Hello World!");
				});
			});
			
			app.UseRestModels<TestModel, object>("/",
				options => {
					options
						.UseModelProvider(new EntityFrameworkModelProvider<TestModel, TestDbContext>())
						.AddRequestMethod("POST")
						.AddAuthProvider(new DelegateAuthProvider<TestModel, object>(async (c, m) => {
							if (c.Request.Query["key"] != "1234") throw new AuthFailedException("Wrong key");
							return null;
						}))
						.AddBodyParser(new JsonBodyParser<TestModel>())
						.AddFilter(new DelegateFilter<TestModel, object>(async (c, d, m, u) => d.Skip(1)))
						.AddFilter(new DelegateFilter<TestModel, object>(async (c, d, m, u) => d.Take(1)))
						.AddCondition(new DelegateCondition<TestModel, object>(async (c, d, m, u) => d.Count() == 1))
						.UseOperation(new CreateOperation<TestModel, TestDbContext>())
						.AddExceptionHandler(new DelegateExceptionHandler(
							async (e, c) => {
								await c.Response.WriteAsync(e.Message);
								c.Response.StatusCode = 500;
								return false;
							}))
						.UseResultWriter(new JsonResultWriter<TestModel>());
				}, null);
			/*
			app.UseRestModels<string>(
				options => {
					options.MapRoute(
						"/a",
						opts => {
							opts.MapRoute("1", opts2 => { }, null);
							opts.MapRoute("2", opts2 => { }, null);
						},
						null).MapRoute(
						"/b",
						opts => {
							opts.MapRoute("1", opts2 => { }, null);
							opts.MapRoute("2", opts2 => { }, null);
						},
						null);
				});*/


		}
	}
}
