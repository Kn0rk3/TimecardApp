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
    public class Filter : INotifyPropertyChanged, INotifyPropertyChanging
    {

        private string filterID;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string FilterID
        {
            get
            {
                return filterID;
            }

            set
            {
                if (filterID != value)
                {
                    NotifyPropertyChanging("FilterID");
                    filterID = value;
                    NotifyPropertyChanged("FilterID");
                }
            }
        }

        private string filterObject;
        [Column(CanBeNull = true)]
        public string FilterObject
        {
            get
            {
                return filterObject;
            }

            set
            {
                if (filterObject != value)
                {
                    NotifyPropertyChanging("FilterObject");
                    filterObject = value;
                    NotifyPropertyChanged("FilterObject");
                }
            }
        }

        //both, string, both
        private string filterMode;
        [Column(CanBeNull = true)]
        public string FilterMode
        {
            get
            {
                return filterMode;
            }

            set
            {
                if (filterMode != value)
                {
                    NotifyPropertyChanging("FilterMode");
                    filterMode = value;
                    NotifyPropertyChanged("FilterMode");
                }
            }
        }

        private string filterSearchString;
        [Column(CanBeNull = true)]
        public string FilterSearchString
        {
            get
            {
                return filterSearchString;
            }

            set
            {
                if (filterSearchString != value)
                {
                    NotifyPropertyChanging("FilterSearchString");
                    filterSearchString = value;
                    NotifyPropertyChanged("FilterSearchString");
                }
            }
        }

        private string filterName;
        [Column(CanBeNull = true)]
        public string FilterName
        {
            get
            {
                return filterName;
            }

            set
            {
                if (filterName != value)
                {
                    NotifyPropertyChanging("FilterName");
                    filterName = value;
                    NotifyPropertyChanged("FilterName");
                }
            }
        }

        private bool inActive;
        [Column(CanBeNull = true)]
        public bool InActive
        {
            get
            {
                return inActive;
            }

            set
            {
                if (inActive != value)
                {
                    NotifyPropertyChanging("InActive");
                    inActive = value;
                    NotifyPropertyChanged("InActive");
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
                    NotifyPropertyChanging("StartDate");
                    startDate = value;
                    NotifyPropertyChanged("StartDate");
                }
            }
        }

        private DateTime endDate;
        [Column]
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }

            set
            {
                if (endDate != value)
                {
                    NotifyPropertyChanging("EndDate");
                    endDate = value;
                    NotifyPropertyChanged("EndDate");
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
