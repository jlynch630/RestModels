// -----------------------------------------------------------------------
// <copyright file="TestModel.cs" company="John Lynch">
//   This file is licensed under the MIT license
//   Copyright (c) 2020 John Lynch
// </copyright>
// -----------------------------------------------------------------------

namespace TestProject.TestComponents {
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class TestModel {
		public Guid FancyType { get; set; } = Guid.NewGuid();

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }

		public string PrivateKey { get; set; }
	}
}