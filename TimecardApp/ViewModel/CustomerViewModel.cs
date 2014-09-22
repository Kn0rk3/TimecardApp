using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.Model;
using System.Collections.ObjectModel;

namespace TimecardApp.ViewModel
{
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private Customer thisCustomer;
        public Customer ThisCustomer
        {
            get { return thisCustomer; }
            set { thisCustomer = value; }
        }

        private string customerID;
        public string CustomerID
        {
            get { return customerID; }
            set 
            { 
                customerID = value;
                NotifyPropertyChanged("CustomerID");
            }
        }

        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set 
            { 
                customerName = value;
                NotifyPropertyChanged("CustomerName");
            }
        }

        private string customerShort;
        public string CustomerShort
        {
            get { return customerShort; }
            set 
            { 
                customerShort = value;
                NotifyPropertyChanged("CustomerShort");
            }
        }

        private string customerContactPerson;
        public string CustomerContactPerson
        {
            get { return customerContactPerson; }
            set 
            { 
                customerContactPerson = value;
                NotifyPropertyChanged("CustomerContactPerson");
            }
        }

        private string customerContactMail;
        public string CustomerContactMail
        {
            get { return customerContactMail; }
            set 
            { 
                customerContactMail = value;
                NotifyPropertyChanged("CustomerContactMail");
            }
        }

        private ObservableCollection<Project> customerProjectCollection;
        public ObservableCollection<Project> CustomerProjectCollection
        {
            get
            {
                return customerProjectCollection;
            }
        }

        public CustomerViewModel(Customer newCustomer)
        {
            thisCustomer = newCustomer;

            customerID = newCustomer.CustomerID;
            customerName = newCustomer.CustomerName;
            customerShort = newCustomer.CustomerShort;
            customerContactMail = newCustomer.CustomerContactMail;
            customerContactPerson = newCustomer.CustomerContactPerson;

            customerProjectCollection = App.AppViewModel.GetProjectsForCustomer(customerID);

            calculateTimeForProjects();
        }

        private void calculateTimeForProjects()
        {
            foreach (Project p in customerProjectCollection)
            {
                p.TotalWorkTimeString = App.AppViewModel.GetProjectTotalWorktimeString(p.ProjectID);
            }
        }

        public void saveThisCustomer()
        {
            thisCustomer.CustomerContactMail = CustomerContactMail;
            thisCustomer.CustomerContactPerson = customerContactPerson;
            thisCustomer.CustomerName = customerName;
            thisCustomer.CustomerShort = customerShort;

            App.AppViewModel.SaveCustomerToDB(thisCustomer);
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

        internal void deleteProject(Project project)
        {
            customerProjectCollection.Remove(project);
            App.AppViewModel.deleteProject(project);
        }
    }
}
