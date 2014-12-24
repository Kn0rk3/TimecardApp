using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using TimecardApp.Model;
using TimecardApp.Model.NonPersistent;
using TimecardApp.Model.Timelog;
using TimecardApp.View;

namespace TimecardApp.ViewModel
{
    public class TimelogViewModel : INotifyPropertyChanged, ITimelogViewModel
    {
        private TimelogSetting timelogSetting;
        private ITimelogUsingView timelogUsingView;
        public ITimelogUsingView TimelogUsingView
        {
            set
            {
                timelogUsingView = value;
            }
        }

        private string url;
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
                NotifyPropertyChanged("Url");
            }
        }

        private string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                NotifyPropertyChanged("Username");
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                NotifyPropertyChanged("Password");
            }
        }

        private bool isSaveCredentials;
        public bool IsSaveCredentials
        {
            get
            {
                return isSaveCredentials;
            }
            set
            {
                isSaveCredentials = value;
                NotifyPropertyChanged("IsSaveCredentials");
            }
        }

        private bool isLoginBackground;
        public bool IsLoginBackground
        {
            get
            {
                return isLoginBackground;
            }
            set
            {
                isLoginBackground = value;
                NotifyPropertyChanged("IsLoginBackground");
            }
        }

        private TimelogState currentState;
        public TimelogState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                currentState = value;
                NotifyPropertyChanged("CurrentState");
            }
        }

        private TimelogOperation currentOperation;
        public TimelogOperation CurrentOperation
        {
            get
            {
                return currentOperation;
            }
            set
            {
                currentOperation = value;
                NotifyPropertyChanged("CurrentOperation");
            }
        }

        private ObservableCollection<TimelogTask> tlTaskCollection;
        public ObservableCollection<TimelogTask> TlTaskCollection
        {
            get
            {
                return tlTaskCollection;
            }
            set
            {
                if (tlTaskCollection != value)
                {
                    tlTaskCollection = value;
                    NotifyPropertyChanged("TlTaskCollection");
                }
            }
            
        }

        public TimelogViewModel(TimelogSetting tlSetting, ITimelogUsingView view)
        {
            timelogSetting = tlSetting;
            timelogUsingView = view;

            Username = tlSetting.Username;
            isLoginBackground = tlSetting.LoginBackground;
            if (!String.IsNullOrEmpty(username))
                IsSaveCredentials = true;
            else
                IsSaveCredentials = false;

            url = tlSetting.TimelogUrl;
#if DEBUG
            url = "https://tl.timelog.com/leuschel";
#endif
            if (!String.IsNullOrEmpty(tlSetting.Password))
            {
                password = HelperClass.GetDecryptedPWString(tlSetting.Password);
            }

            TlTaskCollection = App.AppViewModel.GetTimelogTasks();
        }

        public void SaveThisTlSetting()
        {
            timelogSetting.TimelogUrl = Url;
            timelogSetting.LoginBackground = IsLoginBackground;
            if (isSaveCredentials)
            {
                timelogSetting.Username = Username;

                if (!String.IsNullOrEmpty(password))
                {
                    timelogSetting.Password = HelperClass.GetEncryptedPWString(password);
                }
            }
            else
            {
                timelogSetting.Username = String.Empty;
                timelogSetting.Password = String.Empty;
            }
            
            //TODO
            //Save TimelogTasks into DB (implement function in AppViewModel)
            App.AppViewModel.SaveNewTimelogTasks(tlTaskCollection);
            App.AppViewModel.SaveChangesToDB();
        }

        public void ChangeState(TimelogState newState, TimelogOperation operationRunning, string message)
        {
            CurrentState = newState;

            switch (operationRunning)
            {
                case TimelogOperation.GetTasks:
                    {
                        switch (newState)
                        {
                            case TimelogState.UnexpectedError:
                                {
                                    timelogUsingView.ShowErrorMessage(message);
                                    break;
                                }
                        }
                        break;
                    }
                case TimelogOperation.Login:
                    {
                        switch (newState)
                        {
                            case TimelogState.ExectionSuccessfull:
                                {
                                    if (currentOperation != operationRunning)
                                        ExecuteTlOperation(currentOperation);
                                    else
                                    {
                                        //only navigate back in case the login is not automatically
                                        if (!isLoginBackground)
                                            timelogUsingView.NavigateBack();
                                    }
                                    break;
                                }
                            case TimelogState.UnexpectedError:
                                {
                                    timelogUsingView.ShowErrorMessage(message);
                                    timelogUsingView.NavigateLogin();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        public void ExecuteTlOperation(TimelogOperation operation)
        {
            ITimelogWrapper wrapper = new TimelogWrapper(this);
            CurrentOperation = operation;
            switch (operation)
            {
                case TimelogOperation.GetTasks:
                    {
                        if (wrapper.IsValidSecurityToken())
                            wrapper.GetTimelogTasks();

                        else
                        {
                            //login has to be performed before
                            if (isLoginBackground && !String.IsNullOrEmpty(url) && !String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                            {
                                wrapper.LoginTimelog(url, username, password);
                            }
                            else
                            {
                                timelogUsingView.ShowErrorMessage("You need to login to timelog first!");
                                timelogUsingView.NavigateLogin();
                            }
                        }
                        break;
                    }

                case TimelogOperation.Login:
                    {
                        if (!String.IsNullOrEmpty(url) && !String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                        {
                            wrapper.LoginTimelog(url, username, password);
                        }
                        else
                            timelogUsingView.ShowErrorMessage("Make sure you typed in username, password and the timelogsite!");
                        break;
                    }
            }
        }

        public void SetTimelogTasks(ObservableCollection<TimelogProjectManagementService.Task> tasks)
        {
            if (tasks != null)
            {
                ObservableCollection<TimelogTask> tlTasks = new ObservableCollection<TimelogTask>();
                foreach (TimelogProjectManagementService.Task task in tasks)
                {
                    TimelogTask newTask = new TimelogTask()
                    {
                        TimelogTaskUID = task.ID.ToString(),
                        TimelogProjectID = task.Details.ProjectHeader.ID,
                        TimelogProjectName = task.Details.ProjectHeader.Name,
                        TimelogTaskName = task.Name,
                        EndDate = task.EndDate,
                        StartDate = task.StartDate,
                        TimelogTaskID = task.TaskID
                    };
                    tlTasks.Add(newTask);
                }
                TlTaskCollection = tlTasks;
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
