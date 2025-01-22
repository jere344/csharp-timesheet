using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;
using wisecorp.Context;
using wisecorp.Models;
using wisecorp.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using wisecorp.Views;
using wisecorp.Helpers;
using System.Diagnostics;

namespace wisecorp.ViewModels;

public partial class VMTimeSheet : ObservableObject
{
    private readonly WisecorpContext context;

    private Account _account;
    public Account Account
    {
        get => _account;
        set => SetProperty(ref _account, value);
    }

    private ObservableCollection<ProjectTask> _rootProjectTasks = new();
    public ObservableCollection<ProjectTask> RootProjectTasks
    {
        get => _rootProjectTasks;
        set => SetProperty(ref _rootProjectTasks, value);
    }


    // Instead of storing the calendar output directly, we store it in a temporary variable
    // and when the user clicks on the "Apply" button, we copy the value to the SelectedDate property
    private DateTime _beforeSelectedDate = DateTime.Today;
    public DateTime BeforeSelectedDate
    {
        get => _beforeSelectedDate;
        set {
            SetProperty(ref _beforeSelectedDate, value);
            OnPropertyChanged(nameof(CanApplySelectedDate));
        }
    }


    private DateTime _selectedDate = DateTime.Today;
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set {
            CurrentWeek = value.AddDays(-(int)value.DayOfWeek);
            _ = InitializeAsync(); // we reload the timesheet with the selected week
            SetProperty(ref _selectedDate, value);
            OnPropertyChanged(nameof(CanApplySelectedDate));
        }
    }
    // if the selected date is in the current week, no need to apply it
    public bool CanApplySelectedDate => BeforeSelectedDate.AddDays(-(int)BeforeSelectedDate.DayOfWeek) != CurrentWeek;

    [RelayCommand]
    public void ApplySelectedDate()
    {
        SelectedDate = BeforeSelectedDate;
    }

    private DateTime _currentWeek;
    public DateTime CurrentWeek { 
        get => _currentWeek;
        set {
            SetProperty(ref _currentWeek, value);
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(CanDeposit));
        }
    }

    public decimal TotalHour { 
        get {
            decimal total = 0;
            foreach (ProjectTask project in RootProjectTasks)
            {
                foreach (Work work in project.Works)
                {
                    total += work.HourWorkedSun ?? 0;
                    total += work.HourWorkedMon ?? 0;
                    total += work.HourWorkedTue ?? 0;
                    total += work.HourWorkedWed ?? 0;
                    total += work.HourWorkedThur ?? 0;
                    total += work.HourWorkedFri ?? 0;
                    total += work.HourWorkedSat ?? 0;
                }
            }
            return total;
        }
    }



    public string Title => string.Format((string)Application.Current.FindResource("timeSheetTitle"), Account.FullName, CurrentWeek.ToShortDateString(), CurrentWeek.AddDays(6).ToShortDateString());
    
    // when rejected, the submit is set back to false (so the user can submit again)
    public bool IsWeekApproved => RootProjectTasks.Any(p => p.Works.Any(w => w.IsApproved));
    public bool IsWeekRejected => RootProjectTasks.Any(p => p.Works.Any(w => w.IsRejected));
    public bool IsWeekSubmitted => RootProjectTasks.Any(p => p.Works.Any(w => w.IsSubmitted));
    public string? RejectedReason => RootProjectTasks.SelectMany(p => p.Works).FirstOrDefault(w => w.IsRejected)?.RejectedReason;
    public bool IsWeekPending => IsWeekSubmitted && !IsWeekApproved;

    // only for ui purposes
    public bool IsWeekRejectedAndNotApproved => IsWeekRejected && !IsWeekApproved;

    /// <summary>
    /// Met à jour l'état des propriétés liées à l'approbation de la semaine.
    /// Cette méthode notifie l'interface utilisateur des changements d'état
    /// des propriétés concernées, permettant ainsi une mise à jour dynamique.
    /// </summary>
    public void UpdateStatus()
    {
        OnPropertyChanged(nameof(IsWeekApproved));
        OnPropertyChanged(nameof(IsWeekRejected));
        OnPropertyChanged(nameof(IsWeekSubmitted));
        OnPropertyChanged(nameof(IsWeekPending));
        OnPropertyChanged(nameof(RejectedReason));
        OnPropertyChanged(nameof(IsWeekRejectedAndNotApproved));
    }
    
    private bool _canSaveHours = false;
    public bool CanSaveHours
    {
        get => _canSaveHours;
        set {
            if (!IsWeekSubmitted) {
                SetProperty(ref _canSaveHours, value);
                OnPropertyChanged(nameof(CanDeposit));
            }
        }
    }

    public bool CanDeposit => !CanSaveHours && !IsWeekSubmitted;

    public VMTimeSheet()
    {
        // If empty, we use the App.CurrentUser
        if (App.Current.ConnectedAccount == null) { throw new Exception("No account connected"); }
        Account = App.Current.ConnectedAccount;
        context = new WisecorpContext();

        CurrentWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

        SaveHour = new RelayCommand(_saveHour);
        DepositeTimeSheet = new RelayCommand(_depositeTimeSheet);

        _ = InitializeAsync();
    }

    public VMTimeSheet(WisecorpContext context)
    {
        this.context = context;
        Account = context.Accounts.FirstOrDefault();

        CurrentWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

        SaveHour = new RelayCommand(_saveHour);
        DepositeTimeSheet = new RelayCommand(_depositeTimeSheet);

        _ = InitializeAsync();
    }


    /// <summary>
    /// Permet d'initiliser les listes de maniere asynchronne
    /// </summary>
    /// <returns></returns>
    private async Task InitializeAsync()
    {
        // Detach all tracked entities because we don't want to keep them in memory while moving to the next week
        // (it would mean that saving the timesheet would save all the previous/future weeks which have not been saved which is unintuitive)
        context.ChangeTracker.Clear(); 

        List<ProjectAssignement> assignements = await context.ProjectAssignements
            .Include(p => p.Project)
            .ThenInclude(p => p.SubProjects)
            .Where(p =>
                p.Project.ParentProjectId == null // Si c'est un projet root
                && p.Project.StartDate < (CurrentWeek + new TimeSpan(7, 0, 0, 0)) // Si le projet a été actif avant la semaine courante
        ).ToListAsync(); 

        RootProjectTasks.Clear();
        foreach (ProjectAssignement assignement in assignements)
        {
            if (assignement.Project == null) continue;
            if (RootProjectTasks.Any(p => p.MainProject.Id == assignement.Project.Id)) continue;
            if (assignement.Project.IsAssigned(Account) == false) continue;

            ProjectTask newRootProject = new(assignement.Project);
            newRootProject.FetchTasks(assignement.Project);
            newRootProject.FetchWorks(context, Account, CurrentWeek);
            RootProjectTasks.Add(newRootProject);
        }

        // now we create a fake root that contains all the works that are not assigned to any project (deleted projects)
        ProjectTask deletedProjects = new(new Project { Name = "Projets supprimés" });
        List<Work> deletedWorks = await context.Works.Include(w => w.Project).Where(w => w.Project.IsActive == false && w.AccountId == Account.Id && w.WeekStartDate == CurrentWeek).ToListAsync();
        deletedProjects.Works = new(deletedWorks);
        if (deletedProjects.Works.Count > 0)
        {
            RootProjectTasks.Add(deletedProjects);
        }

        UpdateStatus();

        OnPropertyChanged(nameof(CanDeposit));
        OnPropertyChanged(nameof(CanSaveHours));
        OnPropertyChanged(nameof(TotalHour));

        // We want all the displayed work to be on the same status.
        // So we remove all non-conforming works
        // That way if we have half the projects which are rejected and half which are approved, we will only display the rejected ones
        // so the user can see the rejected ones and fix them

        // So the order of the following if statements is important

        // if the week is rejected, we remove the works that are not rejected
        if (IsWeekRejected)
        {
            foreach (ProjectTask project in RootProjectTasks)
            {
                project.Works = new(project.Works.Where(w => w.IsRejected));
            }
            UpdateStatus();
        }

        // the week is approved, we remove the works that are not approved
        if (IsWeekApproved)
        {
            foreach (ProjectTask project in RootProjectTasks)
            {
                project.Works = new(project.Works.Where(w => w.IsApproved));
            }
            UpdateStatus();
        }

        // if the week is submitted, we remove the works that are not submitted
        if (IsWeekSubmitted)
        {
            foreach (ProjectTask project in RootProjectTasks)
            {
                project.Works = new(project.Works.Where(w => w.IsSubmitted));
            }
            UpdateStatus();
        }
        
    }

    /// <summary>
    /// Permet de sauvegarder les heures
    /// </summary>
    public ICommand SaveHour { get; }
    private async void _saveHour()
    {
        await context.SaveChangesAsync();      
        OnPropertyChanged(nameof(TotalHour));
        CanSaveHours = false;
    }

    /// <summary>
    /// Permet de d�poser la feuille de temps
    /// </summary>
    public ICommand DepositeTimeSheet { get; }
    private async void _depositeTimeSheet()
    {
        // confirm
        MessageBoxResult result = MessageBox.Show((string)Application.Current.FindResource("confirmDeposit"), (string)Application.Current.FindResource("confirmDepositTitle"), MessageBoxButton.YesNo);
        if (result == MessageBoxResult.No) return;
        foreach (ProjectTask project in RootProjectTasks)
        {
            foreach (Work work in project.Works)
            {
                work.IsSubmitted = true;
            }
        }

        LogTimesheetDeposit();
        RootProjectTasks.Clear();
        await InitializeAsync();
    }

    /// <summary>
    /// Permet de logger ce qu'il a �t� fait
    /// </summary>
    private void LogTimesheetDeposit()
    {

        SecurityLog log = new SecurityLog
        {
            AccountId = Account.Id,
            Code = SecurityLog.FDTRemise,
            Date = DateTime.Now,
            Ip = App.GetIPAddress(),
            Description = "Feuille de temps remise avec succes"
        };
        context.SecurityLogs.Add(log);
        context.SaveChanges();
    }
    /// <summary>
    /// Permets l'export de la current timesheet en pdf
    /// </summary>
    [RelayCommand]
    public void ExportPdf()
    {
        PdfGenerator.GeneratePdf(Account, CurrentWeek, RootProjectTasks.ToList());
    }

    /// <summary>
    /// Permet d'exporter en excel
    /// </summary>
    [RelayCommand]
    public void ExportToXlsx()
    {
        XlsxGenerator.GenerateXlsx(Account, CurrentWeek, RootProjectTasks.ToList());
    }

    /// <summary>
    /// Permet d'ouvrir la window pour mettre des heures
    /// </summary>
    public void OpenHourWindow(Work work, int day)
    {
        var vm = new VMAddHour(work, day);
        Window window = new AddHour
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };

        window.ShowDialog();
        
        if (vm.Saving)
        {
            RootProjectTasks.FirstOrDefault(pt => pt.Works.Contains(work))?.RefreshWorks();
            CanSaveHours = true;
        }
    }

    private bool _showAdditionalActions = false;
    public bool ShowAdditionalActions
    {
        get => _showAdditionalActions;
        set => SetProperty(ref _showAdditionalActions, value);
    }

}