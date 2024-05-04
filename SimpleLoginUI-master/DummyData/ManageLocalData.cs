using System;
using SimpleLoginUI.Handlers;
using SimpleLoginUI.Models;
using System.Linq;
using static Android.Content.ClipData;

namespace SimpleLoginUI.DummyData;

public sealed class ManageLocalData
{
    private static ManageLocalData instance = null;

    private ManageLocalData()
    {
    }

    public static ManageLocalData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ManageLocalData();
                database = new SQLiteDBContext();
            }
            return instance;
        }
    }

    private static SQLiteDBContext database;

    public async Task<int> SaveEmployeeData()
    {
        var managerId = await GetFirstManagerId();
        var emp = new EmployeeMaster
        {
            Email = "viveklpu2011@gmail.com",
            IsActive = true,
            MobileNumber = "7839659584",
            Name = "Vivek",
            ReportingManagerID = managerId
        };
        return await database.SaveEmployeeMasterAsync(emp);
    }

    public async Task<int> SaveReportingManagerData()
    {
        var manager = new ReportingManagerMaster
        {
            Designamtion = "Manager",
            ManagerName = "Subhash",
            Mobile = "9328766630"
        };
        return await database.SaveReportingManagerMasterAsync(manager);
    }

    public async Task<int> SaveLeaveMaster(LeaveMaster leaveMaster)
    {
        return await database.SaveLeaveMasterAsync(leaveMaster);
    }

    public async Task<int> GetFirstManagerId()
    {
        var managerlist =  await database.GetReportingManagerListAsync();
        if (managerlist is not null)
        {
            return managerlist.FirstOrDefault().ManagerId;
        }
        return 0;
    }

    public async Task<List<ReportingManagerMaster>> GetReportingManagerList()
    {
        return await database.GetReportingManagerListAsync();
    }

    public async Task<List<EmployeeMaster>> GetEmployeeList()
    {
        return await database.GetEmployeeListAsync();
    }

    public async Task<EmployeeMaster> GetSavedEmployee(string phone, string name)
    {
        return await database.GetEmployeeAsync(phone,name);
    }

    public async Task<ReportingManagerMaster> GetSavedManager(string phone, string name)
    {
        return await database.GetManagerAsync(phone, name);
    }

    public async Task<List<LeaveTypeMaster>> SaveGetLeaveTypes()
    {
        var leaves = new List<LeaveTypeMaster>
        {
          new LeaveTypeMaster
          {
              IsActive = true,
              Limit = 20,
              Type = "Casual"
          },
          new LeaveTypeMaster
          {
              IsActive = true,
              Limit = 10,
              Type = "Paternity"
          },
          new LeaveTypeMaster
          {
              IsActive = true,
              Limit = 8,
              Type = "Maternity"
          },
        };

        var checkExistingLeaveType = await database.GetLeaveTypeListAsync();
        if (checkExistingLeaveType is not null && checkExistingLeaveType.Count >= 3)
        {
            return checkExistingLeaveType;
        }

        foreach (var item in leaves)
        {
            await database.SaveLeaveTypeMasterAsync(item);
        }
        return await database.GetLeaveTypeListAsync();
    }

    public async Task<List<AppStatusTypeMaster>> SaveGetAppStatus()
    {
        var appstatus = new List<AppStatusTypeMaster>
        {
          new AppStatusTypeMaster
          {
             StatusName = "Applied",
             StatusType = "X"
          },
          new AppStatusTypeMaster
          {
              StatusName = "Approved",
              StatusType = "A"
          },
          new AppStatusTypeMaster
          {
              StatusName = "Rejected",
              StatusType = "R"
          },
        };

        var checkExistingApplicationStatus = await database.GetApplicationStatusListAsync();
        if (checkExistingApplicationStatus is not null && checkExistingApplicationStatus.Count >= 3)
        {
            return checkExistingApplicationStatus;
        }

        foreach (var item in appstatus)
        {
            await database.SaveApplicationStatusAsync(item);
        }
        return await database.GetApplicationStatusListAsync();
    }

    public async Task<EmployeeLeaveBalanceMaster> UpdateLeaveBalance(int userId,int consumedLeaves)
    {
        var checkConsumedLeaves = await database.GetEmployeeLeaveBalanceListAsync();
        var filteredConsumedLeaves = checkConsumedLeaves?.Where(x => x.EmployeeId == userId).FirstOrDefault();

        var leaveBalance = new EmployeeLeaveBalanceMaster
        {
            AllowedLeave = Convert.ToInt32(filteredConsumedLeaves.AllowedLeave),
            ConsumedLeave = consumedLeaves,
            EmployeeId = userId,
            TypeID = filteredConsumedLeaves.TypeID
        };

        await database.SaveEmployeeLeaveBalanceMasterAsync(leaveBalance);
        var checkLeaveBalance = await database.GetEmployeeLeaveBalanceListAsync();
        return checkLeaveBalance?.Where(x => x.EmployeeId == userId).FirstOrDefault();
    }

    public async Task<EmployeeLeaveBalanceMaster> SaveGetEmployeeLeaveBalance(int userId)
    {
        var checkConsumedLeaves = await database.GetLeaveListAsync();
        var filteredConsumedLeaves = checkConsumedLeaves?.Where(x => x.EmployeeId == userId).Sum(x => x.NumberOfDays);

        var leaveBalance = new EmployeeLeaveBalanceMaster
        {
            AllowedLeave = 20,
            ConsumedLeave = filteredConsumedLeaves,
            EmployeeId = userId
        };

        var checkLeaveBalance = await database.GetEmployeeLeaveBalanceListAsync();
        var filteredLeave = checkLeaveBalance?.Where(x => x.EmployeeId == userId).FirstOrDefault();

        if (filteredLeave is not null)
        {
            return filteredLeave;
        }

        await database.SaveEmployeeLeaveBalanceMasterAsync(leaveBalance);
        var savedLeaveBalance  = await database.GetEmployeeLeaveBalanceListAsync();
        var balance = savedLeaveBalance?.Where(x => x.EmployeeId == userId).FirstOrDefault();
        return balance;
    }

    public async Task<List<LeaveMaster>> GetEmplLeaveList(int userId)
    {
        var leaves =  await database.GetLeaveListAsync();
        return leaves?.Where(x => x.EmployeeId == userId).ToList();
    }

    public async Task<List<LeaveMaster>> GetManagerLeaveList(int userId)
    {
        var leaves = await database.GetLeaveListAsync();
        return leaves?.Where(x => x.ReportingManagerId == userId).ToList();
    }
}

