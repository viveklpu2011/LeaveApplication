using System;
using SQLite;

namespace SimpleLoginUI.Models;

public class EmployeeMaster
{
    [PrimaryKey, AutoIncrement]
    public int EmpCode { get; set; }
    public string Name { get; set; }
    public int ReportingManagerID { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public bool IsActive { get; set; }
}

