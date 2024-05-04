using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using SimpleLoginUI.DummyData;
using SimpleLoginUI.Models;
using static Java.Util.Jar.Attributes;

namespace SimpleLoginUI.ViewModels.Dashboard;

public class StudentDashboardPageViewModel : BaseViewModel
{
    private string _email;
    public string Email
    {
        get { return _email; }
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    private ObservableCollection<LeaveTypeMaster> _leaveTypes;
    public ObservableCollection<LeaveTypeMaster> LeaveTypes
    {
        get { return _leaveTypes; }
        set
        {
            _leaveTypes = value;
            OnPropertyChanged(nameof(LeaveTypes));
        }
    }

    private LeaveTypeMaster _selectedLeaveType;
    public LeaveTypeMaster SelectedLeaveType
    {
        get { return _selectedLeaveType; }
        set
        {
            _selectedLeaveType = value;
            OnPropertyChanged(nameof(SelectedLeaveType));
            SubmitCommand.NotifyCanExecuteChanged();
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

    private string _purpose;
    public string Purpose
    {
        get { return _purpose; }
        set
        {
            _purpose = value;
            OnPropertyChanged(nameof(Purpose));
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    private DateTime _fromDate = DateTime.Now.Date;
    public DateTime FromDate
    {
        get { return _fromDate; }
        set
        {
            _fromDate = value;
            OnPropertyChanged(nameof(FromDate));
            CalculateDays();
        }
    }

    private DateTime _toDate = DateTime.Now.Date;
    public DateTime ToDate
    {
        get { return _toDate; }
        set
        {
            _toDate = value;
            OnPropertyChanged(nameof(ToDate));
            CalculateDays();
        }
    }

    private int _numberOfDays;
    public int NumberOfDays
    {
        get { return _numberOfDays; }
        set
        {
            _numberOfDays = value;
            OnPropertyChanged(nameof(NumberOfDays));
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    private int _consumedLeaves;
    public int ConsumedLeaves
    {
        get { return _consumedLeaves; }
        set
        {
            _consumedLeaves = value;
            OnPropertyChanged(nameof(ConsumedLeaves));
        }
    }

    private int _balanceLeaves;
    public int BalanceLeaves
    {
        get { return _balanceLeaves; }
        set
        {
            _balanceLeaves = value;
            OnPropertyChanged(nameof(BalanceLeaves));
        }
    }

    private DateTime _minimumDate = DateTime.Now.Date;
    public DateTime MinimumDate
    {
        get { return _minimumDate; }
        set
        {
            _minimumDate = value;
            OnPropertyChanged(nameof(MinimumDate));
        }
    }

    private ObservableCollection<AppStatusTypeMaster> AppStatus;

    public IAsyncRelayCommand SubmitCommand { get; private set; }

    public UserBasicInfo UserData => JsonConvert.DeserializeObject<UserBasicInfo>(Preferences.Get(nameof(App.UserDetails), null));

    private int consumedDBLeaves = 0;

    private int totalconsumedleaves = 0;

    public StudentDashboardPageViewModel()
	{
        SubmitCommand = new AsyncRelayCommand(SubmitCommandExecute, CanSubmitCommandExecute);
    }

    private bool CanSubmitCommandExecute()
    {
        var result = SelectedLeaveType != null && !string.IsNullOrEmpty(Purpose) && NumberOfDays > 0 && ConsumedLeaves < LeaveBalance.AllowedLeave;
        return result;
    }

    private async Task SubmitCommandExecute()
    {
        var leaveData = new LeaveMaster
        {
            AppliedBy = Convert.ToInt32(UserData?.UserId),
            AppliedOn = DateTime.Now.Date,
            AppStatus = AppStatus.Where(x=>x.StatusType =="X").Select(x=>x.StatusType).FirstOrDefault(),
            EmployeeId = Convert.ToInt32(UserData?.UserId),
            EndDate = ToDate.Date,
            LeaveTypeId = Convert.ToInt32(SelectedLeaveType?.TypeID),
            NumberOfDays = NumberOfDays,
            Purpose = Purpose,
            ReportingManagerId = Convert.ToInt32(UserData?.ReportingManagerId),
            StartDate = FromDate.Date,
        };

        var result = await ManageLocalData.Instance.SaveLeaveMaster(leaveData);
        if (result == 1)
        {
            await App.Current.MainPage.DisplayAlert("Success", "Leave Applied Successfully", "OK");
            totalconsumedleaves += NumberOfDays;
            var leavebalance = await ManageLocalData.Instance.UpdateLeaveBalance(Convert.ToInt32(UserData?.UserId), totalconsumedleaves);
            Purpose = string.Empty;
            FromDate = DateTime.Now.Date;
            ToDate = DateTime.Now.Date;
            ConsumedLeaves = Convert.ToInt32(leavebalance.ConsumedLeave);
            BalanceLeaves = leavebalance.AllowedLeave - Convert.ToInt32(leavebalance.ConsumedLeave);
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("error", "error on leave application", "OK");
        }
    }

    private async Task CalculateDays()
    {
        if(FromDate.Date > ToDate.Date)
        {
            await App.Current.MainPage.DisplayAlert("error", "From date must not greater than to date", "OK");
            FromDate = DateTime.Now.Date;
            return;
        }
        int datediff = (ToDate.Date - FromDate.Date).Days;
        NumberOfDays = datediff + 1;
        int consumedLeaves = consumedDBLeaves + NumberOfDays;
        if (consumedLeaves < LeaveBalance.AllowedLeave)
        {
            BalanceLeaves = LeaveBalance.AllowedLeave - ConsumedLeaves  - NumberOfDays;
            //ConsumedLeaves = consumedLeaves;
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("error", "Consumed leaved must not be greater than allowed leaves", "OK");
            ToDate = DateTime.Now.Date;
        }
    }

    private async Task LoadLeaveTypesAndConsumedLeaves()
    {
        var leaveTypes = await ManageLocalData.Instance.SaveGetLeaveTypes();
        LeaveTypes = new ObservableCollection<LeaveTypeMaster>(leaveTypes);

        var savedData = Preferences.Get(nameof(App.UserDetails), null);
        var userInfo = JsonConvert.DeserializeObject<UserBasicInfo>(savedData);

        if (userInfo!= null)
        {
            LeaveBalance = new EmployeeLeaveBalanceMaster();
            LeaveBalance = await ManageLocalData.Instance.SaveGetEmployeeLeaveBalance(userInfo.UserId);
            consumedDBLeaves = Convert.ToInt32(LeaveBalance.ConsumedLeave);
            ConsumedLeaves = Convert.ToInt32(LeaveBalance.ConsumedLeave);
            AppStatus = new ObservableCollection<AppStatusTypeMaster>(await ManageLocalData.Instance.SaveGetAppStatus());
        }
    }

    public async void RunTasksSynchronously()
    {
        try
        {
            await LoadLeaveTypesAndConsumedLeaves();
            await CalculateDays();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

