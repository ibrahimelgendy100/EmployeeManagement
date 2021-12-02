using EmployeeManagement.Models;
using EmployeeManagement.Models.EmployeeRepository;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
			//throw new Exception("Error in Details View");
			Employee employee = _employeeRepository.GetEmployee(id.Value);
			if (employee ==  null)
			{
				Response.StatusCode = 404;
				return View("NotFoundError", id.Value);
			}
			HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel() {
				//Employee = _employeeRepository.GetEmployee(id??1),
				Employee=employee,
				PageTitle ="Employee Details"
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
						uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
						string filePath = Path.Combine(uploadFolder, uniqueFileName);
						model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
					
				}		
				Employee newEmployee = new Employee
				{
					Name=model.Name,
					Email=model.Email,
					Department=model.Department,
					PhotoPath= uniqueFileName
				};
				_employeeRepository.Add(newEmployee);
				return RedirectToAction("details", new { id = newEmployee.Id });
			}
			return View();
		}

		[HttpGet]
		public ViewResult Edit(int id)
		{
			Employee employee = _employeeRepository.GetEmployee(id);
			EmpEditViewModel empEditViewModel = new EmpEditViewModel()
			{
				Id = employee.Id,
				Name = employee.Name,
				Email = employee.Email,
				Department = employee.Department,
				ExistingPhotoPath = employee.PhotoPath
			};
			return View(empEditViewModel);
		}

		[HttpPost]
		public ActionResult Edit(EmpEditViewModel model)
		{
			if (ModelState.IsValid)
			{
				Employee employee = _employeeRepository.GetEmployee(model.Id);
				employee.Name = model.Name;
				employee.Email = model.Email;
				employee.Department = model.Department;

				if (model.Photo !=null)
				{
					if (model.ExistingPhotoPath !=null)
					{
						string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
						System.IO.File.Delete(filePath);
					}
					employee.PhotoPath = ProcessUploadFile(model);
				}
				Employee UpdateEmployee = _employeeRepository.Update(employee);
				return RedirectToAction("index");
			}
			return View(model);
		}

		private string ProcessUploadFile(EmpEditViewModel model)
		{
			string uniqueFileName = null;
			if (model.Photo !=null)
			{
				string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
				uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				//model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
				var fileStream = new FileStream(filePath, FileMode.Create);
				model.Photo.CopyTo(fileStream);
					
			}
			return uniqueFileName;
		}
	}
}
