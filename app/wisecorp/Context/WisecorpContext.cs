using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;
using wisecorp.Models.DBModels;
using wisecorp.Helpers;
using System.Diagnostics;

namespace wisecorp.Context;

public class WisecorpContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Departement> Departements { get; set; }
    public DbSet<Title> Titles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectAssignement> ProjectAssignements { get; set; }
    public DbSet<Work> Works { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<SessionToken> SessionTokens { get; set; }
    public DbSet<SecurityLog> SecurityLogs { get; set; }

    private bool isDebug = false;

    [Conditional("DEBUG")]
    private void IsDebugCheck(ref bool isDebug)
    {
        isDebug = true;
    }
    public WisecorpContext() { }
    public WisecorpContext(DbContextOptions<WisecorpContext> options): base(options) { }

    /// <summary>
    /// Configure le contexte de la base de données
    /// </summary>
    /// <param name="optionsBuilder">Le constructeur d'options pour configurer le contexte</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            IsDebugCheck(ref isDebug);
            var connectionString = "";

            if (isDebug)
            {
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }
            else
            {
                connectionString = configuration.GetConnectionString("ReleaseConnection");
            }


            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            //use lazy loadings
            optionsBuilder.UseLazyLoadingProxies();
        } 
    }

    /// <summary>
    /// Authentifie un utilisateur avec son email et son mot de passe
    /// </summary>
    /// <param name="email">L'email de l'utilisateur</param>
    /// <param name="password">Le mot de passe de l'utilisateur</param>
    /// <returns>Le compte de l'utilisateur s'il est authentifié, sinon null</returns>
    public Account? Login(string email, string password)
    {
        var account = Accounts.FirstOrDefault(a => a.Email == email);
        if (account == null)
        {
            return null;
        }
        if (!account.IsEnabled)
        {
            MessageBox.Show((string)Application.Current.FindResource("account_disabled"));
            return null;
        }
        if (CryptographyHelper.VerifyPassword(password, account.Password))
        {
            App.Current.ConnectedAccount = account;
            return account;
        }
        return null;
    }



    /// <summary>
    /// Génère un code de vérification pour un compte donné
    /// </summary>
    /// <param name="account">Le compte pour lequel générer le code</param>
    /// <returns>Le code de vérification généré</returns>
    public VerificationCode GenerateVerificationCode(Account account)
    {
        // if there is already a verification code for this account, delete it
        var existingCode = VerificationCodes.FirstOrDefault(c => c.AccountId == account.Id);
        if (existingCode != null)
        {
            VerificationCodes.Remove(existingCode);
        }
        var code = new VerificationCode
        {
            AccountId = account.Id,
            Code = CryptographyHelper.GenerateRandomString(8),
            ExpirationDate = DateTime.Now.AddMinutes(5)
        };
        VerificationCodes.Add(code);
        SaveChanges();

        return code;
    }
}

