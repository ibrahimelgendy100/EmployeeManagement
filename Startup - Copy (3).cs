using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Models.EmployeeRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
			services.AddMvc(options=>options.EnableEndpointRouting=false).AddXmlSerializerFormatters();
			services.AddIdentity<IdentityUser, IdentityRole>(options =>
			{
				options.Password.RequiredLength = 10;
				options.Password.RequiredUniqueChars = 3;
				options.Password.RequireNonAlphanumeric = false;
			}).AddEntityFrameworkStores<DataContext>();
			//services.AddScoped<IEmployeeRepository, EmployeeRepository>();
			services.AddScoped<IEmployeeRepository, SqlEmpRepository>();

			// frist method to complixy password
			//services.Configure<IdentityOptions>(options =>
			//{
			//	options.Password.RequiredLength = 10;
			//	options.Password.RequiredUniqueChars = 3;
			//	options.Password.RequireNonAlphanumeric = false;
			//});
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
			app.UseMvc(routes =>
			{
				routes.MapRoute(
						name: "default",
						template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
