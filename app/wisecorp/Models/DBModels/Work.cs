using System.ComponentModel;

namespace wisecorp.Models.DBModels;

public class Work : BaseModel
{ 
    public int ProjectId { get; set; }
    public int? AccountId { get; set; }
    public DateTime WeekStartDate { get; set; }

    public bool IsSubmitted { get; set; } = false;
    public bool IsApproved { get; set; } = false;
    public bool IsRejected { get; set; } = false;
    public string? RejectedReason { get; set; }

    public decimal? HourWorkedSun { get; set; }
    public decimal? HourWorkedMon { get; set; }
    public decimal? HourWorkedTue { get; set; }
    public decimal? HourWorkedWed { get; set; }
    public decimal? HourWorkedThur { get; set; }
    public decimal? HourWorkedFri { get; set; }
    public decimal? HourWorkedSat { get; set; }

    public string? CommentSun { get; set; }
    public string? CommentMon { get; set; }
    public string? CommentTue { get; set; }
    public string? CommentWed { get; set; }
    public string? Commenthur { get; set; }
    public string? CommentFri { get; set; }
    public string? CommentSat { get; set; }



    //Nav propreties
    public virtual Project Project { get; set; }
    public virtual Account Account { get; set; }

    // computed properties
    public decimal TotalHours => HourWorkedSun ?? 0 + HourWorkedMon ?? 0 + HourWorkedTue ?? 0  + HourWorkedWed ?? 0  + HourWorkedThur ?? 0  + HourWorkedFri ?? 0  + HourWorkedSat ?? 0 ;
    public bool HasHours => TotalHours > 0;

    public bool IsEnabled => !IsSubmitted && Project?.IsActive == true;
}
