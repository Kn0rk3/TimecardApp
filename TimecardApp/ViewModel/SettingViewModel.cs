using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Live;
using System.Windows;

namespace TimecardApp.ViewModel
{
    public class SettingViewModel : INotifyPropertyChanged

    {
        private Setting thisSetting;

        private Project defaultProject;
        public Project DefaultProject
        {
            get
            {
                return defaultProject;
            }
            set
            {
                defaultProject = value;
                NotifyPropertyChanged("DefaultProject");
            }
        }

        private string numberTimecards;
        public string NumberTimecards
        {
            get
            {
                return numberTimecards;
            }
            set
            {
                numberTimecards = value;
                NotifyPropertyChanged("NumberTimecards");
            }
        }

        
       
        //private String mailAddress;
        //public String MailAddress
        //{
        //    get
        //    {
        //        return mailAddress;
        //    }
        //    set
        //    {
        //        if (mailAddress != value)
        //        {
        //            mailAddress = value;
        //            NotifyPropertyChanged("MailAddress");
        //        }
        //    }
        //}

        private ObservableCollection<Project> projectCollection;
        public ObservableCollection<Project> ProjectCollection
        {
            get
            {
                return projectCollection;
            }
        }

        private ObservableCollection<Customer> customerCollection;
        public ObservableCollection<Customer> CustomerCollection
        {
            get
            {
                return customerCollection;
            }
        }


        public SettingViewModel(Setting setting)
        {
            thisSetting = setting;

            //if (!String.IsNullOrEmpty(setting.MailAddress))
            //    MailAddress = setting.MailAddress;

            if (setting.Project != null)
                defaultProject = setting.Project;

            numberTimecards = setting.NumberTimecards.ToString();

            projectCollection = App.AppViewModel.ProjectCollection;
            customerCollection = App.AppViewModel.CustomerCollection;
        }

        public void saveThisSetting()
        {
            if (defaultProject != null)
            {
                thisSetting.Project = DefaultProject;
            }

            thisSetting.NumberTimecards = Int16.Parse(numberTimecards);

            //if (!String.IsNullOrEmpty(mailAddress))
            //    thisSetting.MailAddress = mailAddress; ;
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
