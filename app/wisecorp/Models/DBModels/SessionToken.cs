namespace wisecorp.Models.DBModels;

public class SessionToken : BaseModel
{ 
    public int AccountId { get; set; }
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }

    // Navigation properties
    public virtual Account Account { get; set; }
}
