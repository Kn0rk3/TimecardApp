using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.Model;

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


        private ObservableCollection<TimelogTask> tlTaskCollection;
        public ObservableCollection<TimelogTask> TlTaskCollection
        {
            get
            {
                return tlTaskCollection;
            }
        }

        private ObservableCollection<TimelogProject > tlProjectCollection;
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

            byte[] passwordByte = ProtectedData.Unprotect(Encoding.UTF8.GetBytes(tlSetting.Password), null);
            password = Encoding.UTF8.GetString(passwordByte, 0, passwordByte.Length);
        }

        public void saveThisSetting()
        {

            timelogSetting.Username = Username;
            timelogSetting.TimelogUrl = Url;   

            byte[] passwordInByte = Encoding.UTF8.GetBytes(password);
            byte[] protectedPasswordByte = ProtectedData.Protect(passwordInByte, null);
            timelogSetting.Password = Encoding.UTF8.GetString(protectedPasswordByte, 0, protectedPasswordByte.Length);
                     
            App.AppViewModel.SaveChangesToDB();
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
