namespace wisecorp.Models.DBModels;

public class VerificationCode : BaseModel
{ 
    public string Code { get; set; }
    public int AccountId { get; set; }
    public DateTime ExpirationDate { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; }
}
