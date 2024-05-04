using System;
using SQLite;

namespace SimpleLoginUI.Models;

public class EmployeeLeaveBalanceMaster
{
    [PrimaryKey, AutoIncrement]
    public int TypeID { get; set; }
    public int EmployeeId { get; set; }
    public int AllowedLeave { get; set; }
    public int? ConsumedLeave { get; set; }
}

