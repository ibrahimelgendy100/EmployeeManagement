using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models.EmployeeRepository
{
	public class EmployeeRepository : IEmployeeRepository
	{
		private List<Employee> _employeeList;

		public EmployeeRepository()
		{
			_employeeList = new List<Employee>()
			{
				new Employee(){Id=1, Name="ibrahim",Email="test1@test.com",Department=Dept.HR},
				new Employee(){Id=2, Name="Ali",Email="test2@test.com",Department=Dept.IT},
				new Employee(){Id=3, Name="Ahmed",Email="test3@test.com",Department=Dept.IT}
			};
		}

		public Employee Add(Employee employee)
		{
			employee.Id = _employeeList.Max(e => e.Id) + 1;
			_employeeList.Add(employee);
			return employee;
		}

		public Employee Delete(int id)
		{
			Employee employee = _employeeList.FirstOrDefault(e => e.Id == id);
			if (employee !=null)
			{
				_employeeList.Remove(employee);
			}
			return employee;
		}

		public IEnumerable<Employee> GetAllEmployees()
		{
			return _employeeList;
		}

		public  Employee GetEmployee(int id)
		{
			return  _employeeList.FirstOrDefault(e => e.Id == id);
		}

		public Employee Update(Employee employee)
		{
			Employee empChange = _employeeList.FirstOrDefault(e => e.Id == employee.Id);
			if (empChange !=null)
			{
				employee.Name = empChange.Name;
				employee.Department = empChange.Department;
				employee.Email = empChange.Email;
			}
			return empChange; 
		}
	}
}
