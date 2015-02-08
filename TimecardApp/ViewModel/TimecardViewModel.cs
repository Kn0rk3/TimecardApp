using TimecardApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.ViewModel
{
    public class TimecardViewModel : INotifyPropertyChanged 
    {
        private string timecardID;
        public string TimecardID
        {
            get { return timecardID; }
            set { timecardID = value; }
        }

        private string timecardName;
        public string TimecardName
        {
            get { return timecardName; }
            set 
            { 
                timecardName = value;
                NotifyPropertyChanged("TimecardName");
            }
        }

        private bool timecardEnabled;
        public bool TimecardEnabled
        {
            get { return timecardEnabled; }
            set
            {
                timecardEnabled = value;
                NotifyPropertyChanged("TimecardEnabled");
            }
        }

        private bool isClosed;
        public bool IsClosed
        {
            get { return isClosed; }
            set
            {
                isClosed = value;
                TimecardEnabled = !value;
                NotifyPropertyChanged("IsClosed");
            }
        }

        private DateTime timecardStartDate;
        public DateTime TimecardStartDate
        {
            get { return timecardStartDate; }
        }

        private string timecardPageWorkTime = "";
        public string TimecardPageWorkTime
        {
            get
            {
                return timecardPageWorkTime;
            }
            set
            {
                if (timecardPageWorkTime != value)
                {
                    timecardPageWorkTime = value;
                    NotifyPropertyChanged("TimecardPageWorkTime");
                }
            }
        }

        private ObservableCollection<WorkTask> worktaskTimecardCollection;
        public ObservableCollection<WorkTask> WorktaskTimecardCollection
        {
            get
            {
                return worktaskTimecardCollection;
            }
            set
            {
                if (worktaskTimecardCollection != value)
                {
                    worktaskTimecardCollection = value;
                    NotifyPropertyChanged("WorktaskTimecardCollection");
                }
            }
        }

        private ObservableCollection<Project> projectCollection;
        public ObservableCollection<Project> ProjectCollection
        {
            get
            {
                return projectCollection;
            }
            set
            {
                if (projectCollection != value)
                {
                    projectCollection = value;
                    NotifyPropertyChanged("ProjectCollection");
                }
            }
        }

        private Timecard thisTimecard;
        
        public TimecardViewModel(Timecard newTimecard)
        {
            thisTimecard = newTimecard;

            timecardID = newTimecard.TimecardID;
            IsClosed = newTimecard.IsComplete;
            //TimecardEnabled = !newTimecard.IsComplete;
            timecardName = newTimecard.TimecardName;
            timecardStartDate = newTimecard.StartDate;
        }

        public void CalculateTimeForProjects()
        {
            projectCollection = App.AppViewModel.GetProjectsStatsForTimecard(timecardID);
        }

        public void ReloadWorktasks()
        {
            App.AppViewModel.LoadWorktasksForTimecard(timecardID);
            worktaskTimecardCollection = App.AppViewModel.WorktaskCollection;
            long tempTicks = 0;

            //Calculation of week worktime
            foreach (WorkTask worktask in WorktaskTimecardCollection)
            {
                tempTicks = tempTicks + worktask.WorkTimeTicks;
            }
            //36000000000
            long minutesTicks = tempTicks % 36000000000;
            long minutes = minutesTicks / 600000000;
            long hours = (tempTicks - minutesTicks) / 36000000000;

            TimecardPageWorkTime = hours.ToString("00") + ":" + minutes.ToString("00");
        }

        public void saveThisTimecard()
        {
            thisTimecard.TimecardName = timecardName;
            thisTimecard.IsComplete = isClosed;
            
            App.AppViewModel.SaveTimecardToDB(thisTimecard);
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
