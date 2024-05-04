using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using SimpleLoginUI.Controls;
using SimpleLoginUI.DummyData;
using SimpleLoginUI.Models;
using SimpleLoginUI.Views.Dashboard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLoginUI.ViewModels.Startup
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<LoginTypeModels> loginType;

        private LoginTypeModels selectedLoginType;
        public LoginTypeModels SelectedLoginType
        {
            get { return selectedLoginType; }
            set
            {
                selectedLoginType = value;
                OnPropertyChanged(nameof(SelectedLoginType));
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        private string mobile;
        public string Mobile
        {
            get { return mobile; }
            set
            {
                mobile = value;
                OnPropertyChanged(nameof(Mobile));
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        public IAsyncRelayCommand LoginCommand { get; private set; }

        public LoginPageViewModel()
        {
            LoginCommand = new AsyncRelayCommand(LoginCommandExecute, CheckLoginIsValid);

            LoginType = new ObservableCollection<LoginTypeModels>
            {
                new LoginTypeModels
                {
                    Id = 1,
                    LoginType = "Employee"
                },
                new LoginTypeModels
                {
                    Id = 2,
                    LoginType = "Manager"
                }
            };

            Task.Run(async() =>
            {
                var checkExistingDetails = await CheckExistingLogin();
                if (!checkExistingDetails)
                {
                    await ManageLocalData.Instance.SaveReportingManagerData();
                    await ManageLocalData.Instance.SaveEmployeeData();
                }
            });
        }

        private async Task<bool> CheckExistingLogin()
        {
            var managerlist = await ManageLocalData.Instance.GetReportingManagerList();
            var employeeList = await ManageLocalData.Instance.GetEmployeeList();

            if (managerlist is not null &&
                managerlist.Count > 0 &&
                employeeList is not null &&
                employeeList.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool CheckLoginIsValid() => !string.IsNullOrEmpty(Mobile) && !string.IsNullOrEmpty(Name) && Mobile.Length >= 10 && SelectedLoginType != null;

        #region Commands
        
        async Task LoginCommandExecute()
        {
            try
            {
                var userDetails = new UserBasicInfo();
                if (SelectedLoginType?.LoginType == "Employee")
                {
                    var empResponse = await ManageLocalData.Instance.GetSavedEmployee(Mobile, Name);
                    if (empResponse != null)
                    {
                        userDetails.FullName = Name;
                        userDetails.RoleID = 1;
                        userDetails.RoleText = SelectedLoginType?.LoginType;
                        userDetails.UserId = empResponse.EmpCode;
                        userDetails.Email = empResponse.Email;
                        userDetails.ReportingManagerId = empResponse.ReportingManagerID;

                        if (Preferences.ContainsKey(nameof(App.UserDetails)))
                        {
                            Preferences.Remove(nameof(App.UserDetails));
                        }

                        string userDetailStr = JsonConvert.SerializeObject(userDetails);
                        Preferences.Set(nameof(App.UserDetails), userDetailStr);
                        App.UserDetails = userDetails;
                        await AppConstant.AddFlyoutMenusDetails();
                    }
                    else
                        await App.Current.MainPage.DisplayAlert("Invalid", "Invalid Employee Credentials", "Ok");
                }
                else
                {
                    var managerResponse = await ManageLocalData.Instance.GetSavedManager(Mobile, Name);
                    if (managerResponse != null)
                    {
                        userDetails.FullName = Name;
                        userDetails.RoleID = 2;
                        userDetails.RoleText = SelectedLoginType?.LoginType;
                        userDetails.UserId = managerResponse.ManagerId;
                        if (Preferences.ContainsKey(nameof(App.UserDetails)))
                        {
                            Preferences.Remove(nameof(App.UserDetails));
                        }

                        string userDetailStr = JsonConvert.SerializeObject(userDetails);
                        Preferences.Set(nameof(App.UserDetails), userDetailStr);
                        App.UserDetails = userDetails;
                        await AppConstant.AddFlyoutMenusDetails();
                    }
                    else
                        await App.Current.MainPage.DisplayAlert("Invalid", "Invalid Manager Credentials", "Ok");
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
