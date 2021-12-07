using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Models.EmployeeRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement
{
	public class Startup
	{
		private readonly IConfiguration _config;

		public Startup(IConfiguration config)
		{
			_config = config;
		}
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContextPool<DataContext>(options => options.UseSqlServer(_config.GetConnectionString("DataConnection")));
			services.AddMvc(options=> {
				var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
				options.Filters.Add(new AuthorizeFilter(policy));
			});
			services.AddScoped<IEmployeeRepository, SqlEmpRepository>();
			services.AddMvc(options=>options.EnableEndpointRouting=false).AddXmlSerializerFormatters();
			services.AddIdentity<AppUser, IdentityRole>(options =>
			{
				options.Password.RequiredLength = 10;
				options.Password.RequiredUniqueChars = 3;
				options.Password.RequireNonAlphanumeric = false;
			}).AddEntityFrameworkStores<DataContext>();
			
			
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{

				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseStatusCodePagesWithReExecute("/Error/{0}");
			}

			app.UseRouting();	
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
						name: "default",
						template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
