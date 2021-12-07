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
	[Authorize]
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		#region Register
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register()
		{
			return View();
		}
		
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new AppUser
				{
					UserName = model.UserName,
					Email = model.Email,
					City =model.City
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					if (_signInManager.IsSignedIn(User)&&User.IsInRole("Admin"))
					{
						return RedirectToAction("ListUsers", "home");
					}
					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("index", "home");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return View(model);
		}
		#endregion

		#region Logout

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction("index", "home");
		}
		#endregion

		#region Login
		
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel model,string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,model.RememberMe,false);
				if (result.Succeeded)
				{
					if (!string.IsNullOrEmpty(returnUrl) &&Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("index", "home");
					}
					
				}
				ModelState.AddModelError(string.Empty, "Invalid Login Attemp");
			}
			return View(model);
		}

		#endregion

		#region IsEmailUse

		[AcceptVerbs("Get","Post")]
		[AllowAnonymous]
		public async Task<IActionResult> IsEmailInUse(string Email)
		{
			var user = await _userManager.FindByEmailAsync(Email);
			if (user ==null)
			{
				return Json(true);
			}
			else
			{
				return Json($"Email {Email} is already in use.");
			}
		}

		#endregion

		#region AccessDenied
		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccessDenied()
		{
			return View();
		}
		#endregion

	}
}
