using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdministrationController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;

		public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		#region CreateRole
		[HttpGet]
		public IActionResult CreateRole()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				IdentityRole role = new IdentityRole
				{
					Name = model.RoleName
				};
				IdentityResult result =await _roleManager.CreateAsync(role);

				if (result.Succeeded)
				{
					return RedirectToAction("index", "home");
				}

				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return View(model);
		}
		#endregion


		#region GetAllRoles

		[HttpGet]
		public IActionResult ListRoles()
		{
			var roles = _roleManager.Roles;
			return View(roles);
		}
		#endregion

		#region EditRole
		[HttpGet]
		public async Task<IActionResult> EditRole(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role ==null)
			{
				ViewBag.ErrorMessage = $"Role with id {id} not found";
				return View("NotFound");
			}

			var model = new EditRoleViewModel
			{
				Id = role.Id,
				RoleName = role.Name
			};
			foreach (var user in _userManager.Users.ToList())
			{
				
				if (await _userManager.IsInRoleAsync(user, role.Name))
				{
					model.Users.Add(user.UserName);
				}
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditRole(EditRoleViewModel model)
		{
			var role = await _roleManager.FindByIdAsync(model.Id);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with id {model.Id} not found";
				return View("NotFound");
			}
			else
			{
				role.Name = model.RoleName;
				var result = await _roleManager.UpdateAsync(role);
				if (result.Succeeded)
				{
					return RedirectToAction("ListRoles");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);
			}
		}

		#endregion

		#region EditUserRole

		[HttpGet]
		public async Task<IActionResult> EditUserInRole(string roleId)
		{
			ViewBag.roleId = roleId;
			var role = await _roleManager.FindByIdAsync(roleId);
			if (role == null)
			{

				ViewBag.ErrorMessage = $"Role with id {roleId} not found";
				return View("NotFound");
			}

			var model = new List<UserRoleViewModel>();
			foreach (var user in _userManager.Users.ToList())
			{
				var userRoleviewModel = new UserRoleViewModel
				{
					UserId=user.Id,
					UserName=user.UserName
				};
				if (await _userManager.IsInRoleAsync(user,role.Name))
				{
					userRoleviewModel.IsSelected = true;
				}
				else
				{
					userRoleviewModel.IsSelected = false;
				}
				model.Add(userRoleviewModel);
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string roleId)
		{
			var role = await _roleManager.FindByIdAsync(roleId);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
				return View("NotFound");
			}

			for (int i = 0; i < model.Count; i++)
			{
				var user = await _userManager.FindByIdAsync(model[i].UserId);

				IdentityResult result;
				if (!(await _userManager.IsInRoleAsync(user, role.Name)) && model[i].IsSelected )
				{
					result = await _userManager.AddToRoleAsync(user, role.Name);
				}
				else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
				{
					result = await _userManager.RemoveFromRoleAsync(user, role.Name);
				}
				else
				{
					continue;
				}

				if (result.Succeeded)
				{
					if (i < (model.Count - 1))
						continue;
					else
						return RedirectToAction("EditRole", new { Id = roleId });
				}
			}

			return RedirectToAction("EditRole", new { Id = roleId });
		}
		#endregion

		#region ListUsers

		[HttpGet]
		public IActionResult ListUsers()
		{
			var users = _userManager.Users;
			return View(users);
		}

		#endregion
	}
}
