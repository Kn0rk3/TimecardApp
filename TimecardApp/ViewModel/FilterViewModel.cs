using TimecardApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.ViewModel
{
    public class FilterViewModel : INotifyPropertyChanged
    {
        private Filter thisFilter;
        public Filter ThisFilter
        {
            get { return thisFilter; }
            set { thisFilter = value; }
        }

        private string filterID;
        public string FilterID
        {
            get
            {
                return filterID;
            }
        }

        private String filterObject;
        public String FilterObject
        {
            get
            {
                return filterObject;
            }

            set
            {
                if (filterObject != value)
                {
                    filterObject = value;
                    NotifyPropertyChanged("FilterObject");
                }
            }
        }

        private String filterName;
        public String FilterName
        {
            get
            {
                return filterName;
            }

            set
            {
                if (filterName != value)
                {
                    filterName = value;
                    NotifyPropertyChanged("FilterName");
                }
            }
        }

        private string searchString;
        public string SearchString
        {
            get
            {
                return searchString;
            }

            set
            {
                if (searchString != value)
                {
                    searchString = value;
                    NotifyPropertyChanged("SearchString");
                }
            }
        }

        private string filterMode;
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
                    filterMode = value;
                    NotifyPropertyChanged("FilterMode");
                }
            }
        }

        //private bool inActive;
        //public bool InActive
        //{
        //    get
        //    {
        //        return inActive;
        //    }

        //    set
        //    {
        //        if (inActive != value)
        //        {
        //            inActive = value;
        //            NotifyPropertyChanged("InActive");
        //        }
        //    }
        //}

        private DateTime startDate;
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
                    startDate = value;
                    NotifyPropertyChanged("StartDate");
                }
            }
        }

        private DateTime endDate;
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
                    endDate = value;
                    NotifyPropertyChanged("EndDate");
                }
            }
        }

        //private ObservableCollection<String> filterObjectCollection = new ObservableCollection<String>() {"", "Timecard", "Worktask", "Project", "Customer"};
        private ObservableCollection<String> filterObjectCollection = new ObservableCollection<String>() {"Timecard"};
        public ObservableCollection<String> FilterObjectCollection
        {
            get
            {
                return filterObjectCollection;
            }
        }

        public FilterViewModel(Filter newFilter)
        {
            thisFilter  = newFilter;

            filterID = newFilter.FilterID;
            filterObject = newFilter.FilterObject;
            startDate = newFilter.StartDate;
            endDate = newFilter.EndDate;
            filterMode = newFilter.FilterMode;
            //inActive = newFilter.InActive;
            searchString = newFilter.FilterSearchString;
        }

        public void saveThisFilter()
        {
            thisFilter.EndDate = endDate;
            thisFilter.StartDate = startDate;
            //thisFilter.InActive = inActive;
            thisFilter.FilterObject = filterObject;
            thisFilter.FilterSearchString = searchString;
            thisFilter.FilterMode = filterMode;
            App.AppViewModel.SaveChangesToDB();
            App.AppViewModel.LoadFilterTimecardCollectionFromDatabase(thisFilter);
            //App.AppViewModel.SaveFilterToDB(thisFilter );
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
