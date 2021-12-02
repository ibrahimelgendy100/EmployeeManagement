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
			
		}

		[Route("{id?}")]
		public ViewResult Details(int?id) //jsonResult to objectResult
		{
			HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel() {
				Employee = _employeeRepository.GetEmployee(id??1),
				PageTitle="Employee Details"
			};
			return View(homeDetailsViewModel);
		}

		[HttpGet]
		public ViewResult Create()
		{
			return View();
		}
		[HttpPost]
		public ActionResult Create(Employee employee) 
		{
			//if (string.IsNullOrEmpty(employee.Name.Trim())||string.IsNullOrEmpty(employee.Email.Trim()))
			//{
			//	ViewBag.Error = "Please Fill All Information About Employee Record";
			//	return View();
			//}
			if (ModelState.IsValid)
			{
				Employee newEmployee = _employeeRepository.Add(employee);
				return RedirectToAction("details", new { id = newEmployee.Id });
			}
			return View();
		}

	}
}
