using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject.TestComponents {
	using Microsoft.EntityFrameworkCore;

	public class TestDbContext : DbContext {
		public TestDbContext(DbContextOptions options)
			: base(options) { }

		public DbSet<TestModel> Models { get; set; }
	}
}
