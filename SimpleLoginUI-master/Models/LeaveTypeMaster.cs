using System;
using SQLite;

namespace SimpleLoginUI.Models;

public class LeaveTypeMaster
{
	[PrimaryKey,AutoIncrement]
	public int TypeID { get; set; }
	public string Type { get; set; }
	public int Limit { get; set; }
	public bool IsActive { get; set; }
}


