using CommunityToolkit.Mvvm.ComponentModel;
using wisecorp.Models.DBModels;
using System.Windows;
using wisecorp.Context;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace wisecorp.ViewModels;

public partial class VMAdmin : ObservableObject
{
    private Departement? selectedDepartement;
    public Departement? SelectedDepartement
    {
        get { return selectedDepartement; }
        set
        {
            selectedDepartement = value;
            FilterAccounts();
        }
    }


    private List<Departement>? departements;
    public List<Departement>? Departements
    {
        get { return departements; }
        set { departements = value; }
    }



    private Role? selectedRole;
    public Role? SelectedRole
    {
        get { return selectedRole; }
        set { 
            selectedRole = value; 
            FilterAccounts();
            }
    }


    private List<Role>? roles;
    public List<Role>? Roles
    {
        get { return roles; }
        set { roles = value; }
    }



    private Title? selectedTitle;
    public Title? SelectedTitle
    {
        get { return selectedTitle; }
        set { 
            selectedTitle = value; 
            FilterAccounts();
        }
    }


    private List<Title>? titles;
    public List<Title>? Titles
    {
        get { return titles; }
        set { titles = value; }
    }


    private List<Account>? accounts;



    private ObservableCollection<Account>? filteredAccounts;
    public ObservableCollection<Account>? FilteredAccounts
    {
        get { return filteredAccounts; }
        set { filteredAccounts = value; }
    }


    private Account? selectedAccount;
    public Account? SelectedAccount
    {
        get { return selectedAccount; }
        set
        {
            selectedAccount = value;
            if (selectedAccount != null)
            {
                deepCopyAccout = selectedAccount.DeepCopy();
            }
            OnPropertyChanged(nameof(deepCopyAccout));
        }
    }

    // a copy of the selected account to avoid changing the original account. Save changes only if the user clicks the save button.
    private Account? deepCopyAccout;
    public Account? DeepCopyAccout
    {
        get { return deepCopyAccout; }
        set { deepCopyAccout = value; }
    }


    private string? filterText;
    public string? FilterText
    {
        get { return filterText; }
        set
        {
            filterText = value;
            FilterAccounts();
        }
    }


    private readonly WisecorpContext context;

    private bool displayDisabled = false;
    public bool DisplayDisabled
    {
        get { return displayDisabled; }
        set
        {
            displayDisabled = value;
            FilterAccounts();
        }
    }


    public ICommand DeleteImageCommand { get; }

    public VMAdmin()
    {
        if (App.Current.ConnectedAccount  == null || App.Current.ConnectedAccount.Role.Name != "Admin")
        {
            throw new Exception("Vous n'avez pas les droits pour acceder a cette page.");
        }

        context = new WisecorpContext();

        _ = InitializeAsync();

        Edit = new RelayCommand(Edit_Execute);
        DeleteImageCommand = new RelayCommand(DeleteImage_Execute);
    }

    public VMAdmin(WisecorpContext _context)
    {
        context = _context;
        _ = InitializeAsync();

        Edit = new RelayCommand(Edit_Execute);
        DeleteImageCommand = new RelayCommand(DeleteImage_Execute);
    }

    /// <summary>
    /// Permet d'initialiser les listes
    /// </summary>
    /// <returns></returns>
    private async Task InitializeAsync()
    {
        await GetAccounts();
        await GetDepartement();
        await GetRoles();
        await GetTitles();
        FilterAccounts();
    }

    /// <summary>
    /// Permet d'initialiser la liste des comptes
    /// </summary>
    /// <returns></returns>
    private async Task GetAccounts()
    {
        accounts = await context.Accounts.ToListAsync();
        filteredAccounts = new ObservableCollection<Account>(accounts);
        OnPropertyChanged(nameof(FilteredAccounts));
        SelectedAccount = accounts.FirstOrDefault();
    }


    /// <summary>
    /// Permet d'initialiser la liste des départements
    /// </summary>
    /// <returns></returns>
    private async Task GetDepartement()
    {
        departements = await context.Departements.ToListAsync();
        departements.Insert(0, new Departement { Id = 0, Name = "" });
        OnPropertyChanged(nameof(Departements));
    }


    /// <summary>
    /// Permet d'initialiser la listes des roles
    /// </summary>
    /// <returns></returns>
    private async Task GetRoles()
    {
        roles = await context.Roles.ToListAsync();
        roles.Insert(0, new Role { Id = 0, Name = "" });
        OnPropertyChanged(nameof(Roles));
    }


    /// <summary>
    /// Permet d'initialiser la liste des titres
    /// </summary>
    /// <returns></returns>
    private async Task GetTitles()
    {
        titles = await context.Titles.ToListAsync();
        titles.Insert(0, new Title { Id = 0, Name = "" });
        OnPropertyChanged(nameof(Titles));
    }

    /// <summary>
    /// Permet de filtrer les comptes pour la recherche
    /// </summary>
    public void FilterAccounts()
    {
        var f = accounts ?? new List<Account>();
        
        if(selectedDepartement != null && selectedDepartement.Id != 0)
            f = f.Where(a => a.DepartementId == selectedDepartement.Id).ToList();
        
        if(selectedRole != null && selectedRole.Id != 0)
            f = f.Where(a => a.RoleId == selectedRole.Id).ToList();
        
        if(selectedTitle != null && selectedTitle.Id != 0)
            f = f.Where(a => a.TitleId == selectedTitle.Id).ToList();
        
        if(!string.IsNullOrEmpty(filterText))
            f = f.Where(a => a.FullName.ToLower().Contains(filterText.ToLower())).ToList();
        
        if(!displayDisabled)
            f = f.Where(a => a.IsEnabled == true).ToList();

        filteredAccounts = new ObservableCollection<Account>(f);
        OnPropertyChanged(nameof(FilteredAccounts));
    }



    public ICommand Edit { get; }
    /// <summary>
    /// Permet de sauvegarder les modifications apporter a un compte
    /// </summary>
    public async void Edit_Execute()
    {
        if(deepCopyAccout != null && selectedAccount != null)
        {
            selectedAccount.RoleId = deepCopyAccout.RoleId;
            selectedAccount.DepartementId = deepCopyAccout.DepartementId;
            selectedAccount.TitleId = deepCopyAccout.TitleId;
            selectedAccount.EmploymentDate = deepCopyAccout.EmploymentDate;
            selectedAccount.FullName = deepCopyAccout.FullName;
            selectedAccount.Phone = deepCopyAccout.Phone;
            selectedAccount.Salary = deepCopyAccout.Salary;
            selectedAccount.NbHour = deepCopyAccout.NbHour;
            selectedAccount.HourBank = deepCopyAccout.HourBank;
            selectedAccount.Email = deepCopyAccout.Email;
            selectedAccount.IsEnabled = deepCopyAccout.IsEnabled;
            selectedAccount.Password = deepCopyAccout.Password;
            selectedAccount.Picture = deepCopyAccout.Picture;
            selectedAccount.Role = deepCopyAccout.Role;
            selectedAccount.Departement = deepCopyAccout.Departement;
            selectedAccount.Title = deepCopyAccout.Title;

            if(deepCopyAccout.IsEnabled == false)
                selectedAccount.DisableDate = DateTime.Now;
            else
                selectedAccount.DisableDate = null;

            if (SelectedAccount.IsEnabled == true)
                LogAccountModification();
            if (SelectedAccount.IsDisabled == true)
                LogDeactivatedAccount();

            await context.SaveChangesAsync();

            OnPropertyChanged(nameof(selectedAccount));
            FilterAccounts();



            ShowChangeSaved();
        }
        else
        {
            ShowSelectEmployeeToModify();
        }


    }

    /// <summary>
    /// Affiche un message indiquant que les changements ont été sauvegardés
    /// </summary>
    protected virtual void ShowChangeSaved()
    {
        MessageBox.Show((string)Application.Current.FindResource("changes_saved"));
    }


    /// <summary>
    /// Affiche un message demandant de sélectionner un employé à modifier
    /// </summary>
    protected virtual void ShowSelectEmployeeToModify()
    {
        MessageBox.Show((string)Application.Current.FindResource("select_employee_to_modify"));
    }

    /// <summary>
    /// Permet de rediriger a la fenêytre de création de compte
    /// </summary>
    [RelayCommand]
    public static void RedirectToAdd()
    {
        var mainWindow = (MainWindow)App.Current.MainWindow;
        mainWindow.NavigateTo("Views/Admin/ViewAjoutAcc.xaml");
    }

    /// <summary>
    /// Enregistre un log de sécurité pour la désactivation d'un compte
    /// </summary>
    protected virtual void LogDeactivatedAccount()
    {

        Models.DBModels.SecurityLog log = new Models.DBModels.SecurityLog
        {
            AccountId = App.Current.ConnectedAccount.Id,
            Code = Models.DBModels.SecurityLog.DeleteAccount,
            Date = DateTime.Now,
            Ip = App.GetIPAddress(),
            Description = $"Déactivation de {SelectedAccount.FullName}"
        };
        context.SecurityLogs.Add(log);
        context.SaveChanges();
    }

    /// <summary>
    /// Enregistre un log de sécurité pour la modification d'un compte
    /// </summary>
    protected virtual void LogAccountModification()
    {

        Models.DBModels.SecurityLog log = new Models.DBModels.SecurityLog
        {
            AccountId = App.Current.ConnectedAccount.Id,
            Code = Models.DBModels.SecurityLog.EditAccount,
            Date = DateTime.Now,
            Ip = App.GetIPAddress(),
            Description = $"Modification de {SelectedAccount.FullName}"
        };
        context.SecurityLogs.Add(log);
        context.SaveChanges();
    }

    // / <summary>
    // / Permet de supprimer l'image d'un compte
    // / </summary>
    private void DeleteImage_Execute()
    {
        if (selectedAccount != null)
        {
            MessageBoxResult result = MessageBox.Show((string)Application.Current.FindResource("deleteImageMessage"), (string)Application.Current.FindResource("deleteImage"), MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            selectedAccount.Picture = string.Empty;
            context.SaveChanges();
            OnPropertyChanged(nameof(selectedAccount));
            FilterAccounts();
        }
    }
    
}
 public class VMAdminTest : VMAdmin
{
    public VMAdminTest(WisecorpContext context) : base(context) { }
    protected override void LogDeactivatedAccount() { }
    protected override void LogAccountModification() { }
    protected override void ShowChangeSaved() { }
    protected override void ShowSelectEmployeeToModify() { }

}
