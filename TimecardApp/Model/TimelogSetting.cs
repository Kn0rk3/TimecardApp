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
    public class TimelogSetting : INotifyPropertyChanged, INotifyPropertyChanging, IDBObjects
    {
        private string timelogSettingID;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string TimelogSettingID
        {
            get
            {
                return timelogSettingID;
            }

            set
            {
                if (timelogSettingID != value)
                {
                    NotifyPropertyChanging("TimelogSettingID");
                    timelogSettingID = value;
                    NotifyPropertyChanged("TimelogSettingID");
                }
            }
        }

        private string username;
        [Column]
        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                if (username != value)
                {
                    NotifyPropertyChanging("Username");
                    username = value;
                    NotifyPropertyChanged("Username");
                }
            }
        }

        private string password;
        [Column(CanBeNull = true)]
        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                if (password != value)
                {
                    NotifyPropertyChanging("Password");
                    password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }

        private string timelogUrl;
        [Column(CanBeNull = true)]
        public string TimelogUrl
        {
            get
            {
                return timelogUrl;
            }

            set
            {
                if (timelogUrl != value)
                {
                    NotifyPropertyChanging("TimelogUrl");
                    timelogUrl = value;
                    NotifyPropertyChanged("TimelogUrl");
                }
            }
        }


        private string lastSynchronisationTimestamp;
        [Column(CanBeNull = true)]
        public string LastSynchronisationTimestamp
        {
            get
            {
                return lastSynchronisationTimestamp;
            }

            set
            {
                if (lastSynchronisationTimestamp != value)
                {
                    NotifyPropertyChanging("LastSynchronisationTimestamp");
                    lastSynchronisationTimestamp = value;
                    NotifyPropertyChanged("LastSynchronisationTimestamp");
                }
            }
        }

        private bool loginBackground;
        [Column(CanBeNull = true)]
        public bool LoginBackground
        {
            get
            {
                return loginBackground;
            }

            set
            {
                if (loginBackground != value)
                {
                    NotifyPropertyChanging("LoginBackground");
                    loginBackground = value;
                    NotifyPropertyChanged("LoginBackground");
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
