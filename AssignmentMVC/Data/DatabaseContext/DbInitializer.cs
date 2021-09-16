using AssignmentMVC.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentMVC.Data.DatabaseContext
{
    public class DbInitializer
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DatabaseContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await context.Database.EnsureCreatedAsync();
            if (context.Employees.Any())
            {
                return;
            }

            var Employee = new Employee
            {
                Address = "Home",
                Id = Guid.NewGuid(),
                Email = "employee@gmail.com",
                Name = "Employee",
                DoB = DateTime.Now,
                Gender = "Male",
                PhoneNumber = "08123123123",
            };
            await context.Employees.AddAsync(Employee);
            await context.SaveChangesAsync();

            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            var User = new Users
            {
                Email = "nicofernando39@gmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "nicofernando39@gmail.com",
            };
            await userManager.CreateAsync(User, "Letme1n123!");
            await userManager.AddToRoleAsync(User, "SuperAdmin");

        }
    }
}
