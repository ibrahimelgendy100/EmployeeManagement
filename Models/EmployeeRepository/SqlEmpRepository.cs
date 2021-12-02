using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EmployeeManagement.Models.EmployeeRepository
{
	public class SqlEmpRepository : IEmployeeRepository
	{
		private readonly DataContext _context;

		public SqlEmpRepository(DataContext context)
		{
			_context = context;
		}

		public Employee Add(Employee employee)
		{
			_context.Employees.Add(employee);
			_context.SaveChanges();
			return employee;

		}

		public Employee Delete(int id)
		{
			Employee employee = _context.Employees.Find(id);
			if (employee !=null)
			{
				_context.Employees.Remove(employee);
				_context.SaveChanges();
			}
			return employee;
		}

		public IEnumerable<Employee> GetAllEmployees()
		{
			return _context.Employees;
		}

		public Employee GetEmployee(int id)
		{
			return _context.Employees.Find(id);
		}

		public Employee Update(Employee employee)
		{
			var empChange = _context.Employees.Attach(employee);
			empChange.State = EntityState.Modified;
			_context.SaveChanges();
			return employee;
		}
	}
}
