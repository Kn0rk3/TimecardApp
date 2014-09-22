using TimecardApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace TimecardApp.ViewModel
{
    public class ProjectViewModel : INotifyPropertyChanged
    {
        private Project thisProject;
        public Project ThisProject
        {
            get { return thisProject; }
            set { thisProject = value; }
        }
        
        private string projectID;
        public string ProjectID
        {
            get { return projectID; }
            set 
            { 
                projectID = value;
                NotifyPropertyChanged("ProjectID");
            }
        }

        private bool isInactive;
        public bool IsInactive
        {
            get { return isInactive; }
            set
            {
                isInactive = value;
                NotifyPropertyChanged("IsInactive");
            }
        }

        private string projectShort;
        public string ProjectShort
        {
            get { return projectShort; }
            set
            {
                projectShort = value;
                buildProjectIdent();
                NotifyPropertyChanged("ProjectShort");
            }
        }

        private string projectCode;
        public string ProjectCode
        {
            get { return projectCode; }
            set
            {
                projectCode = value;
                NotifyPropertyChanged("ProjectCode");
            }
        }

        private string ident_Project;
        public string Ident_Project
        {
            get { return ident_Project; }
            set
            {
                ident_Project = value;
                NotifyPropertyChanged("Ident_Project");
            }
        }

        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                projectName = value;
                NotifyPropertyChanged("ProjectName");
            }
        }

        private string projectDescription;
        public string ProjectDescription
        {
            get { return projectDescription; }
            set
            {
                projectDescription = value;
                NotifyPropertyChanged("ProjectDescription");
            }
        }

        private string projectTotalTime;
        public string ProjectTotalTime
        {
            get { return projectTotalTime; }
            set
            {
                projectTotalTime = value;
                NotifyPropertyChanged("ProjectTotalTime");
            }
        }

        private string projectTotalTasks;
        public string ProjectTotalTasks
        {
            get { return projectTotalTasks; }
            set
            {
                projectTotalTasks = value;
                NotifyPropertyChanged("ProjectTotalTasks");
            }
        }

        private string projectQuote;
        public string ProjectQuote
        {
            get { return projectQuote; }
            set
            {
                projectQuote = value;
                NotifyPropertyChanged("ProjectQuote");
            }
        }

        private Customer projectCustomer;
        public Customer ProjectCustomer
        {
            get { return projectCustomer; }
            set
            {
                projectCustomer = value;
                buildProjectIdent();
                NotifyPropertyChanged("ProjectCustomer");
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

        private ObservableCollection<WorkTask> projectWorktaskCollection;
        public ObservableCollection<WorkTask> ProjectWorktaskCollection
        {
            get
            {
                return projectWorktaskCollection;
            }
        }

        public ProjectViewModel(Project project)
        {
            thisProject = project;

            projectID = project.ProjectID;
            projectCode = project.ProjectCode;
            projectShort = project.ProjectShort;
            projectCustomer = project.Customer;
            projectDescription = project.ProjectDescription;
            projectName = project.ProjectName;
            isInactive = project.IsInactive;
            ident_Project = project.Ident_Project;

            buildProjectIdent();
        }

        public void LoadAndCalsInitial()
        {
            //load reference to collection of AppViewModel
            customerCollection = App.AppViewModel.CustomerCollection;
            projectWorktaskCollection = App.AppViewModel.GetWorktasksForProject(projectID);
            projectTotalTime = App.AppViewModel.GetProjectTotalWorktimeString(projectID);
            projectTotalTasks = projectWorktaskCollection.Count.ToString();
            projectQuote = App.AppViewModel.GetProjectQuoteWorktimeString(projectID);
        }

        private void buildProjectIdent()
        {
            if (projectCustomer != null)
            {
                Ident_Project = projectCustomer.CustomerShort + "-" + projectShort;
            }
            else
                Ident_Project = "-" + projectShort;
        }

        public void saveThisProject()
        {
            if (ProjectCustomer != null)
            {
                thisProject.Customer = projectCustomer;
            }

            thisProject.Ident_Project = ident_Project;
            thisProject.ProjectName = projectName;
            thisProject.ProjectCode = projectCode;
            thisProject.ProjectDescription = projectDescription;
            thisProject.ProjectShort = projectShort;
            thisProject.IsInactive = isInactive;

            App.AppViewModel.SaveProjectToDB(thisProject);
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
