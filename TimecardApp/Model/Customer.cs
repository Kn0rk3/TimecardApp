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
    public class Customer : INotifyPropertyChanged, INotifyPropertyChanging, IDBObjects 
    {
        private string customerID;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string CustomerID
        {
            get
            {
                return customerID;
            }

            set
            {
                if (customerID != value)
                {
                    NotifyPropertyChanging("CustomerID");
                    customerID = value;
                    NotifyPropertyChanged("CustomerID");
                }
            }
        }

        private string customerName;
        [Column(CanBeNull = false)]
        public string CustomerName
        {
            get
            {
                return customerName;
            }

            set
            {
                if (customerName != value)
                {
                    NotifyPropertyChanging("CustomerName");
                    customerName = value;
                    NotifyPropertyChanged("CustomerName");
                }
            }
        }



        private string customerShort;
        [Column]
        public string CustomerShort
        {
            get
            {
                return customerShort;
            }

            set
            {
                if (customerShort != value)
                {
                    NotifyPropertyChanging("CustomerShort");
                    customerShort = value;
                    NotifyPropertyChanged("CustomerShort");
                }
            }
        }

        private string customerContactPerson;
        [Column(CanBeNull = true)]
        public string CustomerContactPerson
        {
            get
            {
                return customerContactPerson;
            }

            set
            {
                if (customerContactPerson != value)
                {
                    NotifyPropertyChanging("CustomerContactPerson");
                    customerContactPerson = value;
                    NotifyPropertyChanged("CustomerContactPerson");
                }
            }
        }

        private string customerContactMail;
        [Column(CanBeNull = true)]
        public string CustomerContactMail
        {
            get
            {
                return customerContactMail;
            }

            set
            {
                if (customerContactMail != value)
                {
                    NotifyPropertyChanging("CustomerContactMail");
                    customerContactMail = value;
                    NotifyPropertyChanged("CustomerContactMail");
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
