using TimecardApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TimecardApp.Model.NonPersistent;
using TimecardApp.Resources;

namespace TimecardApp.ViewModel
{
    public class WorktaskViewModel : INotifyPropertyChanged
    {
        #region variables

        private WorkTask thisWorkTask;

        private string worktaskID;
        public string WorktaskID
        {
            get { return worktaskID; }
            set { worktaskID = value; }
        }

        private string worktaskPageTimecardName;
        public string WorktaskPageTimecardName
        {
            get { return worktaskPageTimecardName; }
            set
            {
                worktaskPageTimecardName = value;
                NotifyPropertyChanged("WorktaskPageTimecardName");
            }
        }

        private string worktaskPageName;
        public string WorktaskPageName
        {
            get { return worktaskPageName; }
            set { worktaskPageName = value; }
        }

        private string worktaskPageIdent;
        public string WorktaskPageIdent
        {
            get { return worktaskPageIdent; }
            set
            {
                worktaskPageIdent = value;
                NotifyPropertyChanged("WorktaskPageIdent");
            }
        }

        private bool worktaskPageIsOnsite;
        public bool WorktaskPageIsOnsite
        {
            get { return worktaskPageIsOnsite; }
            set
            {
                worktaskPageIsOnsite = value;
                NotifyPropertyChanged("WorktaskPageIsOnsite");
            }
        }

        private DateTime worktaskPageDayDate;
        public DateTime WorktaskPageDayDate
        {
            get { return worktaskPageDayDate; }
            set
            {
                if (worktaskPageDayDate != value)
                {
                    if (checkTimecardForDate(value))
                    {
                        if (worktaskPageProject != null)
                            WorktaskPageIdent = HelperClass.GetIdentForWorktask(value, worktaskPageProject.Ident_Project);
                        else
                            WorktaskPageIdent = HelperClass.GetIdentForWorktask(value, "");

                        worktaskPageDayDate = value;
                        loadTimelogTasksForDate(value);
                        NotifyPropertyChanged("WorktaskPageDayDate");
                    }
                    else
                    {
                        WorktaskPageDayDate = worktaskPageDayDate;
                        NotifyPropertyChanged("WorktaskPageDayDate");
                    }
                }

            }
        }

        private DateTime worktaskPageStartTime;
        public DateTime WorktaskPageStartTime
        {
            get { return worktaskPageStartTime; }
            set
            {
                worktaskPageStartTime = value;
                CalculateWorkTime();
                NotifyPropertyChanged("WorktaskPageStartTime");
            }
        }

        private DateTime worktaskPageEndTime;
        public DateTime WorktaskPageEndTime
        {
            get { return worktaskPageEndTime; }
            set
            {
                worktaskPageEndTime = value;
                CalculateWorkTime();
                NotifyPropertyChanged("WorktaskPageEndTime");
            }
        }

        private DateTime worktaskPagePauseTime;
        public DateTime WorktaskPagePauseTime
        {
            get { return worktaskPagePauseTime; }
            set
            {
                worktaskPagePauseTime = value;
                CalculateWorkTime();
                NotifyPropertyChanged("WorktaskPagePauseTime");
            }
        }

        private string worktaskPageWorkDescription;
        public string WorktaskPageWorkDescription
        {
            get 
            {            
                    return worktaskPageWorkDescription; 
            }
            set
            {
                worktaskPageWorkDescription = value;
                NotifyPropertyChanged("WorktaskPageWorkDescription");
            }
        }

        private DateTime worktaskPageWorkTime;
        public DateTime WorktaskPageWorkTime
        {
            get { return worktaskPageWorkTime; }
            set
            {

                if (worktaskPageWorkTime != value)
                {
                    worktaskPageWorkTime = value;
                    NotifyPropertyChanged("WorktaskPageWorkTime");
                }
            }
        }

        private bool worktaskPageEnabled;
        public bool WorktaskPageEnabled
        {
            get { return worktaskPageEnabled; }
            set
            {

                if (worktaskPageEnabled != value)
                {
                    worktaskPageEnabled = value;
                    NotifyPropertyChanged("WorktaskPageEnabled");
                }
            }
        }

        private Project worktaskPageProject;
        public Project WorktaskPageProject
        {
            get
            {
                return worktaskPageProject;
            }
            set
            {
                if (worktaskPageProject != value)
                {
                    WorktaskPageIdent = HelperClass.GetIdentForWorktask(worktaskPageDayDate, value.Ident_Project);

                    worktaskPageProject = value;
                    NotifyPropertyChanged("WorktaskPageProject");
                }

            }
        }

        private Timecard worktaskPageTimecard;
        public Timecard WorktaskPageTimecard
        {
            get
            {
                return worktaskPageTimecard;
            }
            set
            {
                if (worktaskPageTimecard != value)
                {
                    worktaskPageTimecard = value;
                    WorktaskPageTimecardName = value.TimecardName;
                    NotifyPropertyChanged("WorktaskPageTimecard");
                }

            }
        }

        private ObservableCollection<Project> worktaskPageProjectCollection;
        public ObservableCollection<Project> WorktaskPageProjectCollection
        {
            get
            {
                return worktaskPageProjectCollection;
            }
        }

        private ObservableCollection<TimelogTask> timelogTaskCollection;
        public ObservableCollection<TimelogTask> TimelogTaskCollection
        {
            get
            {
                return timelogTaskCollection;
            }
            set
            {
                timelogTaskCollection = value;
                NotifyPropertyChanged("TimelogTaskCollection");
            }
        }

        private TimelogTask timelogTask;
        public TimelogTask TimelogTask
        {
            get
            {
                return timelogTask;
            }
            set
            {
                if (timelogTask != value)
                {
                    timelogTask = value;
                    TaskInformation = value.ExpandItems;
                    NotifyPropertyChanged("TimelogTask");
                }

            }
        }

        private IList<string> taskInformation;
        public IList<string> TaskInformation
        {
            get
            {
                return taskInformation;
            }
            set
            {
                taskInformation = value;
                NotifyPropertyChanged("TaskInformation");
            }
        }

        private string lastTimelogRegistration;
        public string LastTimelogRegistration
        {
            get
            {
                return lastTimelogRegistration;
            }
            set
            {
                if (lastTimelogRegistration != value)
                {
                    lastTimelogRegistration = value;
                    if (String.IsNullOrEmpty(value))
                        IsResetableForTimelog = false;
                    else
                        IsResetableForTimelog = true;
                    NotifyPropertyChanged("LastTimelogRegistration");
                }
            }
        }

        private bool? forTimelog;
        public bool? ForTimelog
        {
            get
            {
                return forTimelog;
            }
            set
            {
                if (value.HasValue)
                {
                    if (forTimelog != value)
                    {
                        forTimelog = value.Value;
                        if (value.Value)
                            TimelogActive = Visibility.Visible;
                        else
                            TimelogActive = Visibility.Collapsed;
                        loadTimelogTasksForDate(worktaskPageDayDate);
                        NotifyPropertyChanged("ForTimelog");
                    }

                }
                else
                    forTimelog = false;
            }
        }

        private bool isResetableForTimelog;
        public bool IsResetableForTimelog
        {
            get
            {
                return isResetableForTimelog;
            }
            set
            {

                if (isResetableForTimelog != value)
                {
                    isResetableForTimelog = value;
                    NotifyPropertyChanged("IsResetableForTimelog");
                }

            }
        }

        private Visibility timelogActive;
        public Visibility TimelogActive
        {
            get { return timelogActive; }
            set
            {

                if (timelogActive != value)
                {
                    timelogActive = value;
                    NotifyPropertyChanged("TimelogActive");
                }
            }
        }

        #endregion

        // Class constructor, create the data context object.
        public WorktaskViewModel(WorkTask newWorkTask)
        {
            thisWorkTask = newWorkTask;

            if (App.AppViewModel.UsingTimelogInterface)
                timelogActive = Visibility.Visible;
            else
                timelogActive = Visibility.Collapsed;

            worktaskID = thisWorkTask.WorkTaskID;
            WorktaskPageTimecard = thisWorkTask.Timecard;

            // fill the properties for this datacontext
            if (worktaskPageTimecard.IsComplete)
                worktaskPageEnabled = false;

            else
                worktaskPageEnabled = !thisWorkTask.IsComplete;

            worktaskPageIdent = thisWorkTask.Ident_WorkTask;
            worktaskPageDayDate = thisWorkTask.DayDate;
            worktaskPageIsOnsite = thisWorkTask.IsOnsite;
            worktaskPageEndTime = thisWorkTask.EndTime;
            worktaskPageStartTime = thisWorkTask.StartTime;
            worktaskPageWorkTime = new DateTime(thisWorkTask.WorkTimeTicks);
            worktaskPagePauseTime = new DateTime(thisWorkTask.PauseTimeTicks);
            worktaskPageWorkDescription = thisWorkTask.WorkDescription;

            //load reference to collection of AppViewModel
            worktaskPageProjectCollection = App.AppViewModel.GetActiveProjects();

            if (thisWorkTask.Project != null)
            {
                worktaskPageProject = thisWorkTask.Project;

                // in case the project isn't active anymore but the worktask is assigned to this project, add it to the list
                if (!worktaskPageProjectCollection.Contains(worktaskPageProject))
                {
                    worktaskPageProjectCollection.Add(worktaskPageProject);
                }
            }

            // timelog
            forTimelog = thisWorkTask.IsForTimelogRegistration;
            if (forTimelog == true)
            {
                loadTimelogTasksForDate(thisWorkTask.DayDate);
                lastTimelogRegistration = thisWorkTask.LastTimelogRegistration;
                if (String.IsNullOrEmpty(lastTimelogRegistration))
                    isResetableForTimelog = false;
                else
                    isResetableForTimelog = true;

                if (thisWorkTask.TimelogTask != null)
                {
                    TimelogTask = thisWorkTask.TimelogTask;
                    if (!TimelogTaskCollection.Contains(timelogTask))
                    {
                        TimelogTaskCollection.Add(timelogTask);
                    }
                }
            }
        }

        public void SaveThisWorkTask()
        {
            thisWorkTask.EndTime = worktaskPageEndTime;
            thisWorkTask.StartTime = worktaskPageStartTime;
            thisWorkTask.PauseTimeTicks = worktaskPagePauseTime.TimeOfDay.Ticks;
            thisWorkTask.WorkTimeTicks = worktaskPageWorkTime.TimeOfDay.Ticks;
            thisWorkTask.WorkDescription = worktaskPageWorkDescription;

            if (worktaskPageProject != null)
                thisWorkTask.Project = worktaskPageProject;

            thisWorkTask.IsOnsite = worktaskPageIsOnsite;
            thisWorkTask.Ident_WorkTask = worktaskPageIdent;
            thisWorkTask.DayDate = worktaskPageDayDate;
            thisWorkTask.Timecard = worktaskPageTimecard;
            thisWorkTask.IsForTimelogRegistration = forTimelog;
            thisWorkTask.LastTimelogRegistration = lastTimelogRegistration;

            if (timelogTask != null)
                thisWorkTask.TimelogTask = TimelogTask;

            App.AppViewModel.SaveWorkTaskToDB(thisWorkTask);
        }

        public void CalculateWorkTime()
        {
            long startTimeTicks = worktaskPageStartTime.Ticks;
            long endTimeTicks = worktaskPageEndTime.Ticks;

            long workTime = endTimeTicks - startTimeTicks - worktaskPagePauseTime.Ticks;
            if (workTime >= 0)
                WorktaskPageWorkTime = new DateTime(workTime);
            else
                WorktaskPageWorkTime = new DateTime(0);
        }

        private void loadTimelogTasksForDate(DateTime date)
        {
            TimelogTaskCollection = App.AppViewModel.GetTimelogTasksForDate(date);
        }

        public void DeleteThisWorktask()
        {
            App.AppViewModel.DeleteWorktask(thisWorkTask);
        }

        private bool checkTimecardForDate(DateTime value)
        {
            Timecard tmpTimecard = App.AppViewModel.getTimecardForDate(value);

            if (tmpTimecard != null)
            {
                if (tmpTimecard.TimecardID == worktaskPageTimecard.TimecardID)
                {
                    //Timecards sind gleich
                    return true;
                }
                else
                {
                    // neue Timecard wäre nicht die gleiche
                    MessageBoxButton buttons = MessageBoxButton.OKCancel;
                    MessageBoxResult result = MessageBox.Show("With this new date, the worktask will be moved into the timecard " + tmpTimecard.TimecardName + ". Do you wanna do this?", "", buttons);

                    if (result == MessageBoxResult.OK)
                    {
                        WorktaskPageTimecard = tmpTimecard;
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
            {
                MessageBox.Show("There is no timecard for this timespan. Create one first for this date.");
                return false;
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
