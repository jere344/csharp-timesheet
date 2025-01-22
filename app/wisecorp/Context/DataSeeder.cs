using wisecorp.Models.DBModels;
using wisecorp.Helpers;

namespace wisecorp.Context;

public class DataSeeder
{
    /// <summary>
    /// Initialise la base de données avec des données de départ
    /// </summary>
    /// <param name="context">Le contexte de la base de données</param>
    public static void Seed(WisecorpContext context)
    {
        SeedDepartements(context);
        SeedTitles(context);
        SeedRoles(context);
        SeedAccounts(context);
    }
    /// <summary>
    /// Ajoute les départements par défaut à la base de données
    /// </summary>
    /// <param name="context">Le contexte de la base de données</param>
    private static void SeedDepartements(WisecorpContext context)
    {
        if (!context.Departements.Any())
        {
            context.Departements.AddRange(
                new Departement { Name = "IT" },
                new Departement { Name = "HR" },
                new Departement { Name = "Finance" },
                new Departement { Name = "Marketing" },
                new Departement { Name = "Management" }
            );
            context.SaveChanges();
        }
    }
    /// <summary>
    /// Ajoute les titres par défaut à la base de données
    /// </summary>
    /// <param name="context">Le contexte de la base de données</param>
    private static void SeedTitles(WisecorpContext context)
    {
        if (!context.Titles.Any())
        {
            context.Titles.AddRange(
                new Title { Name = "Software Engineer" },
                new Title { Name = "HR Manager" },
                new Title { Name = "Accountant" },
                new Title { Name = "Marketing Manager" },
                new Title { Name = "CEO" },
                new Title { Name = "CTO" },
                new Title { Name = "Admin" }
            );

            context.SaveChanges();
        }
    }
    /// <summary>
    /// Ajoute les rôles par défaut à la base de données
    /// </summary>
    /// <param name="context">Le contexte de la base de données</param>
    private static void SeedRoles(WisecorpContext context)
    {
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "Manager" },
                new Role { Name = "User" }
            );

            context.SaveChanges();
        }
    }
    /// <summary>
    /// Ajoute les comptes par défaut à la base de données
    /// </summary>
    /// <param name="context">Le contexte de la base de données</param>
    private static void SeedAccounts(WisecorpContext context)
    {
        if (!context.Accounts.Any())
        {
            context.Accounts.AddRange(new Account
            {
                Email = "admin",
                Password = CryptographyHelper.HashPassword("admin"),
                Role = context.Roles.First(r => r.Name == "Admin"),
                Title = context.Titles.First(t => t.Name == "Admin"),
                Departement = context.Departements.First(d => d.Name == "IT"),
                FullName = "Admin",
                Salary = 100000,
                IsEnabled = true,
                EmploymentDate = DateTime.Now,
                Picture = "",
                Phone = "123456789",
                NbHour = 40,
                HourBank = 0
            },
            new Account
            {
                Email = "manager",
                Password = CryptographyHelper.HashPassword("manager"),
                Role = context.Roles.First(r => r.Name == "Manager"),
                Title = context.Titles.First(t => t.Name == "CEO"),
                Departement = context.Departements.First(d => d.Name == "Management"),
                FullName = "Manager",
                Salary = 50000,
                IsEnabled = true,
                EmploymentDate = DateTime.Now,
                Picture = "",
                Phone = "123456789",
                NbHour = 40,
                HourBank = 0
            },
            new Account
            {
                Email = "user",
                Password = CryptographyHelper.HashPassword("user"),
                Role = context.Roles.First(r => r.Name == "User"),
                Title = context.Titles.First(t => t.Name == "Software Engineer"),
                Departement = context.Departements.First(d => d.Name == "IT"),
                FullName = "User",
                Salary = 30000,
                IsEnabled = true,
                EmploymentDate = DateTime.Now,
                Picture = "",
                Phone = "123456789",
                NbHour = 40,
                HourBank = 0
            });
            context.SaveChanges();
        }
    }

}