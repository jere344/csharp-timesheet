namespace wisecorp.Models.DBModels;

public class SecurityLog : BaseModel
{ 
    public string Code { get; set; }
    public string Description { get; set; }
    public string Ip { get; set; }
    public int? AccountId { get; set; }
    public DateTime Date { get; set; }

    public bool HasMoreInfo => Description.Contains('\n');

    //Nav propreties
    public virtual Account Account { get; set; }

    public const string LoginSuccess = "LoginSuccess";
    public const string LoginFailed = "LoginFailed";
    public const string SendSecurityCode = "SendSecurityCode";

    public const string AddAccount = "AddAccount";
    public const string EditAccount = "EditAccount";
    public const string DeleteAccount = "DeactivatedAccount";

    public const string AddProject = "AddProject";
    public const string EditProject = "EditProject";
    public const string DeleteProject = "DeactivatedProject";
    public const string DefinitiveDeleteProject = "DefinitiveDeleteProject";

    public const string AssignDepartement = "AssignDepartement";
    public const string UnassignDepartement = "UnassignDepartement";
    public const string AssignAccount = "AssignAccount";
    public const string UnassignAccount = "UnassignAccount";
    
    public const string FDTRemise = "RemiseFeuilleDeTemps";
}