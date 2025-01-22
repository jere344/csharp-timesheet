using wisecorp.Models.DBModels;

namespace wisecorp.Context
{
    public class DatabaseSeeder
    {
        private readonly WisecorpContext _context;

        public DatabaseSeeder(WisecorpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Initialise la base de données avec des données de départ
        /// </summary>
        public void Seed()
        {
            SeedDepartements();
            SeedTitles();
            SeedRoles();
            SeedAccounts();
        }

        /// <summary>
        /// Ajoute les départements par défaut à la base de données
        /// </summary>
        private void SeedDepartements()
        {
            if (_context.Departements.Any())
            {
                return;
            }

            var departements = new List<Departement>
            {
                new Departement { Name = "IT" },
                new Departement { Name = "HR" },
                new Departement { Name = "Finance" },
                new Departement { Name = "Marketing" },
                new Departement { Name = "Sales" },
                new Departement { Name = "Production" },
                new Departement { Name = "Logistics" },
                new Departement { Name = "Quality" },
                new Departement { Name = "Maintenance" },
                new Departement { Name = "R&D" },
                new Departement { Name = "Legal" },
                new Departement { Name = "Communication" },
                new Departement { Name = "Management" }
            };

            _context.Departements.AddRange(departements);
            _context.SaveChanges();
        }

        /// <summary>
        /// Ajoute les titres par défaut à la base de données
        /// </summary>
        private void SeedTitles()
        {
            if (_context.Titles.Any())
            {
                return;
            }

            var titles = new List<Title>
            {
                new Title { Name = "Software Engineer" },
                new Title { Name = "HR Manager" },
                new Title { Name = "Accountant" },
                new Title { Name = "Marketing Manager" },
                new Title { Name = "Sales Manager" },
                new Title { Name = "Production Manager" },
                new Title { Name = "Logistics Manager" },
                new Title { Name = "Quality Manager" },
                new Title { Name = "Maintenance Manager" },
                new Title { Name = "R&D Manager" },
                new Title { Name = "Legal Manager" },
                new Title { Name = "Communication Manager" },
                new Title { Name = "CEO" }
            };

            _context.Titles.AddRange(titles);
            _context.SaveChanges();
        }

        /// <summary>
        /// Ajoute les rôles par défaut à la base de données
        /// </summary>
        private void SeedRoles()
        {
            if (_context.Roles.Any())
            {
                return;
            }

            var roles = new List<Role>
            {
                new Role { Name = "Admin" },
                new Role { Name = "Manager" },
                new Role { Name = "Employee" }
            };

            _context.Roles.AddRange(roles);
            _context.SaveChanges();
        }

        /// <summary>
        /// Ajoute les comptes par défaut à la base de données
        /// </summary>
        private void SeedAccounts()
        {
            if (_context.Accounts.Any())
            {
                return;
            }

            var accounts = new List<Account>
            {
                new Account
                {
                    FullName = "Admin",
                    Email = "admin",
                    Password = "admin",
                    Role = _context.Roles.First(r => r.Name == "Admin"),
                    Departement = _context.Departements.First(d => d.Name == "Management"),
                    Title = _context.Titles.First(t => t.Name == "CEO"),
                    Salary = 0,
                    EmploymentDate = DateTime.Now,
                    IsEnabled = true
                },
                new Account
                {
                    FullName = "Manager",
                    Email = "manager",
                    Password = "manager",
                    Role = _context.Roles.First(r => r.Name == "Manager"),
                    Departement = _context.Departements.First(d => d.Name == "Management"),
                    Title = _context.Titles.First(t => t.Name == "Manager"),
                    Salary = 0,
                    EmploymentDate = DateTime.Now,
                    IsEnabled = true
                },
                new Account
                {
                    FullName = "Employee",
                    Email = "employee",
                    Password = "employee",
                    Role = _context.Roles.First(r => r.Name == "Employee"),
                    Departement = _context.Departements.First(d => d.Name == "Management"),
                    Title = _context.Titles.First(t => t.Name == "Software Engineer"),
                    Salary = 0,
                    EmploymentDate = DateTime.Now,
                    IsEnabled = true
                }
            };

            _context.Accounts.AddRange(accounts);
            _context.SaveChanges();
        }
    }
}
