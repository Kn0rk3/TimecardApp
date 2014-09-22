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
    public class Project : INotifyPropertyChanged, INotifyPropertyChanging, IDBObjects 
    {
        private string projectID;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string ProjectID
        {
            get
            {
                return projectID;
            }

            set
            {
                if (projectID != value)
                {
                    NotifyPropertyChanging("ProjectID");
                    projectID = value;
                    NotifyPropertyChanged("ProjectID");
                }
            }
        }

        private string customerID;
        [Column(CanBeNull = true)]
        public string CustomerID;

        private EntityRef<Customer> _Customer;
        [Association(Storage = "_Customer", ThisKey = "CustomerID", IsForeignKey = true)]
        public Customer Customer
        {
            get { return this._Customer.Entity; }
            set 
            {
                NotifyPropertyChanging("Customer");
                this._Customer.Entity = value;
                if (value != null)
                    customerID = value.CustomerID;
                NotifyPropertyChanged("Customer");
            }
        }       

        //customerShort + "-" + projectShort
        private string ident_Project;
        [Column]
        public string Ident_Project
        {
            get
            {
                return ident_Project;
            }

            set
            {
                if (ident_Project != value)
                {
                    NotifyPropertyChanging("Ident_Project");
                    ident_Project = value;
                    NotifyPropertyChanged("Ident_Project");
                }
            }
        }

        //max 5 signs
        private string projectShort;
        [Column]
        public string ProjectShort
        {
            get
            {
                return projectShort;
            }

            set
            {
                if (projectShort != value)
                {
                    NotifyPropertyChanging("ProjectShort");
                    projectShort = value;
                    NotifyPropertyChanged("ProjectShort");
                }
            }
        } 


        private string projectName;
        [Column]
        public string ProjectName
        {
            get
            {
                return projectName;
            }

            set
            {
                if (projectName != value)
                {
                    NotifyPropertyChanging("ProjectName");
                    projectName = value;
                    NotifyPropertyChanged("ProjectName");
                }
            }
        }

        private string projectCode;
        [Column(CanBeNull = true)]
        public string ProjectCode
        {
            get
            {
                return projectCode;
            }

            set
            {
                if (projectCode != value)
                {
                    NotifyPropertyChanging("ProjectCode");
                    projectCode = value;
                    NotifyPropertyChanged("ProjectCode");
                }
            }
        }

        private string projectDescription;
        [Column(CanBeNull = true)]
        public string ProjectDescription
        {
            get
            {
                return projectDescription;
            }

            set
            {
                if (projectDescription != value)
                {
                    NotifyPropertyChanging("ProjectDescription");
                    projectDescription = value;
                    NotifyPropertyChanged("ProjectDescription");
                }
            }
        }


        private bool isInactive;
        [Column(CanBeNull = true)]
        public bool IsInactive
        {
            get
            {
                return isInactive;
            }

            set
            {
                if (isInactive != value)
                {
                    NotifyPropertyChanging("IsInactive");
                    isInactive = value;
                    NotifyPropertyChanged("IsInactive");
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
