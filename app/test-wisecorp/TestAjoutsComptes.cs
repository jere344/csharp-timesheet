using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using wisecorp.Context;
using wisecorp.Models.DBModels;
using wisecorp.ViewModels;

namespace test_wisecorp
{
    [TestClass]
    public class UnitTest1
    {
        private WisecorpContext context;
        private VMAdminAjouts viewmodel;
        private DatabaseHelper dbHelper;

        [TestInitialize]
        public async Task Initialize()
        {
            dbHelper = new DatabaseHelper();
            context = dbHelper.CreateContext();
            await dbHelper.CreateTables(context);
            viewmodel = new VMAdminAjoutsTest(context);
        }
        [TestMethod]
        public async Task AjoutsGood()
        {
            Departement deps = await context.Departements.FirstOrDefaultAsync();
            Role rols = await context.Roles.FirstOrDefaultAsync();
            Title tils = await context.Titles.FirstOrDefaultAsync();

            viewmodel.NbHours = 20;
            viewmodel.Telephone = "581-306-3896";
            viewmodel.DateEmbauche = DateTime.Now;
            viewmodel.MotsDePasse = "ligam123";
            viewmodel.Depart = deps;
            viewmodel.NomComplet = "Testitos";
            viewmodel.Salaire = 25;
            viewmodel.Mail = "lemail";
            viewmodel.RoleSelect = rols;
            viewmodel.Titre = tils;

            await viewmodel.SaveAccount();
            await Task.Run(() => viewmodel.SaveAccountCommand.Execute(null));

            List<Account> lecompes = await context.Accounts.ToListAsync();
            lecompes.Count().Should().Be(3);


        }

        [TestMethod]
        public async Task AjoutTelephoneNull()
        {
            Departement deps = await context.Departements.FirstOrDefaultAsync();
            Role rols = await context.Roles.FirstOrDefaultAsync();
            Title tils = await context.Titles.FirstOrDefaultAsync();

            viewmodel.NbHours = 20;
            viewmodel.Telephone = null;
            viewmodel.DateEmbauche = DateTime.Now;
            viewmodel.MotsDePasse = "ligam123";
            viewmodel.Depart = deps;
            viewmodel.NomComplet = "Testitos";
            viewmodel.Salaire = 25;
            viewmodel.Mail = "lemail";
            viewmodel.RoleSelect = rols;
            viewmodel.Titre = tils;

            await viewmodel.SaveAccount();

            List<Account> lecompes = await context.Accounts.ToListAsync();
            lecompes.Count().Should().Be(2);


        }

        [TestMethod]
        public async Task AjoutnbHourZero()
        {
            Departement deps = await context.Departements.FirstOrDefaultAsync();
            Role rols = await context.Roles.FirstOrDefaultAsync();
            Title tils = await context.Titles.FirstOrDefaultAsync();

            viewmodel.NbHours = 0;
            viewmodel.Telephone = "581-306-3896";
            viewmodel.DateEmbauche = DateTime.Now;
            viewmodel.MotsDePasse = "ligam123";
            viewmodel.Depart = deps;
            viewmodel.NomComplet = "Testitos";
            viewmodel.Salaire = 25;
            viewmodel.Mail = "lemail";
            viewmodel.RoleSelect = rols;
            viewmodel.Titre = tils;

            await viewmodel.SaveAccount();

            List<Account> lecompes = await context.Accounts.ToListAsync();
            lecompes.Count().Should().Be(2);
            viewmodel.ErrorMessage.Should().Be("Le nombre d'heure semaine ne peut pas être vide.");
        }

        [TestMethod]
        public async Task DateEmbaucheWrong()
        {
            Departement deps = await context.Departements.FirstOrDefaultAsync();
            Role rols = await context.Roles.FirstOrDefaultAsync();
            Title tils = await context.Titles.FirstOrDefaultAsync();

            DateTime date = new DateTime(2015, 12, 01);
;
            viewmodel.NbHours = 10;
            viewmodel.Telephone = "581-306-3896";
            viewmodel.DateEmbauche = date;
            viewmodel.MotsDePasse = "ligam123";
            viewmodel.Depart = deps;
            viewmodel.NomComplet = "Testitos";
            viewmodel.Salaire = 25;
            viewmodel.Mail = "lemail";
            viewmodel.RoleSelect = rols;
            viewmodel.Titre = tils;

            await viewmodel.SaveAccount();

            List<Account> lecompes = await context.Accounts.ToListAsync();
            lecompes.Count().Should().Be(2);
            viewmodel.ErrorMessage.Should().Be("La date d'embauche ne peut pas être antérieure à aujourd'hui.");
        }

        [TestMethod]
        public async Task MdpNull()
        {
            Departement deps = await context.Departements.FirstOrDefaultAsync();
            Role rols = await context.Roles.FirstOrDefaultAsync();
            Title tils = await context.Titles.FirstOrDefaultAsync();

            viewmodel.NbHours = 20;
            viewmodel.Telephone = "581-306-3896";
            viewmodel.DateEmbauche = DateTime.Now;
            viewmodel.MotsDePasse = null;
            viewmodel.Depart = deps;
            viewmodel.NomComplet = "Testitos";
            viewmodel.Salaire = 25;
            viewmodel.Mail = "lemail";
            viewmodel.RoleSelect = rols;
            viewmodel.Titre = tils;

            await viewmodel.SaveAccount();

            List<Account> lecompes = await context.Accounts.ToListAsync();
            lecompes.Count().Should().Be(2);
            viewmodel.ErrorMessage.Should().Be("Le mots de passe ne peut pas être vide.");

        }

        [TestMethod]
        public async Task NomNull()
        {
            Departement deps = await context.Departements.FirstOrDefaultAsync();
            Role rols = await context.Roles.FirstOrDefaultAsync();
            Title tils = await context.Titles.FirstOrDefaultAsync();

            viewmodel.NbHours = 20;
            viewmodel.Telephone = "581-306-3896";
            viewmodel.DateEmbauche = DateTime.Now;
            viewmodel.MotsDePasse = "ligma123";
            viewmodel.Depart = deps;
            viewmodel.NomComplet = null;
            viewmodel.Salaire = 25;
            viewmodel.Mail = "lemail";
            viewmodel.RoleSelect = rols;
            viewmodel.Titre = tils;

            await viewmodel.SaveAccount();

            List<Account> lecompes = await context.Accounts.ToListAsync();
            lecompes.Count().Should().Be(2);
            viewmodel.ErrorMessage.Should().Be("Le Nom ne peut pas être vide.");

        }

        [TestMethod]
        public async Task Salaire0()
        {
            Departement deps = await context.Departements.FirstOrDefaultAsync();
            Role rols = await context.Roles.FirstOrDefaultAsync();
            Title tils = await context.Titles.FirstOrDefaultAsync();

            viewmodel.NbHours = 20;
            viewmodel.Telephone = "581-306-3896";
            viewmodel.DateEmbauche = DateTime.Now;
            viewmodel.MotsDePasse = "ligma123";
            viewmodel.Depart = deps;
            viewmodel.NomComplet = "Mec";
            viewmodel.Salaire = 0;
            viewmodel.Mail = "lemail";
            viewmodel.RoleSelect = rols;
            viewmodel.Titre = tils;

            await viewmodel.SaveAccount();

            List<Account> lecompes = await context.Accounts.ToListAsync();
            lecompes.Count().Should().Be(2);
            viewmodel.ErrorMessage.Should().Be("Le Salaire ne peut pas être vide.");

        }

        [TestMethod]
        public async Task MailNull()
        {
            Departement deps = await context.Departements.FirstOrDefaultAsync();
            Role rols = await context.Roles.FirstOrDefaultAsync();
            Title tils = await context.Titles.FirstOrDefaultAsync();

            viewmodel.NbHours = 20;
            viewmodel.Telephone = "581-306-3896";
            viewmodel.DateEmbauche = DateTime.Now;
            viewmodel.MotsDePasse = "ligma123";
            viewmodel.Depart = deps;
            viewmodel.NomComplet = "Mec";
            viewmodel.Salaire = 10;
            viewmodel.Mail = null;
            viewmodel.RoleSelect = rols;
            viewmodel.Titre = tils;

            await viewmodel.SaveAccount();

            List<Account> lecompes = await context.Accounts.ToListAsync();
            lecompes.Count().Should().Be(2);
            viewmodel.ErrorMessage.Should().Be("Le mail ne peut pas être vide.");

        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await dbHelper.DropTestTables(context);
        }
    }
}