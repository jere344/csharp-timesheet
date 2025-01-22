using CommunityToolkit.Mvvm.ComponentModel;
using wisecorp.Models.DBModels;
using System.Windows;
using wisecorp.Context;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Input;
using wisecorp.Helpers;
using System.Reflection.Metadata;
using System.Windows.Media.Media3D;

namespace wisecorp.ViewModels;

public partial class VMAdminAjouts : ObservableObject
{
    //Propriété bind a la vue
    [ObservableProperty]
    string errorMessage;

    [ObservableProperty]
    string nomComplet;

    [ObservableProperty]
    double salaire;

    [ObservableProperty]
    Role roleSelect;

    [ObservableProperty]
    List<Role> lesRoles;

    [ObservableProperty]
    Departement depart;

    [ObservableProperty]
    List<Departement> departements;

    [ObservableProperty]
    Title titre;

    [ObservableProperty]
    List<Title> lesTitres;

    [ObservableProperty]
    DateTime dateEmbauche = DateTime.Now;

    [ObservableProperty]
    string motsDePasse;

    [ObservableProperty]
    string telephone;

    [ObservableProperty]
    string mail;

    [ObservableProperty]
    int nbHours;

    
    private WisecorpContext context;

    public VMAdminAjouts()
    {
        context = new WisecorpContext();
        _ = InitializeAsync();
    }

    public VMAdminAjouts(WisecorpContext _context)
    {
        context = _context;
        _ = InitializeAsync();
    }

    /// <summary>
    /// Sert a toute loader les liste, car si n'est pas dans une fonction elle ne sont pas toutes loadé
    /// </summary>
    /// <returns></returns>
    private async Task InitializeAsync()
    {
        await GetDepartement();
        await GetRoles();
        await GetTitre();
    }

    /// <summary>
    /// Get la liste des roles présents dans la base de donnée
    /// </summary>
    /// <returns></returns>
    private async Task GetRoles()
    {
        lesRoles = await context.Roles.ToListAsync();

        if (lesRoles != null)
        {
            roleSelect = lesRoles[0];
        }

        OnPropertyChanged(nameof(lesRoles));
        OnPropertyChanged(nameof(roleSelect));
    }

    /// <summary>
    /// Get la liste des départements présents dans la base de donnée
    /// </summary>
    /// <returns></returns>
    private async Task GetDepartement()
    {
        departements = await context.Departements.ToListAsync();

        if (departements != null)
        {
            depart = departements[0];
        }

        OnPropertyChanged(nameof(Departements));
        OnPropertyChanged(nameof(depart));
    }

    /// <summary>
    /// Get la liste des titres présents dans la base de donnée
    /// </summary>
    /// <returns></returns>
    private async Task GetTitre()
    {
        lesTitres = await context.Titles.ToListAsync();

        if (lesTitres != null)
        {
            titre = lesTitres[0];
        }

        OnPropertyChanged(nameof(titre));
        OnPropertyChanged(nameof(lesTitres));
    }

    /// <summary>
    /// Commande qui savegarde le current account dans la window
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    public async Task SaveAccount()
    {
  
        //Série de if qui viennent indiquer a l'utilisateur si le formulaire est bien remplis
        if (NomComplet == null) { errorMessage = "Le Nom ne peut pas être vide."; }
        if (salaire == null || salaire <= 0) { errorMessage = "Le Salaire ne peut pas être vide."; }
        if (motsDePasse == null) { errorMessage = "Le mots de passe ne peut pas être vide."; }
        if (telephone == null) { errorMessage = "Le numéro de téléphone ne peut pas être vide."; }
        if (mail == null) { errorMessage = "Le mail ne peut pas être vide."; }
        if (nbHours == null || nbHours <= 0) { errorMessage = "Le nombre d'heure semaine ne peut pas être vide."; }
        if (dateEmbauche.Date < DateTime.Now.Date) { errorMessage = "La date d'embauche ne peut pas être antérieure à aujourd'hui."; }

        //Va chercher le compte qui a le meme courriel si y'en a un qui existe
        Account? v = await context.Accounts.Where(a => a.Email == mail).FirstOrDefaultAsync();

        //Viens mettre le message a null si tout est valide
        if (NomComplet != null &&
            salaire > 0 &&
            motsDePasse != null &&
            telephone != null &&
            mail != null &&
            nbHours > 0 &&
            dateEmbauche.Date >= DateTime.Now.Date)
        {
            errorMessage = String.Empty;
        }

        //Si error message est null ou empty effectue la sauvegarde
        if (String.IsNullOrEmpty(errorMessage))
        {
            //Check que le compte qui a soit disant le meme courriel est null
            if (v == null)
            {
                string passwd = CryptographyHelper.HashPassword(motsDePasse);

                //Sauvegarde seulement les informations importante a l'administration
                Account leCompte = new Account
                {
                    DepartementId = depart.Id,
                    RoleId = roleSelect.Id,
                    Email = mail,
                    Password = passwd,
                    TitleId = titre.Id,
                    EmploymentDate = DateEmbauche,
                    FullName = NomComplet,
                    Salary = salaire,
                    IsEnabled = true,
                    Phone = Telephone,
                    NbHour = nbHours,
                    HourBank = 0,
                    Pseudo = String.Empty,
                    PersonalEmail = String.Empty,
                    Picture = String.Empty
                };

                await  context.Accounts.AddAsync(leCompte);
                await context.SaveChangesAsync();

                LogAddAccount();
                RedirectToList();
            }
            else
                errorMessage = "Ce courriel est deja présent dans la base de donnée";

        }
        OnPropertyChanged(nameof(ErrorMessage));

    }

    /// <summary>
    /// Redirection vers la liste des comptes présent dans la base de donnée
    /// </summary>
    protected virtual void RedirectToList()
    {
        var mainWindow = (MainWindow)App.Current.MainWindow;
        mainWindow.NavigateTo("Views/Admin/ViewAdmin.xaml");
    }

    /// <summary>
    /// Enregistre un log de sécurité pour la création d'un nouveau compte
    /// </summary>
    protected virtual void LogAddAccount()
    {

        Models.DBModels.SecurityLog log = new Models.DBModels.SecurityLog
        {
            AccountId = App.Current.ConnectedAccount.Id,
            Code = Models.DBModels.SecurityLog.AddAccount,
            Date = DateTime.Now,
            Ip = App.GetIPAddress(),
            Description = $"Compte de {nomComplet} crée"
        };
        context.SecurityLogs.Add(log);
        context.SaveChanges();
    }

}

public class VMAdminAjoutsTest : VMAdminAjouts
{

    public VMAdminAjoutsTest(WisecorpContext context) : base(context)
    {
    }


    protected override void RedirectToList()
    {
        // Ne fait rien
    }

    protected override void LogAddAccount()
    {
        // Ne fait rien
    }
}
