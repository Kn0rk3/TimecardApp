using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.Model
{
    [Table]
    public class Timecard : INotifyPropertyChanged, INotifyPropertyChanging, IDBObjects 
    {
        private string timecardID; 
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string TimecardID
        {
            get
            {
                return timecardID;
            }

            set
            {
                if (timecardID != value)
                {
                    NotifyPropertyChanging("TimecardID");
                    timecardID = value;
                    NotifyPropertyChanged("TimecardID");
                }
            }
        }

        private DateTime startDate;
        [Column]
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }

            set
            {
                if (startDate != value)
                {
                    if (value.DayOfWeek != DayOfWeek.Monday)
                    {
                        // hier darf immer nur ein Montag gespeichert werden
                        DayOfWeek weekDay = value.DayOfWeek;
                        TimeSpan span;
                        switch (weekDay)
                        {
                            case DayOfWeek.Tuesday:
                                span = new TimeSpan(1, 0, 0, 0);
                                value = value.Subtract(span);
                                break;
                            case DayOfWeek.Wednesday:
                                span = new TimeSpan(2, 0, 0, 0);
                                value = value.Subtract(span);
                                break;
                            case DayOfWeek.Thursday:
                                span = new TimeSpan(3, 0, 0, 0);
                                value = value.Subtract(span);
                                break;
                            case DayOfWeek.Friday:
                                span = new TimeSpan(4, 0, 0, 0);
                                value = value.Subtract(span);
                                break;
                            case DayOfWeek.Saturday:
                                span = new TimeSpan(5, 0, 0, 0);
                                value = value.Subtract(span);
                                break;
                            case DayOfWeek.Sunday:
                                span = new TimeSpan(6, 0, 0, 0);
                                value = value.Subtract(span);
                                break;
                        }
                    }
                    NotifyPropertyChanging("StartDate");
                    startDate = value;
                    NotifyPropertyChanged("StartDate");
                }
            }
        }

        private string timecardName;
        [Column]
        public string TimecardName
        {
            get
            {
                return timecardName;
            }

            set
            {
                if (timecardName != value)
                {
                    NotifyPropertyChanging("TimecardName");
                    timecardName = value;
                    NotifyPropertyChanged("TimecardName");
                }
            }
        }

        private Boolean isComplete;
        [Column]
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

        private String totalWorkTimeString;
        public String TotalWorkTimeString
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
