using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace wisecorp.Models.DBModels;

public class Project : BaseModel
{ 
    public string Name { get; set; }
    public int CreatorId { get; set; }
    public int? ParentProjectId { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    private double _budget;
	public double GetOriginalBudget { get => _budget; }
	public double Budget { 
        get {
            if (SubProjects != null && SubProjects.Count > 0)
            {
                return SubProjects.Sum(p => p.Budget);
            }
            return _budget;
        }
        set {
            if (SubProjects != null && SubProjects.Count > 0)
            {
                throw new InvalidOperationException("Cannot set budget on a project with subprojects");
            }
            _budget = value;
        }
    }
    private double _nbHour;
    public double GetOriginalNbHour { get => _nbHour; }
    public double NbHour { 
        get {
            if (SubProjects != null && SubProjects.Count > 0)
            {
                return SubProjects.Sum(p => p.NbHour);
            }
            return _nbHour;
        }
        set {
            if (SubProjects != null && SubProjects.Count > 0)
            {
                throw new InvalidOperationException("Cannot set nbHours on a project with subprojects");
            }
            _nbHour = value;
        }
    }
    public bool IsActive { get; set; }
    
    //Nav propreties
    public virtual Project? ParentProject { get; set; }
    public virtual Account Creator { get; set; }

    public virtual ICollection<Project> SubProjects { get; set; }
    public virtual ICollection<Work> Works { get; set; }
    public virtual ICollection<ProjectAssignement> ProjectAssignements { get; set; }

    // computed properties
    public bool CanEditBudget => SubProjects == null || SubProjects.Count == 0;
    public bool CanEditNbHour => SubProjects == null || SubProjects.Count == 0;
    public string GetFullProjectTree => ParentProject != null ? ParentProject.GetFullProjectTree + " > " + Name : Name;
    public string GetTruncatedFullProjectTree
    {
        get
        {
            const int maxLength = 50;
            const int sideLength = 25;
            string tree = GetFullProjectTree;

            if (tree.Length <= maxLength)
            {
                return tree;
            }

            // Find the last space within the limit for the start
            int start = tree.LastIndexOf(' ', maxLength - sideLength);
            if (start == -1)
            {
                start = maxLength - sideLength;
            }

            // Find the last space within the limit for the end
            int end = tree.LastIndexOf(' ', maxLength);
            if (end == -1)
            {
                end = maxLength;
            }

            return tree[..start] + " ... " + tree[end..];
        }
    }

    public int TotalNbSubProjects => SubProjects?.Count + SubProjects?.Sum(p => p.TotalNbSubProjects) ?? 0;

    [NotMapped] 
    public bool IsExpanded { get; set; }

    [NotMapped] 
    public bool IsSelected { get; set; }

    [NotMapped]
    public ObservableCollection<Project> ObservableSubProjects => new(SubProjects ?? new List<Project>());

    private static bool DisplayDisabledProjects => ((App)Application.Current).SavedSettings["DisplayDisabledProjects"] as bool? ?? false;
    [NotMapped]
    public ObservableCollection<Project> ObservableEnabledSubProjects => new(SubProjects?.Where(p => p.IsActive || DisplayDisabledProjects) ?? new List<Project>());

    /// <summary>
    /// Recursive method to disable a project and all its subprojects
    /// </summary>
    public void Disable()
    {
        IsActive = false;
        foreach (var subProject in SubProjects ?? new List<Project>())
        {
            subProject.Disable();
        }
    }

    /// <summary>
    /// recursive method to enable a project and all its subprojects
    /// </summary>
    public void Enable()
    {
        IsActive = true;
        foreach (var subProject in SubProjects ?? new List<Project>())
        {
            subProject.Enable();
        }
    }

    /// <summary>
    /// Check if the user can put hours on this project
    /// He can if he or his departement is assigned to the project or one of the parent projects
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public bool IsAssigned(Account account)
    {
        if (ProjectAssignements.Any(p => p.AccountId == account.Id || p.DepartementId == account.DepartementId))
        {
            return true;
        }
        if (ParentProject != null)
        {
            return ParentProject.IsAssigned(account);
        }
        return false;
    }
}

