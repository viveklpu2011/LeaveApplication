using System;
using SimpleLoginUI.Models;
using SQLite;

namespace SimpleLoginUI.Handlers;

public class SQLiteDBContext
{
    SQLiteAsyncConnection Database;
    public SQLiteDBContext()
    {
    }

    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        await CreateTables();
    }

    public async Task CreateTables()
    {
        await Database.CreateTableAsync<AppStatusTypeMaster>();
        await Database.CreateTableAsync<EmployeeLeaveBalanceMaster>();
        await Database.CreateTableAsync<EmployeeMaster>();
        await Database.CreateTableAsync<LeaveMaster>();
        await Database.CreateTableAsync<LeaveTypeMaster>();
        await Database.CreateTableAsync<ReportingManagerMaster>();
    }

    public async Task<int> SaveApplicationStatusAsync(AppStatusTypeMaster item)
    {
        await Init();
        if (item.AppStatusId != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    public async Task<int> SaveEmployeeLeaveBalanceMasterAsync(EmployeeLeaveBalanceMaster item)
    {
        await Init();
        if (item.TypeID != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    public async Task<int> SaveEmployeeMasterAsync(EmployeeMaster item)
    {
        await Init();
        if (item.EmpCode != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    public async Task<int> SaveLeaveMasterAsync(LeaveMaster item)
    {
        await Init();
        if (item.ApplicationID != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    public async Task<int> SaveLeaveTypeMasterAsync(LeaveTypeMaster item)
    {
        await Init();
        if (item.TypeID != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    public async Task<int> SaveReportingManagerMasterAsync(ReportingManagerMaster item)
    {
        await Init();
        if (item.ManagerId != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    public async Task<List<AppStatusTypeMaster>> GetApplicationStatusListAsync()
    {
        await Init();
        return await Database.Table<AppStatusTypeMaster>().ToListAsync();

        // SQL queries are also possible
        //return await Database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
    }

    public async Task<List<EmployeeLeaveBalanceMaster>> GetEmployeeLeaveBalanceListAsync()
    {
        await Init();
        return await Database.Table<EmployeeLeaveBalanceMaster>().ToListAsync();
    }

    public async Task<List<EmployeeMaster>> GetEmployeeListAsync()
    {
        await Init();
        return await Database.Table<EmployeeMaster>().ToListAsync();
    }

    public async Task<List<LeaveMaster>> GetLeaveListAsync()
    {
        await Init();
        return await Database.Table<LeaveMaster>().ToListAsync();
    }

    public async Task<List<LeaveTypeMaster>> GetLeaveTypeListAsync()
    {
        await Init();
        return await Database.Table<LeaveTypeMaster>().ToListAsync();
    }

    public async Task<List<ReportingManagerMaster>> GetReportingManagerListAsync()
    {
        try
        {
            await Init();
            var list = await Database.Table<ReportingManagerMaster>().ToListAsync();
            return list;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<EmployeeMaster> GetEmployeeAsync(string phone, string name)
    {
        await Init();
        return await Database.Table<EmployeeMaster>().Where(i => i.Name == name && i.MobileNumber == phone).FirstOrDefaultAsync();
    }

    public async Task<ReportingManagerMaster> GetManagerAsync(string phone, string name)
    {
        await Init();
        return await Database.Table<ReportingManagerMaster>().Where(i => i.ManagerName == name && i.Mobile == phone).FirstOrDefaultAsync();
    }
}

