using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using SimpleLoginUI.DummyData;
using SimpleLoginUI.Models;

namespace SimpleLoginUI.ViewModels.Dashboard;

public class AdminDashboardPageviewModel : BaseViewModel
{

    public UserBasicInfo UserData => JsonConvert.DeserializeObject<UserBasicInfo>(Preferences.Get(nameof(App.UserDetails), null));

    private ObservableCollection<LeaveMasterClass> _leaves;
    public ObservableCollection<LeaveMasterClass> Leaves
    {
        get { return _leaves; }
        set
        {
            _leaves = value;
            OnPropertyChanged(nameof(Leaves));
        }
    }

    public AdminDashboardPageviewModel()
	{
       
    }

    public async Task LoadLeaves()
    {
        var leaves = await ManageLocalData.Instance.GetEmplLeaveList(UserData.UserId);
        Leaves = new ObservableCollection<LeaveMasterClass>();
        if (leaves != null && leaves.Count > 0)
        {
            foreach (var item in leaves)
            {
                var data = new LeaveMasterClass
                {
                    ReportingManagerId = item.ReportingManagerId,
                    ReportingManagerName = await ManagerName(item.ReportingManagerId),
                    AppliedOn = item.AppliedOn,
                    NumberOfDays = item.NumberOfDays,
                    Reason = item.Reason,
                    Purpose = item.Purpose,
                    AppStatus = item.AppStatus,
                    LeaveType = await GetLeaveTypeName(item.LeaveTypeId),
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    ApplicationID = item.ApplicationID
                };
                Leaves.Add(data);
            }
        }
    }

    private async Task<string> ManagerName(int managerId)
    {
        var managerlist = await ManageLocalData.Instance.GetReportingManagerList();
        if (managerlist != null && managerlist.Count > 0)
        {
            return managerlist.Where(x => x.ManagerId == managerId).Select(x => x.ManagerName).FirstOrDefault();
        }
        return string.Empty;
    }

    private async Task<string> GetLeaveTypeName(int typeId)
    {
        var leaveTypeList = await ManageLocalData.Instance.SaveGetLeaveTypes();
        if (leaveTypeList != null && leaveTypeList.Count > 0)
        {
            return leaveTypeList.Where(x => x.TypeID == typeId).Select(x => x.Type).FirstOrDefault();
        }
        return string.Empty;
    }
}

