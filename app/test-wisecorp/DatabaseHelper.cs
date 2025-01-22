using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wisecorp.Context;
using wisecorp.Helpers;
using wisecorp.Models.DBModels;
using Microsoft.EntityFrameworkCore.InMemory;

namespace test_wisecorp
{
    internal class DatabaseHelper
    {
        public WisecorpContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<WisecorpContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new WisecorpContext(options);
            return context;

        }


        public async Task CreateTables(WisecorpContext context)
        {

            await context.Database.EnsureCreatedAsync();

            await context.Departements.AddRangeAsync(
                //new Departement { Name = "IT" },
                new Departement { Name = "Ton test fonctionne le grand" },
                new Departement { Name = "HR" },
                new Departement { Name = "Finance" },
                new Departement { Name = "Marketing" },
                new Departement { Name = "Management" }
                  );

            await context.Titles.AddRangeAsync(
                    //new Title { Name = "Software Engineer" },
                    new Title { Name = "HR Manager" },
                    new Title { Name = "Accountant" },
                    new Title { Name = "Marketing Manager" },
                    new Title { Name = "CEO" },
                    new Title { Name = "CTO" },
                    new Title { Name = "Admin" }
                    );

            await context.Roles.AddRangeAsync(
                    //new Role { Name = "Admin" },
                    new Role { Name = "Manager" },
                    new Role { Name = "User" }
                    );

            Role rol = new Role { Name = "Admin", Id = 8 };
            Title til = new Title { Name = "Software Engineer", Id = 8 };
            Departement dep = new Departement { Name = "IT", Id = 7 };



            Account emp1 = new Account
            {
                Email = "admin",
                Password = CryptographyHelper.HashPassword("admin"),
                Role = rol,
                Title = til,
                Departement = dep,
                FullName = "Admin",
                Salary = 100000,
                IsEnabled = true,
                EmploymentDate = DateTime.Now,
                Picture = "",
                Phone = "123456789",
                NbHour = 40,
                HourBank = 0,
                PersonalEmail = String.Empty,
                Pseudo = String.Empty
            };

            Account emp2 = new Account
            {
                Email = "manager",
                Password = CryptographyHelper.HashPassword("manager"),
                Role = rol,
                Title = til,
                Departement = dep,
                FullName = "Manager",
                Salary = 50000,
                IsEnabled = true,
                EmploymentDate = DateTime.Now,
                Picture = "",
                Phone = "123456789",
                NbHour = 40,
                HourBank = 0,
                PersonalEmail = String.Empty,
                Pseudo = String.Empty
                
            };
            await context.Accounts.AddRangeAsync(emp2,emp1);


            await context.SaveChangesAsync();
        }

        public async Task DropTestTables(WisecorpContext context)
        {
            await context.Database.EnsureDeletedAsync();
        }



    }
}
