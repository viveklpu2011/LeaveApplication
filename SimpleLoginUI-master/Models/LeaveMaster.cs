using System;
using SQLite;

namespace SimpleLoginUI.Models;

public class LeaveMaster
{
    [PrimaryKey, AutoIncrement]
    public int ApplicationID { get; set; }
    public int EmployeeId { get; set; }
    public int ReportingManagerId { get; set; }
    public int LeaveTypeId { get; set; }
    public string AppStatus { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfDays { get; set; }
    public int AppliedBy { get; set; }
    public string Purpose { get; set; }
    public string Reason { get; set; }
    public DateTime AppliedOn { get; set; }
    public int VerifiedBy { get; set; }
    public DateTime VerifiedOn { get; set; }
}

public class LeaveMasterClass
{
    public int ApplicationID { get; set; }
    public int EmployeeId { get; set; }
    public int ReportingManagerId { get; set; }
    public int LeaveTypeId { get; set; }
    public string AppStatus { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfDays { get; set; }
    public int AppliedBy { get; set; }
    public string Purpose { get; set; }
    public string ReportingManagerName { get; set; }
    public string EmployeeName { get; set; }
    public string Reason { get; set; }
    public string LeaveType { get; set; }
    public DateTime AppliedOn { get; set; }
    public int VerifiedBy { get; set; }
    public DateTime VerifiedOn { get; set; }
}

