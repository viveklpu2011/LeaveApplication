using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using SimpleLoginUI.DummyData;
using SimpleLoginUI.Models;

namespace SimpleLoginUI.ViewModels.Dashboard;

public class TeacherDashboardPageViewModel : BaseViewModel
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

    private EmployeeLeaveBalanceMaster _leaveBalance;
    public EmployeeLeaveBalanceMaster LeaveBalance
    {
        get { return _leaveBalance; }
        set
        {
            _leaveBalance = value;
            OnPropertyChanged(nameof(LeaveBalance));
        }
    }

    public IAsyncRelayCommand<LeaveMasterClass> ItemApproveTappedCommand { get; set; }
    public IAsyncRelayCommand<LeaveMasterClass> ItemRejectTappedCommand { get; set; }
    private ObservableCollection<AppStatusTypeMaster> AppStatus;

    public TeacherDashboardPageViewModel()
	{
        ItemApproveTappedCommand = new AsyncRelayCommand<LeaveMasterClass>(ItemApproveTappedCommandExecute);
        ItemRejectTappedCommand = new AsyncRelayCommand<LeaveMasterClass>(ItemRejectTappedCommandExecute);
    }

    private async Task ItemRejectTappedCommandExecute(LeaveMasterClass leaveMasterClass)
    {
        if(leaveMasterClass !=null)
        {
            if(string.IsNullOrEmpty(leaveMasterClass.Reason))
            {
                await App.Current.MainPage.DisplayAlert("Error","Please enter reason to reject","OK");
                return;
            }
            var leavedata = new LeaveMaster
            {
                AppStatus = AppStatus.Where(x => x.StatusType == "R").Select(x => x.StatusType).FirstOrDefault(),
                VerifiedBy = UserData.UserId,
                VerifiedOn = DateTime.Now.Date,
                ApplicationID = leaveMasterClass.ApplicationID,
                AppliedBy = leaveMasterClass.AppliedBy,
                AppliedOn = leaveMasterClass.AppliedOn,
                EmployeeId = leaveMasterClass.EmployeeId,
                EndDate = leaveMasterClass.EndDate,
                LeaveTypeId = leaveMasterClass.LeaveTypeId,
                NumberOfDays = leaveMasterClass.NumberOfDays,
                Purpose = leaveMasterClass.Purpose,
                Reason = leaveMasterClass.Reason,
                ReportingManagerId = leaveMasterClass.ReportingManagerId,
                StartDate = leaveMasterClass.StartDate
            };

            var result = await ManageLocalData.Instance.SaveLeaveMaster(leavedata);
            if(result == 1)
            {
                await App.Current.MainPage.DisplayAlert("Rejected", "Leave Rejected", "OK");
                await LoadLeaveTypesAndConsumedLeaves(leaveMasterClass);
                await LoadLeaves();
            }
        }
    }

    private async Task ItemApproveTappedCommandExecute(LeaveMasterClass leaveMasterClass)
    {
        var leavedata = new LeaveMaster
        {
            AppStatus = AppStatus.Where(x => x.StatusType == "A").Select(x => x.StatusType).FirstOrDefault(),
            VerifiedBy = UserData.UserId,
            VerifiedOn = DateTime.Now.Date,
            ApplicationID = leaveMasterClass.ApplicationID,
            AppliedBy = leaveMasterClass.AppliedBy,
            AppliedOn = leaveMasterClass.AppliedOn,
            EmployeeId = leaveMasterClass.EmployeeId,
            EndDate = leaveMasterClass.EndDate,
            LeaveTypeId = leaveMasterClass.LeaveTypeId,
            NumberOfDays = leaveMasterClass.NumberOfDays,
            Purpose = leaveMasterClass.Purpose,
            Reason = leaveMasterClass.Reason,
            ReportingManagerId = leaveMasterClass.ReportingManagerId,
            StartDate = leaveMasterClass.StartDate
        };

        var result = await ManageLocalData.Instance.SaveLeaveMaster(leavedata);
        if (result == 1)
        {
            await App.Current.MainPage.DisplayAlert("Approved", "Leave Approved", "OK");
            await LoadLeaves();
        }
    }

    public async Task LoadLeaves()
    {
        AppStatus = new ObservableCollection<AppStatusTypeMaster>(await ManageLocalData.Instance.SaveGetAppStatus());

        var leaves = await ManageLocalData.Instance.GetManagerLeaveList(UserData.UserId);
        Leaves = new ObservableCollection<LeaveMasterClass>();
        if (leaves != null && leaves.Count > 0)
        {
            foreach (var item in leaves.Where(x=>x.AppStatus == "X"))
            {
                var data = new LeaveMasterClass
                {
                    ReportingManagerId = item.ReportingManagerId,
                    EmployeeName = await EmployeeName(item.EmployeeId),
                    AppliedOn = item.AppliedOn,
                    NumberOfDays = item.NumberOfDays,
                    Reason = item.Reason,
                    Purpose = item.Purpose,
                    AppStatus = item.AppStatus,
                    LeaveType = await GetLeaveTypeName(item.LeaveTypeId),
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    ApplicationID = item.ApplicationID,
                    EmployeeId = item.EmployeeId,
                    AppliedBy = item.AppliedBy,
                    LeaveTypeId = item.LeaveTypeId,
                };
                Leaves.Add(data);
            }
        }
    }

    private async Task<string> EmployeeName(int empid)
    {
        var emplist = await ManageLocalData.Instance.GetEmployeeList();
        if (emplist != null && emplist.Count > 0)
        {
            return emplist.Where(x => x.EmpCode == empid).Select(x => x.Name).FirstOrDefault();
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

    private async Task LoadLeaveTypesAndConsumedLeaves(LeaveMasterClass leaveMasterClass)
    {
        LeaveBalance = new EmployeeLeaveBalanceMaster();
        var empleravesbalance = await ManageLocalData.Instance.SaveGetEmployeeLeaveBalance(leaveMasterClass.EmployeeId);
        if (empleravesbalance != null)
        {
            var consumespendingleaves = empleravesbalance.ConsumedLeave + leaveMasterClass.NumberOfDays;
            var data = await ManageLocalData.Instance.UpdateLeaveBalance(leaveMasterClass.EmployeeId, Convert.ToInt32(consumespendingleaves));
        }
    }
}

