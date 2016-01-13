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
using TimecardApp.Resources;
using TimecardApp.TimelogProjectManagementService;
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
                url = HelperClass.ReturnManipulatedURLForTimelog(value);
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

        private string lastSynchronisationTimestamp;
        public string LastSynchronisationTimestamp
        {
            get
            {
                return lastSynchronisationTimestamp;
            }
            set
            {
                lastSynchronisationTimestamp = value;
                NotifyPropertyChanged("LastSynchronisationTimestamp");
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

        private ETimelogState currentState;
        public ETimelogState CurrentState
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

        private ETimelogOperation currentOperation;
        public ETimelogOperation CurrentOperation
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

        private ObservableCollection<WorkTask> tlWorktaskCollection;
        public ObservableCollection<WorkTask> TlWorktaskCollection
        {
            get
            {
                return tlWorktaskCollection;
            }
            set
            {
                if (tlWorktaskCollection != value)
                {
                    tlWorktaskCollection = value;
                    NotifyPropertyChanged("TlWorktaskCollection");
                }
            }
        }

        public TimelogViewModel(TimelogSetting tlSetting, ITimelogUsingView view)
        {
            timelogSetting = tlSetting;
            timelogUsingView = view;

            username = tlSetting.Username;
            isLoginBackground = tlSetting.LoginBackground;
            if (!String.IsNullOrEmpty(username))
                isSaveCredentials = true;
            else
                isSaveCredentials = false;

            url = tlSetting.TimelogUrl;
#if DEBUG
            url = "";
#endif
            if (!String.IsNullOrEmpty(tlSetting.Password))
            {
                try
                {
                    password = HelperClass.GetDecryptedPWString(tlSetting.Password);
                }
                catch
                {
                    password = "";
                    timelogUsingView.ShowErrorMessage("Please retype your login credentials. Password couldn't be restored because of different device.");
                }
            }
            lastSynchronisationTimestamp = tlSetting.LastSynchronisationTimestamp;
            TlTaskCollection = App.AppViewModel.GetTimelogTasks();
            TlWorktaskCollection = App.AppViewModel.GetAllWorktasksForTimelog();
        }

        public void SaveThisTlSetting()
        {
            timelogSetting.TimelogUrl = Url;
            timelogSetting.LoginBackground = IsLoginBackground;
            if (isSaveCredentials)
            {
                timelogSetting.Username = Username;

                if (!String.IsNullOrEmpty(password))
                    timelogSetting.Password = HelperClass.GetEncryptedPWString(password);
                else
                    timelogSetting.Password = String.Empty;

            }
            else
            {
                timelogSetting.Username = String.Empty;
                timelogSetting.Password = String.Empty;
            }
            if (timelogSetting.LastSynchronisationTimestamp != LastSynchronisationTimestamp)
            {
                timelogSetting.LastSynchronisationTimestamp = LastSynchronisationTimestamp;
                App.AppViewModel.SaveNewTimelogTasks(tlTaskCollection);
            }
            App.AppViewModel.SaveChangesToDB();
        }

        public void ChangeState(ETimelogState newState, ETimelogOperation operationRunning, string message)
        {
            CurrentState = newState;

            switch (operationRunning)
            {
                case ETimelogOperation.GetTasks:
                    {
                        switch (newState)
                        {
                            case ETimelogState.Error:
                                {
                                    timelogUsingView.ShowErrorMessage(message);
                                    break;
                                }
                        }
                        break;
                    }

                case ETimelogOperation.UploadWorkunits:
                        switch (newState)
                        {
                            case ETimelogState.Error:
                                {
                                    timelogUsingView.ShowErrorMessage(message);
                                    break;
                                }
                        }
                        break;

                case ETimelogOperation.Login:
                    {
                        switch (newState)
                        {
                            case ETimelogState.Successfull:
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
                            case ETimelogState.Error:
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

        public void ExecuteTlOperation(ETimelogOperation operation)
        {
            ITimelogWrapper wrapper = new TimelogWrapper(this);
            CurrentOperation = operation;
            switch (operation)
            {
                case ETimelogOperation.GetTasks:
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

                case ETimelogOperation.UploadWorkunits:
                    {
                        if (wrapper.IsValidSecurityToken())
                        {
                            ObservableCollection<WorkUnit> insertUnits = new ObservableCollection<WorkUnit>();
                            ObservableCollection<WorkUnit> updateUnits = new ObservableCollection<WorkUnit>();
                           
                            foreach (WorkTask worktask in tlWorktaskCollection)
                            {
                                if (worktask.ToRegister)
                                {
                                    WorkUnit _unit = new WorkUnit();
                                    _unit.TaskID = worktask.TimelogTask.TimelogTaskID;
                                    _unit.StartDateTime = worktask.DayDate;
                                    _unit.EmployeeInitials = username;
                                    if (!String.Equals(worktask.WorkDescription, AppResources.ExampleTaskDescription))
                                        _unit.Description = worktask.WorkDescription;
                                    _unit.EndDateTime = worktask.DayDate.AddTicks(worktask.WorkTimeTicks);
                                    _unit.Duration = TimeSpan.FromTicks(worktask.WorkTimeTicks);
                                    if (String.IsNullOrEmpty(worktask.TimelogWorkunitGUID))
                                    {
                                        _unit.GUID = System.Guid.NewGuid();
                                        insertUnits.Add(_unit);
                                    }
                                    else
                                    {
                                        _unit.GUID = System.Guid.Parse(worktask.TimelogWorkunitGUID);
                                        updateUnits.Add(_unit);
                                    }
                                }
                            }

                            if (insertUnits.Count > 0 || updateUnits.Count >0)
                                wrapper.UploadWorkunits(insertUnits, updateUnits);
                            
                        }
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

                case ETimelogOperation.Login:
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

        public void UpdateWorktasksForReturnedWorkunits(ObservableCollection<BatchContainerOfWorkUnit> returnWorkunits)
        {
            if (returnWorkunits != null)
            {
                foreach (TimelogProjectManagementService.BatchContainerOfWorkUnit  workUnit in returnWorkunits)
                {
                    WorkTask worktask = null;
                    var worktasksForUnit = from WorkTask task in tlWorktaskCollection
                                          where (task.TimelogWorkunitGUID == workUnit.Item.GUID.ToString())
                                          select task;
                    
                    if (worktasksForUnit.Count() == 1)
                        worktask = worktasksForUnit.Single();
                    else
                    {
                        //nach Beschreibung suchen
                        worktasksForUnit = from WorkTask task in tlWorktaskCollection
                                          where (task.DayDate == workUnit.Item.StartDateTime.Date && task.TimelogTask.TimelogTaskID == workUnit.Item.TaskID && task.WorkDescription == workUnit.Item.Description)
                                          select task;
                        if (worktasksForUnit.Count() == 1)
                            worktask = worktasksForUnit.Single();
                        else if (worktasksForUnit.Count() > 1)
                        {
                            foreach (WorkTask tasks in worktasksForUnit)
                            {

                            }
                        }
                        else if (worktasksForUnit.Count() == 0)
                        {


                        }

                        

                    }

                    if (workUnit.Status == TimelogProjectManagementService.ExecutionStatus.Success)
                    {
                        //success --> set timestamp and GUID and remove from collection
                        worktask.TimelogWorkunitGUID = workUnit.Item.GUID.ToString();
                        worktask.LastTimelogRegistration = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "\nTask-ID: " + workUnit.Item.TaskID + "\nTask: " 
                            + worktask.TimelogTask.TimelogTaskName + "\nWorktime: " + worktask.TotalWorkTimeString;
                        App.AppViewModel.SaveChangesToDB();
                    }
                    else if (workUnit.Status == TimelogProjectManagementService.ExecutionStatus.Error)
                    {
                        // when error for update workunit check for error message
                        if (String.IsNullOrEmpty(worktask.TimelogWorkunitGUID))
                        {
                            //error during inserting this workunit
                            
                        }
                        else
                        {
                            //error during updating this workunit
                            if (workUnit.Message == "Object reference not set to an instance of an object.")
                            {
                                //in this particular case the workunit was deleted in timelog but not in this app
                                //Reset worktask (it will keep in the list so no error message 
                                worktask.TimelogWorkunitGUID = String.Empty;
                                App.AppViewModel.SaveChangesToDB();
                            }
                        }
                    }
                }
                TlWorktaskCollection = App.AppViewModel.GetAllWorktasksForTimelog();
            }
        }

        public void SetTimelogTasks(ObservableCollection<TimelogProjectManagementService.Task> tasks)
        {
            LastSynchronisationTimestamp = "Last synchronisation: " +  DateTime.Now.ToString("yy-MM-dd hh:mm:ss");
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
