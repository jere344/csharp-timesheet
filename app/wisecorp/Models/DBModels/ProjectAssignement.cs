namespace wisecorp.Models.DBModels;

public class ProjectAssignement : BaseModel
{ 
    public int ProjectId { get; set; }
    // need to have either a account or a departement assigned.
    public int? AccountId { get; set; } 
    public int? DepartementId { get; set; }

    //Nav propreties
    public virtual Project Project { get; set; }
    public virtual Account? Account { get; set; }
    public virtual Departement? Departement { get; set; }
}
