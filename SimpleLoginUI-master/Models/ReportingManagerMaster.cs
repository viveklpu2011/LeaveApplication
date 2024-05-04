using System;
using SQLite;

namespace SimpleLoginUI.Models;

public class ReportingManagerMaster
{
    [PrimaryKey, AutoIncrement]
    public int ManagerId { get; set; }
    public string ManagerName { get; set; }
    public string Designamtion { get; set; }
    public string Mobile { get; set; }
}

