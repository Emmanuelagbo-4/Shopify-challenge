using System;
using System.Linq;
using CodeChallenge.Data;
using CodeChallenge.Entities;
using CodeChallenge.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeChallenge.Services
{
    public class DataSeederService
    {
        public DataSeederService()
        {

        }

        public static async void DataSeed(IServiceProvider service, IConfiguration configuration)
        {
            using (var scope = service.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var _roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var _userService = scope.ServiceProvider.GetService<UserService>();

                
                if (_roleManager.FindByNameAsync(Roles.Admin).Result == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = Roles.Admin });
                }
                if (_roleManager.FindByNameAsync(Roles.Customer).Result == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = Roles.Customer });
                }

                if (!_dbContext.ApplicationUsers.Any())
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        FirstName = "Emmanuel",
                        LastName = "Agbo",
                        Email = "agboemmanuel360@gmail.com",
                        UserName = "agboemmanuel360@gmail.com",
                        EmailConfirmed = true,
                    };
                    var result = _userService.Register(user, "Admin@Blog1232", Roles.Admin).Result;
                }
            }
        }
    }
}