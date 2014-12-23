using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.Model
{
    [Table]
    public class TimelogTask : INotifyPropertyChanged, INotifyPropertyChanging, IDBObjects 
    {
        private string timelogTaskUID;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string TimelogTaskUID
        {
            get
            {
                return timelogTaskUID;
            }

            set
            {
                if (timelogTaskUID != value)
                {
                    NotifyPropertyChanging("TimelogTaskUID");
                    timelogTaskUID = value;
                    NotifyPropertyChanged("TimelogTaskUID");
                }
            }
        }

        private string timelogTaskName;
        [Column]
        public string TimelogTaskName
        {
            get
            {
                return timelogTaskName;
            }

            set
            {
                if (timelogTaskName != value)
                {
                    NotifyPropertyChanging("TimelogTaskName");
                    timelogTaskName = value;
                    NotifyPropertyChanged("TimelogTaskName");
                }
            }
        }

        private int timelogProjectID;
        [Column]
        public int TimelogProjectID
        {
            get
            {
                return timelogProjectID;
            }

            set
            {
                if (timelogProjectID != value)
                {
                    NotifyPropertyChanging("TimelogProjectID");
                    timelogProjectID = value;
                    NotifyPropertyChanged("TimelogProjectID");
                }
            }
        }

        private string timelogProjectName;
        [Column]
        public string TimelogProjectName
        {
            get
            {
                return timelogProjectName;
            }

            set
            {
                if (timelogProjectName != value)
                {
                    NotifyPropertyChanging("TimelogProjectName");
                    timelogProjectName = value;
                    NotifyPropertyChanged("TimelogProjectName");
                }
            }
        }

        private string timelogTaskIdent;
        public string TimelogTaskIdent
        {
            get
            {
                return HelperClass.GetIdentForTimelogTask(timelogTaskName, timelogProjectName, timelogProjectID);
            }
        }

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
