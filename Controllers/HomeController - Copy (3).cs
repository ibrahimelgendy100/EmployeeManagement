using EmployeeManagement.Models;
using EmployeeManagement.Models.EmployeeRepository;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
	[Route("[Controller]/[action]")]
	public class HomeController:Controller
	{
		private readonly IEmployeeRepository _employeeRepository;
		
		private readonly IHostingEnvironment _hostingEnvironment;

		public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment)
		{
			_employeeRepository = employeeRepository;
			_hostingEnvironment = hostingEnvironment;
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
		public ActionResult Create(EmpCreateViewModel model) 
		{
			
			if (ModelState.IsValid)
			{
				string uniqueFileName = null;
				if (model.Photo !=null)
				{
					string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
					uniqueFileName = Guid.NewGuid().ToString()+"_"+ model.Photo.FileName;
					string filePath = Path.Combine(uploadFolder, uniqueFileName);
					model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
				}
				Employee newEmployee = new Employee
				{
					Name=model.Name,
					Email=model.Email,
					Department=model.Department,
					PhotoPath=uniqueFileName
				};
				_employeeRepository.Add(newEmployee);
				return RedirectToAction("details", new { id = newEmployee.Id });
			}
			return View();
		}

	}
}
