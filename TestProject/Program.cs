using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestProject.TestComponents;

namespace TestProject {
	public class Program {
		public static void Main(string[] args) {
			IHost Host = CreateHostBuilder(args).Build();
			using IServiceScope Scope = Host.Services.CreateScope();
			IServiceProvider Provider = Scope.ServiceProvider;
			TestDbContext Context = Provider.GetRequiredService<TestDbContext>();
			Context.Database.EnsureCreated();
			Context.Models.Add(new TestModel { Name = "One", PrivateKey = "1234" });
			Context.Models.Add(new TestModel { Name = "Deux", PrivateKey = "4321" });
			Context.Models.Add(new TestModel { Name = "Tres", PrivateKey = "abcd" });
			Context.Models.AddRange(Enumerable.Repeat(new TestModel { Name = "Three", PrivateKey = "abcd" }, 50));
			Context.SaveChanges();
			Host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => {
					webBuilder.UseStartup<Startup>();
				});
	}
}
