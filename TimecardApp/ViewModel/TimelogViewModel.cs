using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.Model;
using TimecardApp.Model.NonPersistent;
using TimecardApp.Model.Timelog;

namespace TimecardApp.ViewModel
{
    public class TimelogViewModel : INotifyPropertyChanged
    {
        private TimelogSetting timelogSetting;

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

        private bool savingCredentials;
        public bool SavingCredentials
        {
            get
            {
                return savingCredentials;
            }
            set
            {
                savingCredentials = value;
                NotifyPropertyChanged("SavingCredentials");
            }
        }

        private bool loggedIn;
        public bool LoggedIn
        {
            get
            {
                return loggedIn;
            }
            set
            {
                loggedIn = value;
                NotLoggedIn = !value;
                NotifyPropertyChanged("LoggedIn");
            }
        }

        private bool notLoggedIn;
        public bool NotLoggedIn
        {
            get
            {
                return notLoggedIn;
            }
            set
            {
                notLoggedIn = value;
                NotifyPropertyChanged("NotLoggedIn");
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


        private ObservableCollection<TimelogTask> tlTaskCollection;
        public ObservableCollection<TimelogTask> TlTaskCollection
        {
            get
            {
                return tlTaskCollection;
            }
        }

        private ObservableCollection<TimelogProject> tlProjectCollection;
        public ObservableCollection<TimelogProject> TlProjectCollection
        {
            get
            {
                return tlProjectCollection;
            }
        }

        public TimelogViewModel(TimelogSetting tlSetting)
        {
            timelogSetting = tlSetting;

            username = tlSetting.Username;
            url = tlSetting.TimelogUrl;
#if DEBUG
            url = "https://tl.timelog.com/leuschel";
#endif
            if (!String.IsNullOrEmpty(tlSetting.Password))
            {
                password = HelperClass.GetDecryptedPWString(tlSetting.Password);
            }
            LoggedIn = false;
        }

        public void SaveThisTlSetting()
        {
            timelogSetting.TimelogUrl = Url;
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

            App.AppViewModel.SaveChangesToDB();
        }

        public void TL_loginToTimelog()
        {
            // der wrapper ist nur temporär, er wird erzeugt und benutzt. Die SessionInstanz im Hintergrund ist die die den SessionStatus aufrecht erhält. Der Wrapper ist nur eine weitere Abstraktionsschicht 
            ITimelogWrapper wrapper = new TimelogWrapper(this);
            wrapper.LoginTimelog(url, username, password);

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
