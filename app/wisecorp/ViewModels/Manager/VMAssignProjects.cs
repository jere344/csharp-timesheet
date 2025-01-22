using CommunityToolkit.Mvvm.ComponentModel;
using wisecorp.Models.DBModels;
using System.Windows;
using wisecorp.Context;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace wisecorp.ViewModels;

public class VMAssignProjects : ObservableObject
{
    private List<Project> _projects;
    private Project? _selectedProject;
    private List<Account> _accounts;
    private List<Departement> _departements;
    private ObservableCollection<Account> _projectAssignedAccounts;
    private ObservableCollection<Departement> _projectAssignedDepartements;

    public List<Project> Projects
    {
        get => _projects;
        set => SetProperty(ref _projects, value);
    }

    public List<Account> Accounts
    {
        get => _accounts;
        set => SetProperty(ref _accounts, value);
    }

    public List<Departement> Departements
    {
        get => _departements;
        set => SetProperty(ref _departements, value);
    }

    public Project? SelectedProject
    {
        get => _selectedProject;
        set
        {
            SetProperty(ref _selectedProject, value);
            FetchProjectAssignements();
        }
    }

    // ---------------------- Account Assignements ----------------------
    
    public ObservableCollection<Account> ProjectAssignedAccounts
    {
        get => _projectAssignedAccounts ??= new ObservableCollection<Account>();
        set
        {
            SetProperty(ref _projectAssignedAccounts, value);
            OnPropertyChanged(nameof(ProjectUnassignedAccounts));
            OnPropertyChanged(nameof(PAAFiltered));
            OnPropertyChanged(nameof(PUAFiltered));
        }
    }
    // PAA = ProjectAssignedAccounts
    private string _pAASearchText = "";
    public string PAASearchText
    {
        get => _pAASearchText.ToLowerInvariant();
        set
        {
            SetProperty(ref _pAASearchText, value);
            OnPropertyChanged(nameof(PAAFiltered));
        }
    }
    public List<Account> PAAFiltered => new(ProjectAssignedAccounts.Where(a => PAASearchText == "" || a.FullName.ToLowerInvariant().Contains(PAASearchText)).ToList());



    public List<Account> ProjectUnassignedAccounts => SelectedProject != null ? Accounts?.Except(ProjectAssignedAccounts).ToList() : new List<Account>();
    // PUA = ProjectUnassignedAccounts
    private string _pUASearchText = "";
    public string PUASearchText
    {
        get => _pUASearchText.ToLowerInvariant();
        set
        {
            SetProperty(ref _pUASearchText, value);
            OnPropertyChanged(nameof(PUAFiltered));
        }
    }
    public List<Account> PUAFiltered => new(ProjectUnassignedAccounts.Where(a => PUASearchText == "" || a.FullName.ToLowerInvariant().Contains(PUASearchText)).ToList());



    // ---------------------- Departement Assignements ----------------------
    public ObservableCollection<Departement> ProjectAssignedDepartements
    {
        get => _projectAssignedDepartements ??= new ObservableCollection<Departement>();
        set
        {
            SetProperty(ref _projectAssignedDepartements, value);
            OnPropertyChanged(nameof(ProjectUnassignedDepartements));
        }
    }
    public List<Departement> ProjectUnassignedDepartements => SelectedProject != null ? Departements?.Except(ProjectAssignedDepartements).ToList() : new List<Departement>();
    // endregion


    public WisecorpContext context { get; set; }

    public ICommand AssignAccountCommand { get; }
    public ICommand UnassignAccountCommand { get; }
    public ICommand AssignDepartementCommand { get; }
    public ICommand UnassignDepartementCommand { get; }

    public VMAssignProjects()
    {
        context = new WisecorpContext();

        FetchDefaultData();
        AssignAccountCommand = new RelayCommand<int>(AssignAccount);
        UnassignAccountCommand = new RelayCommand<int>(UnassignAccount);
        AssignDepartementCommand = new RelayCommand<int>(AssignDepartement);
        UnassignDepartementCommand = new RelayCommand<int>(UnassignDepartement);

    }

    /// <summary>
    /// Récupère les données par défaut (projets actifs, comptes activés, départements) depuis la base de données
    /// </summary>
    private async void FetchDefaultData()
    {
        Projects = await context.Projects.Where(p => p.IsActive).ToListAsync();
        Projects.Sort((a, b) => a.GetFullProjectTree.CompareTo(b.GetFullProjectTree));
        Accounts = await context.Accounts.Where(a => a.IsEnabled).OrderBy(a => a.FullName).ToListAsync();
        Departements = await context.Departements.OrderBy(d => d.Name).ToListAsync();

        OnPropertyChanged(nameof(Projects));
        OnPropertyChanged(nameof(Accounts));
        OnPropertyChanged(nameof(Departements));
    }

    /// <summary>
    /// Récupère les assignations de projet (comptes et départements) pour le projet sélectionné
    /// </summary>
    private async void FetchProjectAssignements()
    {
        ProjectAssignedAccounts = new ObservableCollection<Account>();
        ProjectAssignedDepartements = new ObservableCollection<Departement>();
        if (SelectedProject == null)
        {
            return;
        }

        List<ProjectAssignement> projectAssignements = await context.ProjectAssignements
            .Where(pa => pa.ProjectId == SelectedProject.Id)
            .ToListAsync();

        ProjectAssignedAccounts = new ObservableCollection<Account>(Accounts.Where(a => projectAssignements.Any(pa => pa.AccountId == a.Id)).ToList());
        ProjectAssignedDepartements = new ObservableCollection<Departement>(Departements.Where(d => projectAssignements.Any(pa => pa.DepartementId == d.Id)).ToList());

        OnPropertyChanged(nameof(ProjectAssignedAccounts));
        OnPropertyChanged(nameof(ProjectAssignedDepartements));
        OnPropertyChanged(nameof(ProjectUnassignedAccounts));
        OnPropertyChanged(nameof(PAAFiltered));
        OnPropertyChanged(nameof(PUAFiltered));
        OnPropertyChanged(nameof(ProjectUnassignedDepartements));
    }

    /// <summary>
    /// Assigne un compte au projet sélectionné
    /// </summary>
    /// <param name="accountId">L'ID du compte à assigner</param>
    private async void AssignAccount(int accountId)
    {
        if (SelectedProject == null) { return; }

        if (SelectedProject.IsActive == false)
        {
            MessageBox.Show((string)Application.Current.FindResource("cannot_assign_accounts"), (string)Application.Current.FindResource("error"), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        ProjectAssignement projectAssignement = new()
        {
            ProjectId = SelectedProject.Id,
            AccountId = accountId
        };
        ProjectAssignedAccounts.Add(Accounts.First(a => a.Id == accountId));
        OnPropertyChanged(nameof(ProjectAssignedAccounts));
        OnPropertyChanged(nameof(ProjectUnassignedAccounts));
        OnPropertyChanged(nameof(PAAFiltered));
        OnPropertyChanged(nameof(PUAFiltered));
        context.ProjectAssignements.Add(projectAssignement);
        await context.SaveChangesAsync();
        App.Current.LogAction(SecurityLog.AssignAccount, $"Assign account {Accounts.First(a => a.Id == accountId).FullName} ({accountId}) to project {SelectedProject.GetFullProjectTree}");
    }

    /// <summary>
    /// Désassigne un compte du projet sélectionné
    /// </summary>
    /// <param name="accountId">L'ID du compte à désassigner</param>
    private async void UnassignAccount(int accountId)
    {
        if (SelectedProject == null) { return; }

        if (SelectedProject.IsActive == false)
        {
            MessageBox.Show((string)Application.Current.FindResource("cannot_unassign_accounts"), (string)Application.Current.FindResource("error"), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        ProjectAssignement projectAssignement = await context.ProjectAssignements.FirstAsync(pa => pa.ProjectId == SelectedProject.Id && pa.AccountId == accountId);
        ProjectAssignedAccounts.Remove(Accounts.First(a => a.Id == accountId));
        OnPropertyChanged(nameof(ProjectAssignedAccounts));
        OnPropertyChanged(nameof(ProjectUnassignedAccounts));
        OnPropertyChanged(nameof(PAAFiltered));
        OnPropertyChanged(nameof(PUAFiltered));
        context.ProjectAssignements.Remove(projectAssignement);
        await context.SaveChangesAsync();
        App.Current.LogAction(SecurityLog.UnassignAccount, $"Unassign account {Accounts.First(a => a.Id == accountId).FullName} ({accountId}) from project {SelectedProject.GetFullProjectTree}");
    }

    /// <summary>
    /// Assigne un département au projet sélectionné
    /// </summary>
    /// <param name="departementId">L'ID du département à assigner</param>
    private async void AssignDepartement(int departementId)
    {
        if (SelectedProject == null) { return; }

        if (SelectedProject.IsActive == false)
        {
            MessageBox.Show((string)Application.Current.FindResource("cannot_assign_departments"), (string)Application.Current.FindResource("error"), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        ProjectAssignement projectAssignement = new()
        {
            ProjectId = SelectedProject.Id,
            DepartementId = departementId
        };
        ProjectAssignedDepartements.Add(Departements.First(d => d.Id == departementId));
        OnPropertyChanged(nameof(ProjectAssignedDepartements));
        OnPropertyChanged(nameof(ProjectUnassignedDepartements));
        context.ProjectAssignements.Add(projectAssignement);
        await context.SaveChangesAsync();
        App.Current.LogAction(SecurityLog.AssignDepartement, $"Assign departement {Departements.First(d => d.Id == departementId).Name} ({departementId}) to project {SelectedProject.GetFullProjectTree}");
    }

    /// <summary>
    /// Désassigne un département du projet sélectionné
    /// </summary>
    /// <param name="departementId">L'ID du département à désassigner</param>
    private async void UnassignDepartement(int departementId)
    {
        if (SelectedProject == null) { return; }

        if (SelectedProject.IsActive == false)
        {
            MessageBox.Show((string)Application.Current.FindResource("cannot_unassign_departments"), (string)Application.Current.FindResource("error"), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        ProjectAssignement projectAssignement = await context.ProjectAssignements.FirstAsync(pa => pa.ProjectId == SelectedProject.Id && pa.DepartementId == departementId);
        ProjectAssignedDepartements.Remove(Departements.First(d => d.Id == departementId));
        OnPropertyChanged(nameof(ProjectAssignedDepartements));
        OnPropertyChanged(nameof(ProjectUnassignedDepartements));
        context.ProjectAssignements.Remove(projectAssignement);
        await context.SaveChangesAsync();
        App.Current.LogAction(SecurityLog.UnassignDepartement, $"Unassign departement {Departements.First(d => d.Id == departementId).Name} ({departementId}) from project {SelectedProject.GetFullProjectTree}");
    }
}
