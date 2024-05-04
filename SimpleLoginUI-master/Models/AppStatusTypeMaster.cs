using System;
using SQLite;

namespace SimpleLoginUI.Models;

public class AppStatusTypeMaster
{
    [PrimaryKey, AutoIncrement]
    public int AppStatusId { get; set; }
    public string StatusName { get; set; }
    public string StatusType { get; set; }
}

