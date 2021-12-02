using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Extensions
{
	public static class ModelBuilderExtensions
	{
		public static void Seed(this ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Employee>().HasData(
				new Employee
				{
					Id = 1,
					Name = "ibrahim",
					Email = "test1@test.com",
					Department = Dept.HR
				},
				new Employee
				{
					Id = 2,
					Name = "Ali",
					Email = "test2@test.com",
					Department = Dept.IT
				},
				new Employee
				{
					Id = 3,
					Name = "Ahmed",
					Email = "test3@test.com",
					Department = Dept.IT
				}
				);
		}
	}
}
