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
    public class Setting : INotifyPropertyChanged, INotifyPropertyChanging, IDBObjects 
    {
        private string settingID;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string SettingID
        {
            get
            {
                return settingID;
            }

            set
            {
                if (settingID != value)
                {
                    NotifyPropertyChanging("SettingID");
                    settingID = value;
                    NotifyPropertyChanged("SettingID");
                }
            }
        }

        private string projectID;
        [Column(CanBeNull = true)]
        public string ProjectID;
        private EntityRef<Project> _Project;
        [Association(Storage = "_Project", ThisKey = "ProjectID")]
        public Project Project
        {
            get { return this._Project.Entity; }
            set 
            {
                NotifyPropertyChanging("ProjectID");
                this._Project.Entity = value;
                if (value != null)
                    projectID = value.ProjectID;
                NotifyPropertyChanged("ProjectID");
            }
        }

        private String mailAddress;
        [Column]
        public String MailAddress
        {
            get
            {
                return mailAddress;
            }

            set
            {
                if (mailAddress != value)
                {
                    NotifyPropertyChanging("MailAddress");
                    mailAddress = value;
                    NotifyPropertyChanged("MailAddress");
                }
            }
        }

        private int numberTimecards;
        [Column]
        public int NumberTimecards
        {
            get
            {
                return numberTimecards;
            }

            set
            {
                if (numberTimecards != value)
                {
                    NotifyPropertyChanging("NumberTimecards");
                    numberTimecards = value;
                    NotifyPropertyChanged("NumberTimecards");
                }
            }
        }

        //db version 2
        private bool isUsingTimelog;
        [Column(CanBeNull = true)]
        public bool? IsUsingTimelog
        {
            get
            {
                return isUsingTimelog;
            }

            set
            {
                if (value.HasValue)
                {
                    if (isUsingTimelog != value)
                    {
                        NotifyPropertyChanging("IsUsingTimelog");
                        isUsingTimelog = value.Value;
                        NotifyPropertyChanged("IsUsingTimelog");
                    }
                }
                else
                    isUsingTimelog = false;
                
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
