// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace TestProject
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using RestModels.Auth;
    using RestModels.EntityFramework;
    using RestModels.ExceptionHandlers;
    using RestModels.Exceptions;
    using RestModels.Extensions;
    using RestModels.Filters;
    using RestModels.EntityFramework.Filters;
    using RestModels.Models;
    using RestModels.EntityFramework.Operations;
    using RestModels.Parsers;
    using RestModels.Results;

    using TestProject.TestComponents;
    using RestModels.EntityFramework.Extensions;
    using RestModels.Responses;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEntityFrameworkRestModels<TestModel, TestDbContext>(
                "/v1",
                options =>
                {
                    // Create: /
                    // Read: /
                    // Read: /{id}
                    // Update: /{id}
                    // Delete: /{id}
                    /*
					options.WriteJsonOrXml()
						.StripArrayIfSingleResult()
						.CatchExceptions()
						.ParseXmlAndJsonArrays()
						.AuthQuery("key", "john")
						.AuthHeader("X-Api-Key", "john")
						.AuthBasic("john", "lynch")
						.Get(opts => { opts.Paginate(); })
						.SetupAnonymousGet(opts => { opts.WriteString("Hello and welcome!"); })
						.PostCreate(opts => {
								opts.RequireProperty(p => p.PrivateKey)
									.Default(p => p.FancyType, Guid.NewGuid)
									.Default(p => p.Name, "Name")
									.IncludePrimaryKey();
						})
						.GetByPrimaryKey()
						.PostUpdateByPrimaryKey()
						.DeleteByPrimaryKey();*/
                    options
                        .WriteXml()
                        .WrapResponse()
                        .WriteResponseCount()
                        .StripArrayIfSingleResult()
                        .CatchExceptions()
                        .ParseJson()
                        .Deprecate("Upgrade to the newest v2 deLUX version of this api today!")
                        .WriteVersion("400.thousand.build.5")
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
                        .Get(o => o.Paginate());
                });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TestDbContext>(options => { options.UseInMemoryDatabase("test"); });
        }
    }
}