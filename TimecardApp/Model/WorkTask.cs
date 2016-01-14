using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using TimecardApp.Resources;

namespace TimecardApp.Model
{
    [Table]
    public class WorkTask : INotifyPropertyChanged, INotifyPropertyChanging, IDBObjects 
    {

        private string workTaskID;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string WorkTaskID
        {
            get
            {
                return workTaskID;
            }

            set
            {
                if (workTaskID != value)
                {
                    NotifyPropertyChanging("WorkTaskID");
                    workTaskID = value;
                    NotifyPropertyChanged("WorkTaskID");
                }
            }
        }

        //date and project
        private String ident_WorkTask;
        [Column]
        public String Ident_WorkTask
        {
            get
            {
                return ident_WorkTask;
            }

            set
            {
                if (ident_WorkTask != value)
                {
                    NotifyPropertyChanging("Ident_WorkTask");
                    ident_WorkTask = value;
                    NotifyPropertyChanged("Ident_WorkTask");
                }
            }
        }

        private string projectID;
        [Column(CanBeNull = true)]
        public string ProjectID;

        private EntityRef<Project> _Project;
        [Association(Storage = "_Project", ThisKey = "ProjectID", IsForeignKey = true )]
        public Project Project
        {
            get { return this._Project.Entity; }
            set 
            {
                NotifyPropertyChanging("ProjectID");
                this._Project.Entity = value;
                if (value != null)
                    projectID  = value.ProjectID;
                NotifyPropertyChanged("ProjectID");
            }
        }

        //db version 2
        private string timelogTaskUID;
        [Column(CanBeNull = true)]
        public string TimelogTaskUID;

        private EntityRef<TimelogTask> _TimelogTask;
        [Association(Storage = "_TimelogTask", ThisKey = "TimelogTaskUID", IsForeignKey = true)]
        public TimelogTask TimelogTask
        {
            get { return this._TimelogTask.Entity; }
            set
            {
                NotifyPropertyChanging("TimelogTaskUID");
                this._TimelogTask.Entity = value;
                if (value != null)
                    timelogTaskUID = value.TimelogTaskUID;
                NotifyPropertyChanged("TimelogTaskUID");
            }
        }

        //db version 2
        private string lastTimelogRegistration;
        [Column(CanBeNull = true)]
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
                    if (!String.IsNullOrEmpty(value))
                        IsComplete = true;
                    NotifyPropertyChanging("LastTimelogRegistration");
                    lastTimelogRegistration = value;
                    NotifyPropertyChanged("LastTimelogRegistration");
                }
            }
        }

        //db version 2
        private string timelogWorkunitGUID;
        [Column(CanBeNull = true)]
        public string TimelogWorkunitGUID
        {
            get
            {
                return timelogWorkunitGUID;
            }

            set
            {
                if (timelogWorkunitGUID != value)
                {
                    NotifyPropertyChanging("TimelogWorkunitGUID");
                    timelogWorkunitGUID = value;
                    NotifyPropertyChanged("TimelogWorkunitGUID");
                }
            }
        }

        //db version 2
        private bool isForTimelogRegistration;
        [Column(CanBeNull = true)]
        public bool? IsForTimelogRegistration
        {
            get
            {
                return isForTimelogRegistration;
            }

            set
            {
                if (value.HasValue)
                {
                    if (isForTimelogRegistration != value)
                    {
                        NotifyPropertyChanging("IsForTimelogRegistration");
                        isForTimelogRegistration = value.Value;
                        NotifyPropertyChanged("IsForTimelogRegistration");
                    }
                }
                else
                    isForTimelogRegistration = false;

            }
        }

        private string timecardID;
        [Column(CanBeNull = true)]
        public string TimecardID;
        private EntityRef<Timecard> _Timecard;
        [Association(Storage = "_Timecard", ThisKey = "TimecardID", IsForeignKey = true)]
        public Timecard Timecard
        {
            get { return this._Timecard.Entity; }
            set 
            {
                NotifyPropertyChanging("TimecardID");
                this._Timecard.Entity = value;
                if (value != null)
                    timecardID  = value.TimecardID;
                NotifyPropertyChanged("TimecardID");
            }
        }

        private Boolean isComplete;
        [Column(CanBeNull = true)]
        public Boolean IsComplete
        {
            get
            {
                return isComplete;
            }

            set
            {
                if (isComplete != value)
                {
                    NotifyPropertyChanging("IsComplete");
                    isComplete = value;
                    NotifyPropertyChanged("IsComplete");
                }
            }
        }

        private Boolean isOnsite;
        [Column(CanBeNull = true)]
        public Boolean IsOnsite
        {
            get
            {
                return isOnsite;
            }

            set
            {
                if (isOnsite != value)
                {
                    NotifyPropertyChanging("IsOnsite");
                    isOnsite = value;
                    NotifyPropertyChanged("IsOnsite");
                }
            }
        }

        private string dayShort;
        public string DayShort
        {
            get
            {
                return dayShort;
            }

            set
            {
                if (dayShort != value)
                {
                    NotifyPropertyChanging("DayShort");
                    dayShort = value;
                    NotifyPropertyChanged("DayShort");
                }
            }
        }

        //for timelog registration
        private bool toRegister;
        public bool ToRegister
        {
            get
            {
                return toRegister;
            }
            set
            {
                if (toRegister != value)
                {
                    NotifyPropertyChanging("ToRegister");
                    toRegister = value;
                    NotifyPropertyChanged("ToRegister");
                }
            }
        }

        private DateTime dayDate;
        [Column]
        public DateTime DayDate
        {
            get
            {
                return dayDate;
            }

            set
            {
                if (dayDate != value)
                {
                    if (dayDate == DateTime.MinValue)
                    {
                        startTime = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                        endTime = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                        pauseTimeTicks = 0;

                        workTimeTicks = 0;    
                    }
                    NotifyPropertyChanging("DayDate");
                    dayDate = value;
                    NotifyPropertyChanged("DayDate");
                }
            }
        }

        private DateTime startTime;
        [Column(CanBeNull = true)]
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                if (startTime != value)
                {
                    NotifyPropertyChanging("StartTime");
                    startTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }
        }

        private DateTime endTime;
        [Column(CanBeNull = true)]
        public DateTime EndTime
        {
            get
            {
                return endTime;
            }

            set
            {
                if (endTime != value)
                {
                    NotifyPropertyChanging("EndTime");
                    endTime = value;
                    NotifyPropertyChanged("EndTime");
                }
            }
        }

        private Int64 pauseTimeTicks;
        [Column(CanBeNull = true)]
        public Int64 PauseTimeTicks
        {
            get
            {
                return pauseTimeTicks;
            }

            set
            {
                if (pauseTimeTicks != value)
                {
                    NotifyPropertyChanging("PauseTimeTicks");
                    pauseTimeTicks = value;
                    NotifyPropertyChanged("PauseTimeTicks");
                }
            }
        }

        private Int64 workTimeTicks;
        [Column(CanBeNull = true)]
        public Int64 WorkTimeTicks
        {
            get
            {
                return workTimeTicks;
            }

            set
            {
                if (workTimeTicks != value)
                {
                    NotifyPropertyChanging("WorkTimeTicks");
                    workTimeTicks = value;
                    DateTime workTimeTmp = new DateTime(workTimeTicks);
                    totalWorkTimeString = workTimeTmp.ToString("HH:mm");
                    NotifyPropertyChanged("WorkTimeTicks");
                }
            }
        }

        private String workDescription;
        [Column(CanBeNull = true)]
        public String WorkDescription
        {
            get
            {
                if (String.IsNullOrEmpty(workDescription))
                    return AppResources.ExampleTaskDescription;
                else    
                    return workDescription;
            }

            set
            {
                if (workDescription != value)
                {
                    NotifyPropertyChanging("WorkDescription");
                    workDescription = value;
                    NotifyPropertyChanged("WorkDescription");
                }
            }
        }

        private string totalWorkTimeString;
        public string TotalWorkTimeString
        {
            get
            {
                return totalWorkTimeString;
            }

            set
            {
                if (totalWorkTimeString != value)
                {
                    NotifyPropertyChanging("TotalWorkTimeString");
                    totalWorkTimeString = value;
                    NotifyPropertyChanged("TotalWorkTimeString");
                }
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                if (isExpanded != value)
                {
                    NotifyPropertyChanging("IsExpanded");
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }


        public string TimelogTaskIdent
        {
            get
            {
                if (TimelogTask != null)
                    return TimelogTask.TimelogTaskName;
                else
                    return "";
            }
        }

        public IList<string> ExpandItems
        {
            get
            {
                IList<string> expandItems = new List<string>();
                expandItems.Add("Des: " + WorkDescription);
                if (TimelogTask != null)
                    expandItems.Add("Tl Task: " + TimelogTask.TimelogTaskIdent);
                return expandItems;
            }
        }

        public IList<string> ExpandItemsTl
        {
            get
            {
                if (TimelogTask != null)
                {
                    IList<string> expandItems = new List<string>();
                    expandItems.Add("Tl Project: " + TimelogTask.TimelogProjectName);
                    expandItems.Add("Worktime: " + TotalWorkTimeString);
                    if (String.IsNullOrEmpty(TimelogWorkunitGUID))
                        expandItems.Add("Action: Insert in Timelog");
                    else
                        expandItems.Add("Action: Update in Timelog");
                    return expandItems;
                }
                else
                    return new List<string>();
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
