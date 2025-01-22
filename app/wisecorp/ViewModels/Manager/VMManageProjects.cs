using CommunityToolkit.Mvvm.ComponentModel;
using wisecorp.Models.DBModels;
using System.Windows;
using wisecorp.Context;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.ObjectModel;
using wisecorp.Models.Graphs;
using System.Windows.Controls;
using wisecorp.Views.Components;
using System.Windows.Threading;

namespace wisecorp.ViewModels;

public class VMManageProjects : ObservableObject
{
    public WisecorpContext Context { get; set; }


    private List<Project> _parentsProjects;
    public List<Project> ParentsProjects
    {
        get => _parentsProjects;
        set => SetProperty(ref _parentsProjects, value);
    }

    private Project? _selectedProject;
    public Project? SelectedProject
    {
        get => _selectedProject;
        set
        {
            SetProperty(ref _selectedProject, value);
            OnPropertyChanged(nameof(IsProjectSelected));
            OnPropertyChanged(nameof(ShowDeleteSelectedProject));
            OnPropertyChanged(nameof(ShowRestoreSelectedProject));
            OnPropertyChanged(nameof(CanEditSelectedProject));
            OnPropertyChanged(nameof(CannotEditSelectedProject));
            OnPropertyChanged(nameof(ShowDuplicateSelectedProject));
            OnPropertyChanged(nameof(CannotEditSelectedProjectBudget));
            OnPropertyChanged(nameof(CannotEditSelectedProjectNbHour));

            // it create aditional request, so we only fetch the data if the panel is shown
            if (ShowAssignProjectsPanel == true)
                VMAssignProjects.SelectedProject = value;
        }
    }

    private bool _showDisabledProjects = ((App)Application.Current).SavedSettings["DisplayDisabledProjects"] as bool? ?? false;
    public bool ShowDisabledProjects
    {
        get => _showDisabledProjects;
        set
        {
            SetProperty(ref _showDisabledProjects, value);
            ((App)Application.Current).SavedSettings["DisplayDisabledProjects"] = value;
            FetchProjects();
        }
    }

    public bool IsProjectSelected => SelectedProject != null;
    public bool ShowDeleteSelectedProject => SelectedProject != null && SelectedProject.IsActive;
    public bool ShowRestoreSelectedProject => SelectedProject != null && !SelectedProject.IsActive;
    public bool ShowDuplicateSelectedProject => SelectedProject != null && SelectedProject.IsActive;
    public bool CanEditSelectedProject => SelectedProject != null && SelectedProject.IsActive && (SelectedProject.CreatorId == App.Current.ConnectedAccount?.Id || App.Current.ConnectedAccount?.Role.Name == "Admin");
    public bool CannotEditSelectedProject => !CanEditSelectedProject;
    public bool CannotEditSelectedProjectBudget => SelectedProject == null || SelectedProject.CanEditBudget == false;
    public bool CannotEditSelectedProjectNbHour => SelectedProject == null || SelectedProject.CanEditNbHour == false;
    public bool CanAddSubProject => !(SelectedProject != null && !SelectedProject.IsActive);

    private bool _showAssignProjectsPanel = false;
    public bool ShowAssignProjectsPanel
    {
        get => _showAssignProjectsPanel;
        set
        {
            SetProperty(ref _showAssignProjectsPanel, value);
            if (value == true)
            {
                VMAssignProjects.SelectedProject = SelectedProject;
            }
        }
    }

    public VMAssignProjects VMAssignProjects { get; } = new VMAssignProjects();
    public ICommand DeleteSelectedProjectCommand { get; }
    public ICommand RestoreSelectedProjectCommand { get; }
    public ICommand AddSubProjectCommand { get; }
    public ICommand DuplicateSelectedProjectCommand { get; }
    public ICommand SaveSelectedProjectCommand { get; }

    private int _selectedTabIndex = 0;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            SetProperty(ref _selectedTabIndex, value);
            SelectedProject = null;
            if (value == 1)
            {
                DrawGraph();
            }
        }
    }


    #region graph specicfics
    private ProjectTree _graph;
    public ProjectTree Graph
    {
        get => _graph;
        set => SetProperty(ref _graph, value);
    }
    private ZoomableCanvas _zoomableCanvasControl;
    public ZoomableCanvas ZoomableCanvasControl
    {
        get => _zoomableCanvasControl;
        set => SetProperty(ref _zoomableCanvasControl, value);
    }
    private void BuildGraph()
    {
        Graph = new ProjectTree();
        Graph.Settings.OnBeforeEdgeRemoved = (parent, child) => { return true; };
        Graph.Settings.OnEdgeRemoved = (parent, child) => {
            MoveProject(child.Data, null);
            DrawGraph();
        };
        Graph.Settings.OnBeforeEdgeAdded = (parent, child) => { return true; };
        Graph.Settings.OnEdgeAdded = (parent, child) => {
            MoveProject(child.Data, parent.Data);
            DrawGraph();
        };
        Graph.Settings.OnNodeClicked = project =>{
            SelectedProject = project;
        };
        Graph.Settings.GetNodeLabel = project => project.Name;

        Graph.InitializeEmptyTreeGraph();

        foreach (Project project in ParentsProjects)
        {
            Graph.AddRoot(project);
        }
    }
    private void DrawGraph()
    {
        ZoomableCanvasControl.GraphCanvas.Children.Clear();
        BuildGraph();
        Graph.DrawGraph(ZoomableCanvasControl.GraphCanvas);
        ZoomableCanvasControl.SceduleCenterCanvas();
    }
    #endregion

    public VMManageProjects()
    {
        Context = new WisecorpContext();
        FetchProjects();

        DeleteSelectedProjectCommand = new RelayCommand(DeleteSelectedProject);
        RestoreSelectedProjectCommand = new RelayCommand(RestoreSelectedProject);
        AddSubProjectCommand = new RelayCommand(AddSubProject);
        DuplicateSelectedProjectCommand = new RelayCommand(DuplicateSelectedProject);
        SaveSelectedProjectCommand = new RelayCommand(SaveSelectedProject);
    }

    /// <summary>
    /// Supprime le projet sélectionné ou le désactive selon les conditions
    /// </summary>
    private void DeleteSelectedProject()
    {
        if (SelectedProject == null) { return; }

        // if there isn't any work with hours in the project and no subprojects, we can delete it permanently
        if ((SelectedProject.SubProjects == null || SelectedProject.SubProjects.Count == 0) &&
            (SelectedProject.Works == null || SelectedProject.Works.FirstOrDefault(w => w.HasHours) == null))
        {
            if (MessageBox.Show((string)Application.Current.FindResource("delete_project_confirmation_no_hours"), (string)Application.Current.FindResource("delete_project"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ReallyDeleteProject(SelectedProject);
            }
            else
            {
                SelectedProject.Disable();
                Context.SaveChanges();
                App.Current.LogAction(SecurityLog.DeleteProject, $"Project {SelectedProject.Name} ({SelectedProject.Id}) deleted");
                FetchProjects();
            }
        }
        else
        {
            if (MessageBox.Show((string)Application.Current.FindResource("delete_project_confirmation"), (string)Application.Current.FindResource("delete_project"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SelectedProject.Disable();
                Context.SaveChanges();
                App.Current.LogAction(SecurityLog.DeleteProject, $"Project {SelectedProject.Name} ({SelectedProject.Id}) deleted");
                FetchProjects();
            }
        }
    }

    /// <summary>
    /// Supprime définitivement un projet de la base de données
    /// </summary>
    /// <param name="projectToDelete">Le projet à supprimer</param>
    private void ReallyDeleteProject(Project projectToDelete)
    {
        Context.Projects.Remove(projectToDelete);
        Context.SaveChanges();
        App.Current.LogAction(SecurityLog.DefinitiveDeleteProject, $"Project {projectToDelete.Name} ({projectToDelete.Id}) definitively deleted");
        FetchProjects();
    }

    /// <summary>
    /// Restaure le projet sélectionné s'il était désactivé
    /// </summary>
    private void RestoreSelectedProject()
    {
        if (SelectedProject == null) { return; }

        // We can't restore a project if one of its parent is disabled
        var parent = SelectedProject.ParentProject;
        while (parent != null)
        {
            if (!parent.IsActive)
            {
                MessageBox.Show((string)Application.Current.FindResource("restore_project_disabled_parent"), (string)Application.Current.FindResource("restore_project"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            parent = parent.ParentProject;
        }

        SelectedProject.Enable();
        Context.SaveChanges();
        App.Current.LogAction(SecurityLog.EditProject, $"Project {SelectedProject.Name} ({SelectedProject.Id}) restored");
        FetchProjects();
    }

    /// <summary>
    /// Déplace un projet sous un nouveau projet parent
    /// </summary>
    /// <param name="projectToMove">Le projet à déplacer</param>
    /// <param name="newParent">Le nouveau projet parent</param>
    public void MoveProject(Project projectToMove, Project? newParent)
    {
        if (projectToMove == null)
        {
            return;
        }

        projectToMove.ParentProjectId = newParent?.Id;
        Context.SaveChanges();
        App.Current.LogAction(SecurityLog.EditProject, $"Project {projectToMove.Name} ({projectToMove.Id}) moved to {newParent?.Name ?? "root"}");
        FetchProjects();
    }

    /// <summary>
    /// Ajoute un nouveau sous-projet au projet sélectionné
    /// </summary>
    private void AddSubProject()
    {
        var name = "New project";
        if (SelectedProject != null)
        {
            name = SelectedProject.GetFullProjectTree + " > " + (SelectedProject.SubProjects?.Count + 1 ?? 1);
        }

        var newProject = new Project
        {
            Name = name,
            CreatorId = App.Current.ConnectedAccount?.Id ?? 0,
            ParentProjectId = SelectedProject?.Id,
            Description = "",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now,
            Budget = SelectedProject?.Budget ?? 0,
            NbHour = SelectedProject?.NbHour ?? 0,
            IsActive = SelectedProject?.IsActive ?? true
        };

        // if there is any hour worked on the project, we need to move them to the new subproject
        // we ask the user for confirmation
        if (SelectedProject?.Works != null && SelectedProject.Works.Any(w => w.HasHours))
        {
            if (!(MessageBox.Show((string)Application.Current.FindResource("move_hours_to_subproject"), (string)Application.Current.FindResource("move_hours"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
            {
                return;
            }
            
        }




        Context.Projects.Add(newProject);
        Context.SaveChanges();
        foreach (Work w in SelectedProject?.Works ?? new ObservableCollection<Work>())
        {
            w.ProjectId = newProject.Id;
        }
        Context.SaveChanges();

        if (SelectedProject != null)
            SelectedProject.IsExpanded = true;
        SelectedProject = newProject;
        App.Current.LogAction(SecurityLog.AddProject, $"Project {newProject.Name} ({newProject.Id}) added");
        FetchProjects();
    }

    /// <summary>
    /// Récupère les projets parents, avec option de filtrage des projets désactivés
    /// </summary>
    private void FetchProjects()
    {
        var pp = Context.Projects.Where(p => !p.ParentProjectId.HasValue);
        if (!ShowDisabledProjects)
        {
            pp = pp.Where(p => p.IsActive);
        }
        ParentsProjects = pp.ToList();

        if (SelectedTabIndex == 1)
        {
            DrawGraph();
        }
    }

    /// <summary>
    /// Duplique le projet sélectionné, y compris ses sous-projets
    /// </summary>
    private void DuplicateSelectedProject()
    {
        if (SelectedProject == null) { return; }

        int number = SelectedProject.TotalNbSubProjects + 1;
        if (number > 5)
        {
            if (MessageBox.Show(((string)Application.Current.FindResource("duplicate_project_confirmation")).Replace("{0}", number.ToString()), (string)Application.Current.FindResource("duplicate_project"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
        }

        var newProject = new Project
        {
            Name = SelectedProject.Name + " (copy)",
            CreatorId = App.Current.ConnectedAccount?.Id ?? 0,
            ParentProjectId = SelectedProject.ParentProjectId,
            Description = SelectedProject.Description,
            StartDate = SelectedProject.StartDate,
            EndDate = SelectedProject.EndDate,
            Budget = SelectedProject.Budget,
            NbHour = SelectedProject.NbHour,
            IsActive = SelectedProject.IsActive
        };
        Context.Projects.Add(newProject);
        Context.SaveChanges();
        App.Current.LogAction(SecurityLog.AddProject, $"Project {newProject.Name} ({newProject.Id}) added");

        DuplicateSubProjects(SelectedProject, newProject);

        FetchProjects();
    }


    /// <summary>
    /// Duplique récursivement les sous-projets d'un projet
    /// </summary>
    /// <param name="originalProject">Le projet original</param>
    /// <param name="newParentProject">Le nouveau projet parent</param>
    private void DuplicateSubProjects(Project originalProject, Project newParentProject)
    {
        foreach (var subProject in originalProject.SubProjects ?? new ObservableCollection<Project>())
        {
            var duplicatedSubProject = new Project
            {
                Name = subProject.Name + " (copy)",
                CreatorId = App.Current.ConnectedAccount?.Id ?? 0,
                ParentProjectId = newParentProject.Id,
                Description = subProject.Description,
                StartDate = subProject.StartDate,
                EndDate = subProject.EndDate,
                Budget = subProject.Budget,
                NbHour = subProject.NbHour,
                IsActive = subProject.IsActive
            };

            Context.Projects.Add(duplicatedSubProject);
            Context.SaveChanges();
            App.Current.LogAction(SecurityLog.AddProject, $"Project {duplicatedSubProject.Name} ({duplicatedSubProject.Id}) added");
            DuplicateSubProjects(subProject, duplicatedSubProject);
        }
    }

    /// <summary>
    /// Sauvegarde les modifications apportées au projet sélectionné et journalise les changements
    /// </summary>
    private void SaveSelectedProject()
    {
        if (SelectedProject == null) { return; }

        // we need to make a list of changes to log them
        // for this we compare the project in the context with the one in the database
        var projectInContext = Context.Projects.AsNoTracking().FirstOrDefault(p => p.Id == SelectedProject.Id);
        if (projectInContext == null) {return;}

        var changes = new List<string>();
        if (projectInContext.Name != SelectedProject.Name)
            changes.Add($"Name: {projectInContext.Name} -> {SelectedProject.Name}");
        if (projectInContext.Description != SelectedProject.Description)
            changes.Add($"Description: {projectInContext.Description} -> {SelectedProject.Description}");
        if (projectInContext.StartDate != SelectedProject.StartDate)
            changes.Add($"Start date: {projectInContext.StartDate} -> {SelectedProject.StartDate}");
        if (projectInContext.EndDate != SelectedProject.EndDate)
            changes.Add($"End date: {projectInContext.EndDate} -> {SelectedProject.EndDate}");
        if (projectInContext.IsActive != SelectedProject.IsActive)
            changes.Add($"Active: {projectInContext.IsActive} -> {SelectedProject.IsActive}");
        // only log budget and nbHour changes if the project has no subprojects
        if (SelectedProject.SubProjects != null && (SelectedProject.SubProjects.Count == 0) && projectInContext.GetOriginalBudget != SelectedProject.Budget)
            changes.Add($"Budget: {projectInContext.GetOriginalBudget} -> {SelectedProject.Budget}");
        if (SelectedProject.SubProjects != null && (SelectedProject.SubProjects.Count == 0) && projectInContext.GetOriginalNbHour != SelectedProject.NbHour)
            changes.Add($"NbHour: {projectInContext.GetOriginalNbHour} -> {SelectedProject.NbHour}");

        if (changes.Count > 0)
        {
            App.Current.LogAction(SecurityLog.EditProject, $"Project {SelectedProject.Name} ({SelectedProject.Id}) edited: \n{string.Join("\n", changes)}");
        }

        Context.SaveChanges();
        
        FetchProjects();
    }
}
