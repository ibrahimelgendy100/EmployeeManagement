using EmployeeManagement.Models;
using EmployeeManagement.Models.EmployeeRepository;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
	[Route("[Controller]/[action]")]
	public class HomeController:Controller
	{
		private readonly IEmployeeRepository _employeeRepository;

		public HomeController(IEmployeeRepository employeeRepository)
		{
			_employeeRepository = employeeRepository;
		}

		[Route("~/")]
		[Route("~/Home")]
		public ViewResult Index()
		{
			var employees = _employeeRepository.GetAllEmployees();
			return View(employees);
			//return _employeeRepository.GetEmployee(1).Name;
		}

		[Route("{id?}")]
		public ViewResult Details(int?id) //jsonResult to objectResult
		{
			HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel() {
				Employee = _employeeRepository.GetEmployee(id??1),
				PageTitle="Emp Details"
			};
			
			//Employee employee = _employeeRepository.GetEmployee(1);
			//return new ObjectResult(employee);
			//return View(employee);
			//ViewData["Emp"] = employee;
			//ViewData["PageTitle"] = "Employee Details";
			//ViewBag.Emp = employee;
			//ViewBag.PageTitle = "Employee Details";
			return View(homeDetailsViewModel);
		}
	}
}
