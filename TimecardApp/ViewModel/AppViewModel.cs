using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TimecardApp.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using TimecardApp.Resources;
using Microsoft.Phone.Data.Linq;
using System.IO.IsolatedStorage;
using TimecardApp.Model.NonPersistent;
using TimecardApp.View;

namespace TimecardApp.ViewModel
{
    public class AppViewModel : INotifyPropertyChanged
    {
        #region Variablen
        // Data context for the local database
        private DBClass dellAppDB;
        private string connectionString;

        private WorktaskViewModel workTaskViewModel;
        private ProjectViewModel projectViewModel;
        private CustomerViewModel customerViewModel;
        private TimecardViewModel timecardViewModel;
        private SettingViewModel settingViewModel;
        private FilterViewModel filterViewModel;
        private TimelogViewModel timelogViewModel;

        private bool usingTimelogInterface;
        public bool UsingTimelogInterface
        {
            get { return usingTimelogInterface; }
            set { usingTimelogInterface = value; }
        }

        // Die ObservableCollection ist im Endeffekt auch nur eine Liste, 
        // jedoch hat sie eine spezielle Eigenschaft, welche sie für unseren Einsatzzweck sehr wertvoll macht: 
        // sie implementiert die Interfaces INotifyCollectionChanged und INotifyPropertyChanged. 
        // Da wir, wie man im obigen C#-Code sehen kann, die ObservableCollection 
        // direkt als ItemsSource des LongListSelectors angeben und diese die oben genannten Interfaces implementiert, 
        // haben wir den Vorteil, dass der LongListSelector von selbst merkt, wenn sich die Liste ändert 
        // (da diese das passende Event aus den Interfaces aufruft) und sich selbst neu zeichnet, 
        // wodurch die neuen Items direkt – ohne Zutun unsererseits – in der UI dargestellt werden.
        private ObservableCollection<Timecard> currentTimecardCollection;
        public ObservableCollection<Timecard> CurrentTimecardCollection
        {
            get
            {
                return currentTimecardCollection;
            }
            set
            {
                if (currentTimecardCollection != value)
                {
                    currentTimecardCollection = value;
                    NotifyPropertyChanged("CurrentTimecardCollection");
                }
            }
        }

        private ObservableCollection<Timecard> filteredTimecardCollection;
        public ObservableCollection<Timecard> FilteredTimecardCollection
        {
            get
            {
                return filteredTimecardCollection;
            }
            set
            {
                if (filteredTimecardCollection != value)
                {
                    filteredTimecardCollection = value;
                    NotifyPropertyChanged("FilteredTimecardCollection");
                }
            }
        }

        private ObservableCollection<Timecard> openTimecardCollection;
        public ObservableCollection<Timecard> OpenTimecardCollection
        {
            get
            {
                return openTimecardCollection;
            }
            set
            {
                if (openTimecardCollection != value)
                {
                    openTimecardCollection = value;
                    NotifyPropertyChanged("OpenTimecardCollection");
                }
            }
        }




        private ObservableCollection<WorkTask> worktaskCollection;
        public ObservableCollection<WorkTask> WorktaskCollection
        {
            get
            {
                return worktaskCollection;
            }
            set
            {
                if (worktaskCollection != value)
                {
                    worktaskCollection = value;
                    NotifyPropertyChanged("WorktaskCollection");
                }
            }
        }

        private ObservableCollection<WorkTask> worktaskCopyCollection;
        public ObservableCollection<WorkTask> WorktaskCopyCollection
        {
            get
            {
                return worktaskCopyCollection;
            }
            set
            {
                if (worktaskCopyCollection != value)
                {
                    worktaskCopyCollection = value;
                    NotifyPropertyChanged("WorktaskCopyCollection");
                }
            }
        }

        private ObservableCollection<Project> projectCollection;
        public ObservableCollection<Project> ProjectCollection
        {
            get
            {
                return projectCollection;
            }
            set
            {
                if (projectCollection != value)
                {
                    projectCollection = value;
                    NotifyPropertyChanged("ProjectCollection");
                }
            }
        }

        private ObservableCollection<Customer> customerCollection;
        public ObservableCollection<Customer> CustomerCollection
        {
            get
            {
                return customerCollection;
            }
            set
            {
                if (customerCollection != value)
                {
                    customerCollection = value;
                    NotifyPropertyChanged("CustomerCollection");
                }
            }
        }

        private string timecardName;
        public string TimecardName
        {
            get { return timecardName; }
            set
            {
                timecardName = value;
                NotifyPropertyChanged("TimecardName");
            }
        }

        private string weekText;
        public string WeekText
        {
            get { return weekText; }
            set
            {
                weekText = value;
                NotifyPropertyChanged("WeekText");
            }
        }

        #endregion

        // Class constructor, create the data context object.
        public AppViewModel(string dbConnectionString)
        {
            connectionString = dbConnectionString;
            ConnectDB();

            if (dellAppDB.Setting.Count() == 0)
            {
                //leer, also neues Setting Objekt
                Setting newSetting = new Setting() { SettingID = System.Guid.NewGuid().ToString(), NumberTimecards = 5 };
                dellAppDB.Setting.InsertOnSubmit(newSetting);
                dellAppDB.SubmitChanges();
            }

            if (dellAppDB.Filter.Count() == 0)
            {
                Filter newFilter = new Filter() { FilterID = System.Guid.NewGuid().ToString(), FilterObject = "Timecard", EndDate = DateTime.Now.Date, StartDate = DateTime.Now.Date };
                dellAppDB.Filter.InsertOnSubmit(newFilter);
                dellAppDB.SubmitChanges();
            }

            var settingObj = from Setting setting in dellAppDB.Setting select setting;
            Setting tmpSetting = settingObj.Single();
            if (tmpSetting.IsUsingTimelog.HasValue)
                UsingTimelogInterface = tmpSetting.IsUsingTimelog.Value;
            else
                UsingTimelogInterface = false;

            LoadCollectionsFromDatabase();
        }

        public void DisposeCurrentDB()
        {
            dellAppDB.Dispose();
        }

        public void ConnectDB()
        {
            dellAppDB = new DBClass(connectionString);
        }

        public void SendExceptionReport(Exception e)
        {
            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            MessageBoxResult result = MessageBox.Show("An error occured. Please allow this app, to send an email to my account with the error report. Will you allow to send the email?", "", buttons);

            if (result == MessageBoxResult.OK)
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask();

                emailComposeTask.Subject = "Error:" + e.Message;
                emailComposeTask.Body = "ErrorMessage:" + Environment.NewLine + e.Message
                    + Environment.NewLine + "StackTrace: " + Environment.NewLine + e.StackTrace
                    + Environment.NewLine + "Source: " + Environment.NewLine + e.Source
                    + Environment.NewLine + "InnerException: " + Environment.NewLine + e.InnerException
                    + Environment.NewLine + "additionalData: " + Environment.NewLine + e.Data;
                emailComposeTask.To = AppResources.MailDev;

                emailComposeTask.Show();
            }
        }

        #region save methods
        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            dellAppDB.SubmitChanges();
        }

        public void SaveNewTimelogTasks(ObservableCollection<TimelogTask> tlTaskCollection)
        {
            // delete the old ones
            var oldTasks = from TimelogTask tasks in dellAppDB.TimelogTasks
                                select tasks;

            ObservableCollection<TimelogTask> oldTaskCollection = new ObservableCollection<TimelogTask>(oldTasks);
            foreach (TimelogTask oldTask in oldTaskCollection)
            {
                // try to match the UID
                if (!tlTaskCollection.Where(u => u.TimelogTaskUID  == oldTask.TimelogTaskUID).Any())
                {
                    //find all worktasks which are using this timelogtask
                    var worktasksUsingOldTask = from WorkTask worktasks in dellAppDB.WorkTasks
                                                where worktasks.TimelogTaskUID == oldTask.TimelogTaskUID
                                                select worktasks;
                    
                    if (worktasksUsingOldTask.Count() > 0)
                    {
                        // set the timelogtask to null
                        ObservableCollection<WorkTask> tlUsingWorktasks = new ObservableCollection<WorkTask>(worktasksUsingOldTask);
                        foreach (WorkTask worktask in tlUsingWorktasks)
                        {
                            worktask.TimelogTask = null;
                        }
                    }
                    dellAppDB.SubmitChanges();
                    dellAppDB.TimelogTasks.DeleteOnSubmit(oldTask);
                }
                   
            }
            dellAppDB.SubmitChanges();

            //insert new ones (or update)
            foreach(TimelogTask newTask in tlTaskCollection )
            {
                if (!oldTaskCollection.Where(u => u.TimelogTaskUID == newTask.TimelogTaskUID).Any())
                    //Insert
                    dellAppDB.TimelogTasks.InsertOnSubmit(newTask);
                else
                {
                    TimelogTask toUpdateTask = oldTaskCollection.Where(u => u.TimelogTaskUID == newTask.TimelogTaskUID).Single();
                    toUpdateTask.TimelogTaskName = newTask.TimelogTaskName;
                    toUpdateTask.TimelogProjectName = newTask.TimelogProjectName;
                    toUpdateTask.TimelogProjectID = newTask.TimelogProjectID;
                    toUpdateTask.StartDate = newTask.StartDate;
                    toUpdateTask.EndDate = newTask.EndDate;
                }
            }
            dellAppDB.SubmitChanges();
        }

        public bool SaveCopiedTimecard(Timecard newTimecard)
        {
            if (!dellAppDB.Timecards.Where(u => u.StartDate.Year == newTimecard.StartDate.Year && u.StartDate.Day == newTimecard.StartDate.Day && u.StartDate.Month == newTimecard.StartDate.Month).Any() || dellAppDB.Timecards.Count() == 0)
            {
                newTimecard.TimecardName = timecardName;
                SaveTimecardToDB(newTimecard);

                foreach (WorkTask singleWorktask in WorktaskCopyCollection)
                {
                    singleWorktask.Timecard = newTimecard;
                    dellAppDB.WorkTasks.InsertOnSubmit(singleWorktask);
                }

                SaveChangesToDB();
                return true;
            }
            else
            {
                MessageBox.Show("There is already an existing timecard for this week.");
                return false;
            }
        }

        public void SetWorktaskPreviewCopyTimecard(DateTime newStartDate)
        {
            // nur wenn diese Timecard noch nicht existiert wird sie angelegt
            if (!dellAppDB.Timecards.Where(u => u.StartDate.Year == newStartDate.Year && u.StartDate.Day == newStartDate.Day && u.StartDate.Month == newStartDate.Month).Any() || dellAppDB.Timecards.Count() == 0)
            {
                WorktaskCopyCollection = new ObservableCollection<WorkTask>();
                Int64 diffTicks = newStartDate.Date.Ticks - timecardViewModel.TimecardStartDate.Date.Ticks;

                foreach (WorkTask singleWorktask in WorktaskCollection)
                {
                    DateTime newDate = singleWorktask.DayDate.Date.AddTicks(diffTicks).Date;
                    WorkTask newWorktask = new WorkTask();
                    newWorktask.WorkTaskID = System.Guid.NewGuid().ToString();
                    newWorktask.DayDate = newDate;
                    newWorktask.DayShort = singleWorktask.DayShort;
                    newWorktask.EndTime = singleWorktask.EndTime;
                    newWorktask.StartTime = singleWorktask.StartTime;
                    newWorktask.WorkDescription = singleWorktask.WorkDescription;
                    newWorktask.Project = singleWorktask.Project;
                    newWorktask.PauseTimeTicks = singleWorktask.PauseTimeTicks;
                    newWorktask.IsOnsite = singleWorktask.IsOnsite;
                    newWorktask.WorkTimeTicks = singleWorktask.WorkTimeTicks;
                    if (singleWorktask.Project != null)
                        newWorktask.Ident_WorkTask = HelperClass.GetIdentForWorktask(newDate, singleWorktask.Project.Ident_Project);
                    else
                        newWorktask.Ident_WorkTask = HelperClass.GetIdentForWorktask(newDate, "");
                    newWorktask.IsForTimelogRegistration = singleWorktask.IsForTimelogRegistration;
                    newWorktask.TimelogTask = singleWorktask.TimelogTask;
                    WorktaskCopyCollection.Add(newWorktask);
                }
            }
            else
            {
                MessageBox.Show("There is already an existing timecard for this week.");
                WorktaskCopyCollection = null;
            }

        }

        public void SaveWorkTaskToDB(WorkTask workTaskToSave)
        {
            int index = -1;
            if (worktaskCollection != null)
                index = worktaskCollection.IndexOf(workTaskToSave);
            else
            {
                worktaskCollection = new ObservableCollection<WorkTask>();
            }

            if (index > -1)
                worktaskCollection[index] = workTaskToSave;

            else
            {
                // wenn es diesen Worktask noch nicht geben sollte, dann kann er hiermit hinzugefügt werden
                dellAppDB.WorkTasks.InsertOnSubmit(workTaskToSave);
                worktaskCollection.Add(workTaskToSave);
            }

            dellAppDB.SubmitChanges();

            workTaskViewModel = null;
        }

        public void SaveProjectToDB(Project projectToSave)
        {
            int index = -1;
            index = projectCollection.IndexOf(projectToSave);

            if (index > -1)
                projectCollection[index] = projectToSave;

            else
            {
                dellAppDB.Projects.InsertOnSubmit(projectToSave);
                projectCollection.Add(projectToSave);
            }

            dellAppDB.SubmitChanges();

            projectViewModel = null;
        }

        public void SaveCustomerToDB(Customer customerToSave)
        {
            int index = -1;
            index = customerCollection.IndexOf(customerToSave);

            if (index > -1)
                customerCollection[index] = customerToSave;

            else
            {
                dellAppDB.Customer.InsertOnSubmit(customerToSave);
                customerCollection.Add(customerToSave);
            }

            dellAppDB.SubmitChanges();

            customerViewModel = null;
        }

        public void SaveTimecardToDB(Timecard thisTimecard)
        {
            if (!dellAppDB.Timecards.Contains(thisTimecard))
                dellAppDB.Timecards.InsertOnSubmit(thisTimecard);

            dellAppDB.SubmitChanges();
        }
        #endregion

        #region getter and load collection methods
        public FilterViewModel GetFilterViewModel(string filterIDParameter)
        {
            if (filterViewModel != null)
                return filterViewModel;

            var filterObj = from Filter filter in dellAppDB.Filter
                            select filter;

            Filter tmpFilter = filterObj.Single();

            filterViewModel = new FilterViewModel(tmpFilter);

            return filterViewModel;
        }

        public CustomerViewModel GetCustomerViewModel(String customerID)
        {
            Customer newCustomer = (Customer)getDBObjectForID(DBObjects.Customer, customerID);

            if (newCustomer != null)
            {
                customerViewModel = new CustomerViewModel(newCustomer);
            }
            else
            {
                newCustomer = new Customer() { CustomerID = customerID };
                customerViewModel = new CustomerViewModel(newCustomer);
            }

            return customerViewModel;
        }

        public WorktaskViewModel GetWorkTaskViewModel(String worktaskID, String projectID, String timecardID)
        {
            if (!String.IsNullOrEmpty(worktaskID))
            {
                //wenn es bereits ein WorkTaskViewMOdel gibt und dies auch noch dem entspricht, was gefordert ist, dann wird auch nur dies zurückgeliefert (Werte sind noch die alten)
                if (workTaskViewModel != null)
                    if (workTaskViewModel.WorktaskID == worktaskID)
                        return workTaskViewModel;

                WorkTask newWorkTask = (WorkTask)getDBObjectForID(DBObjects.Worktask, worktaskID);

                if (newWorkTask == null)
                {
                    if (timecardViewModel != null)
                        newWorkTask = new WorkTask() { WorkTaskID = worktaskID, DayDate = timecardViewModel.TimecardStartDate, Ident_WorkTask = HelperClass.GetIdentForWorktask(timecardViewModel.TimecardStartDate, ""), TimecardID = timecardViewModel.TimecardID, Timecard = (Timecard)getDBObjectForID(DBObjects.Timecard, timecardViewModel.TimecardID) };
                    else
                    {
                        Timecard tmpTimecard = getTimecardForDate(DateTime.Now);
                        if (tmpTimecard != null)
                        {
                            timecardViewModel = new TimecardViewModel(tmpTimecard);
                        }
                        else
                        {
                            return null;
                        }
                        newWorkTask = new WorkTask() { WorkTaskID = worktaskID, DayDate = DateTime.Now, Ident_WorkTask = HelperClass.GetIdentForWorktask(DateTime.Now, ""), Timecard = tmpTimecard, TimecardID = timecardViewModel.TimecardID };
                    }

                    if (!String.IsNullOrEmpty(projectID))
                    {
                        var projectFromColl = from Project project in ProjectCollection
                                              where project.ProjectID == projectID
                                              select project;

                        if (projectFromColl.Count() == 1)
                        {
                            newWorkTask.Project = projectFromColl.Single();
                        }
                    }
                }

                workTaskViewModel = new WorktaskViewModel(newWorkTask);
            }
            return workTaskViewModel;
        }

        public TimecardViewModel GetTimecardViewModel(String timecardID)
        {
            if (timecardViewModel != null)
                if (timecardViewModel.TimecardID == timecardID)
                    return timecardViewModel;

            Timecard newTimecard = (Timecard)getDBObjectForID(DBObjects.Timecard, timecardID);

            timecardViewModel = new TimecardViewModel(newTimecard);

            return timecardViewModel;
        }

        public ProjectViewModel GetProjectViewModel(String projectID, String customerID)
        {
            if (projectViewModel != null)
                if (projectViewModel.ProjectID == projectID)
                    return projectViewModel;

            Project newProject = (Project)getDBObjectForID(DBObjects.Project, projectID);

            //prüfen ob es bereits dieses Projekt gibt oder ob er neu erstellt werden soll
            if (newProject == null)
            {
                if (!String.IsNullOrEmpty(customerID))
                {
                    var customerFromColl = from Customer customer in CustomerCollection
                                           where customer.CustomerID == customerID
                                           select customer;
                    if (customerFromColl.Count() > 0)
                        newProject = new Project() { ProjectID = projectID, Customer = customerFromColl.Single() };
                    else
                        newProject = new Project() { ProjectID = projectID };
                }
                else
                    newProject = new Project() { ProjectID = projectID };
            }

            projectViewModel = new ProjectViewModel(newProject);
            return projectViewModel;
        }

        public SettingViewModel GetSettingViewModel()
        {
            if (settingViewModel != null)
                return settingViewModel;

            var settingObj = from Setting setting in dellAppDB.Setting
                             select setting;

            settingViewModel = new SettingViewModel(settingObj.Single());

            return settingViewModel;
        }

        public TimelogViewModel GetTimelogViewModel(ITimelogUsingView view)
        {
            if (timelogViewModel != null)
            {
                timelogViewModel.TimelogUsingView = view;
                return timelogViewModel;
            }
              
            var timelogSettingObj = from TimelogSetting tlSetting in dellAppDB.TimelogSetting
                                    select tlSetting;

            if (timelogSettingObj.Count() > 0)
                timelogViewModel = new TimelogViewModel(timelogSettingObj.Single(),  view);
            else
            {
                TimelogSetting newTlSetting = new TimelogSetting() { TimelogSettingID = System.Guid.NewGuid().ToString() };
                dellAppDB.TimelogSetting.InsertOnSubmit(newTlSetting);
                dellAppDB.SubmitChanges();
                timelogViewModel = new TimelogViewModel(newTlSetting, view);
            }

            return timelogViewModel;
        }

        public BackupViewModel GetBackupViewModel()
        {
            BackupViewModel newBackupViewModel = new BackupViewModel();
            newBackupViewModel.DatabaseBackupname = "TimecardApp_" + DateTime.Now.ToString("yyyyMMdd_HHmm");
            return newBackupViewModel;
        }

        public ObservableCollection<Project> GetProjectsStatsForTimecard(string timecardID)
        {
            IDictionary<Project, long> projectTimes = new Dictionary<Project, long>();
            ObservableCollection<Project> projectForTimecard = new ObservableCollection<Project>();

            foreach (WorkTask worktask in worktaskCollection)
            {
                if (worktask.Project != null)
                {
                    if (projectTimes.ContainsKey(worktask.Project))
                    {
                        projectTimes[worktask.Project] = projectTimes[worktask.Project] + worktask.WorkTimeTicks;
                    }
                    else
                    {
                        projectTimes.Add(worktask.Project, worktask.WorkTimeTicks);
                    }
                }
            }

            foreach (KeyValuePair<Project, long> pair in projectTimes)
            {
                string workTimeString;
                //36000000000
                long minutesTicks = pair.Value % 36000000000;
                long minutes = minutesTicks / 600000000;
                long hours = (pair.Value - minutesTicks) / 36000000000;
                workTimeString = hours.ToString() + ":" + minutes.ToString("00");
                pair.Key.TotalWorkTimeString = workTimeString;
                projectForTimecard.Add(pair.Key);
            }

            return projectForTimecard;
        }

        public string GetProjectTotalWorktimeString(string projectID)
        {
            string workTimeString;

            long totalWorkSum = 0;

            foreach (WorkTask worktask in GetWorktasksForProject(projectID))
            {
                totalWorkSum = totalWorkSum + worktask.WorkTimeTicks;
            }

            //36000000000
            long minutesTicks = totalWorkSum % 36000000000;
            long minutes = minutesTicks / 600000000;
            long hours = (totalWorkSum - minutesTicks) / 36000000000;
            //long hourDiff = hours % 24;
            //long days = (hours - hourDiff) / 24;

            workTimeString = hours.ToString() + ":" + minutes.ToString("00");

            return workTimeString;
        }

        public string GetProjectQuoteWorktimeString(string projectID)
        {
            string quoteWorkTimeString = "00:00";

            long totalWorkSum = 0;

            ObservableCollection<WorkTask> tmpCol = GetWorktasksForProject(projectID);
            foreach (WorkTask worktask in tmpCol)
            {
                totalWorkSum = totalWorkSum + worktask.WorkTimeTicks;
            }
            if (tmpCol.Count > 0)
            {
                long restTicksPerTask = totalWorkSum % tmpCol.Count;
                long ticksPerTask = (totalWorkSum - restTicksPerTask) / tmpCol.Count;

                long minutesTicksPT = ticksPerTask % 36000000000;
                long minutesPT = minutesTicksPT / 600000000;
                long hoursPT = (ticksPerTask - minutesTicksPT) / 36000000000;
                quoteWorkTimeString = hoursPT.ToString("00") + ":" + minutesPT.ToString("00");
            }

            return quoteWorkTimeString;
        }

        public ObservableCollection<TimelogTask> GetTimelogTasksForDate(DateTime dateTime)
        {
            var timelogTasks = from TimelogTask task in dellAppDB.TimelogTasks 
                                where (task.StartDate.Date <= dateTime.Date && task.EndDate.Date >= dateTime.Date) || (task.StartDate == null || task.EndDate == null)
                                select task;

            return new ObservableCollection<TimelogTask>(timelogTasks);
        }

        public ObservableCollection<WorkTask> GetAllWorktasksForTimelog()
        {
            var worktasks = from WorkTask worktask in dellAppDB.WorkTasks
                            where worktask.IsForTimelogRegistration == true && 
                                    (worktask.LastTimelogRegistration == String.Empty || worktask.LastTimelogRegistration == null) &&
                                    worktask.TimelogTask != null 
                            select worktask;

            return new ObservableCollection<WorkTask>(worktasks);
        }

        public void LoadCollectionsFromDatabase()
        {
            var customersInDB = from Customer customer in dellAppDB.Customer
                                select customer;

            CustomerCollection = new ObservableCollection<Customer>(customersInDB);

            var projectsInDB = from Project project in dellAppDB.Projects
                               select project;

            ProjectCollection = new ObservableCollection<Project>(projectsInDB);
        }

        public void LoadCurrentTimecardsFromDatabase()
        {
            var settingObj = from Setting setting in dellAppDB.Setting
                             select setting;

            Setting tmpSetting = settingObj.Single();

            var timecardsInDB = (from Timecard timecard in dellAppDB.Timecards
                                 orderby timecard.StartDate descending
                                 select timecard).Take(tmpSetting.NumberTimecards);


            CurrentTimecardCollection = new ObservableCollection<Timecard>(timecardsInDB);
        }

        public void LoadOpenTimecardCollectionFromDatabase()
        {
            var timecardsInDB = from Timecard timecard in dellAppDB.Timecards
                                where timecard.IsComplete == false
                                orderby timecard.StartDate descending
                                select timecard;


            OpenTimecardCollection = new ObservableCollection<Timecard>(timecardsInDB);
        }

        public void LoadFilterTimecardCollectionFromDatabase(Filter filter)
        {
            if (filter == null)
            {
                var filterObj = from Filter filterSQL in dellAppDB.Filter
                                where filterSQL.FilterObject == "Timecard"
                                select filterSQL;
                if (filterObj.Count() > 0)
                    filter = filterObj.Single();
            }

            if (filter != null)
            {
                if (filter.FilterMode == "both")
                    if (!String.IsNullOrEmpty(filter.FilterSearchString))
                    {

                        var objectsFromDB = from Timecard timecard in dellAppDB.Timecards
                                            where timecard.TimecardName.Contains(filter.FilterSearchString) && (timecard.StartDate >= filter.StartDate && timecard.StartDate <= filter.EndDate)
                                            orderby timecard.StartDate descending
                                            select timecard;


                        if (objectsFromDB.Count() > 0)
                            FilteredTimecardCollection = new ObservableCollection<Timecard>(objectsFromDB);
                    }

                if (filter.FilterMode == "date")
                {
                    var objectsFromDB = from Timecard timecard in dellAppDB.Timecards
                                        where timecard.StartDate >= filter.StartDate && timecard.StartDate <= filter.EndDate
                                        orderby timecard.StartDate descending
                                        select timecard;


                    if (objectsFromDB.Count() > 0)
                        FilteredTimecardCollection = new ObservableCollection<Timecard>(objectsFromDB);
                }

                if (filter.FilterMode == "string")
                    if (!String.IsNullOrEmpty(filter.FilterSearchString))
                    {

                        var objectsFromDB = from Timecard timecard in dellAppDB.Timecards
                                            where timecard.TimecardName.Contains(filter.FilterSearchString)
                                            orderby timecard.StartDate descending
                                            select timecard;


                        if (objectsFromDB.Count() > 0)
                            FilteredTimecardCollection = new ObservableCollection<Timecard>(objectsFromDB);
                    }
            }
        }

        public ObservableCollection<Project> GetProjectsForCustomer(string customerID)
        {
            var projectsForCustomerInDB = from Project project in dellAppDB.Projects
                                          where project.CustomerID == customerID

                                          select project;

            return new ObservableCollection<Project>(projectsForCustomerInDB);
        }

        public ObservableCollection<Project> GetActiveProjects()
        {
            var projectsInDB = from Project project in dellAppDB.Projects
                               where project.IsInactive == false
                               select project;

            return new ObservableCollection<Project>(projectsInDB);
        }

        public ObservableCollection<WorkTask> GetWorktasksForProject(string projectID)
        {
            var worktasksForProjectsInDB = from WorkTask worktask in dellAppDB.WorkTasks
                                           where worktask.ProjectID == projectID
                                           orderby worktask.DayDate descending
                                           select worktask;

            return new ObservableCollection<WorkTask>(worktasksForProjectsInDB);
        }


        public ObservableCollection<TimelogTask> GetTimelogTasks()
        {
            var timelogTasksInDB = from TimelogTask task in dellAppDB.TimelogTasks 
                                           select task;

            return new ObservableCollection<TimelogTask>(timelogTasksInDB);
        }

        public void LoadWorktasksForTimecard(string timecardID)
        {
            var worktasksOfThisTimecard = from WorkTask worktask in dellAppDB.WorkTasks
                                          where worktask.TimecardID == timecardID
                                          orderby worktask.DayDate
                                          select worktask;

            WorktaskCollection = new ObservableCollection<WorkTask>(worktasksOfThisTimecard);
        }

        public Timecard getTimecardForDate(DateTime date)
        {
            var timecardForDate = from Timecard timecard in dellAppDB.Timecards
                                  where timecard.StartDate <= date && timecard.StartDate.AddDays(7) > date
                                  select timecard;

            if (timecardForDate.Count() == 1)
                return timecardForDate.Single();
            else
                return null;
        }
        #endregion

        #region discard methods
        public void DiscardWorktaskViewModel()
        {
            workTaskViewModel = null;
        }

        public void DiscardTimecardViewModel()
        {
            timecardViewModel = null;
        }

        public void DiscardProjectViewModel()
        {
            projectViewModel = null;
        }

        public void DiscardCustomerViewModel()
        {
            customerViewModel = null;
        }

        public void DiscardSettingViewModel()
        {
            settingViewModel = null;
        }

        public void DiscardTlSettingViewModel()
        {
            timelogViewModel  = null;
        }

        #endregion

        #region add and delete methods
        public void AddNewTimecard(Timecard tc)
        {
            // nur wenn diese Timecard noch nicht existiert wird sie angelegt
            if (!dellAppDB.Timecards.Where(u => u.StartDate.Year == tc.StartDate.Year && u.StartDate.Day == tc.StartDate.Day && u.StartDate.Month == tc.StartDate.Month).Any() || dellAppDB.Timecards.Count() == 0)
            {
                // Add a timecard to the observable collection.
                addTimecardOnTop(tc);

                //currentTimecardCollection.Add(tc);

                // Add timecard and days to the local database.
                dellAppDB.Timecards.InsertOnSubmit(tc);

                dellAppDB.SubmitChanges();

                WorkTask moWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = tc.TimecardID, DayDate = tc.StartDate.Date, Ident_WorkTask = HelperClass.GetIdentForWorktask(tc.StartDate, "") };
                WorkTask diWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = tc.TimecardID, DayDate = tc.StartDate.AddDays(1).Date, Ident_WorkTask = HelperClass.GetIdentForWorktask(tc.StartDate.AddDays(1), "") };
                WorkTask miWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = tc.TimecardID, DayDate = tc.StartDate.AddDays(2).Date, Ident_WorkTask = HelperClass.GetIdentForWorktask(tc.StartDate.AddDays(2), "") };
                WorkTask doWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = tc.TimecardID, DayDate = tc.StartDate.AddDays(3).Date, Ident_WorkTask = HelperClass.GetIdentForWorktask(tc.StartDate.AddDays(3), "") };
                WorkTask frWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = tc.TimecardID, DayDate = tc.StartDate.AddDays(4).Date, Ident_WorkTask = HelperClass.GetIdentForWorktask(tc.StartDate.AddDays(4), "") };

                dellAppDB.WorkTasks.InsertOnSubmit(moWorktask);
                dellAppDB.WorkTasks.InsertOnSubmit(diWorktask);
                dellAppDB.WorkTasks.InsertOnSubmit(miWorktask);
                dellAppDB.WorkTasks.InsertOnSubmit(doWorktask);
                dellAppDB.WorkTasks.InsertOnSubmit(frWorktask);

                // Save changes to the database.
                dellAppDB.SubmitChanges();
            }
            else
            {
                MessageBox.Show("There is already an existing timecard for this week.");
            }
        }

        private void addTimecardOnTop(Timecard tc)
        {
            ObservableCollection<Timecard> tmpCollection = new ObservableCollection<Timecard>();

            tmpCollection.Add(tc);

            foreach (Timecard singleTimecard in currentTimecardCollection)
            {
                tmpCollection.Add(singleTimecard);
            }

            CurrentTimecardCollection = tmpCollection;

        }

        public void deleteTimecard(Timecard tc)
        {
            CurrentTimecardCollection.Remove(tc);
            dellAppDB.Timecards.DeleteOnSubmit(tc);

            // Alle Worktasks müssen auch noch entfernt werden!!!
            var worktaskOfTimecard = from WorkTask worktask in dellAppDB.WorkTasks
                                     where worktask.TimecardID == tc.TimecardID
                                     select worktask;

            ObservableCollection<WorkTask> tmpCollection = new ObservableCollection<WorkTask>(worktaskOfTimecard);
            foreach (WorkTask singleWorktask in tmpCollection)
            {
                dellAppDB.WorkTasks.DeleteOnSubmit(singleWorktask);
            }

            // Save changes to the database.
            dellAppDB.SubmitChanges();
        }

        public void DeleteWorktask(WorkTask thisWorkTask)
        {
            dellAppDB.WorkTasks.DeleteOnSubmit(thisWorkTask);
            dellAppDB.SubmitChanges();
        }

        public void deleteProject(Project project)
        {

            var worktaskCount = from WorkTask worktask in dellAppDB.WorkTasks
                                where worktask.ProjectID == project.ProjectID
                                select worktask;

            if (worktaskCount.Count() == 0)
            {
                projectCollection.Remove(project);
                dellAppDB.Projects.DeleteOnSubmit(project);

                // Save changes to the database.
                dellAppDB.SubmitChanges();
            }
            else
                MessageBox.Show("Project " + project.Ident_Project + " cannot be deleted, because it is used for at least one worktask! But you can set the project on inactive, to shorten the lists.");
        }

        public void deleteCustomer(Customer customer)
        {
            var projectCount = from Project project in dellAppDB.Projects
                               where project.CustomerID == customer.CustomerID
                               select project;

            if (projectCount.Count() == 0)
            {
                customerCollection.Remove(customer);
                dellAppDB.Customer.DeleteOnSubmit(customer);

                // Save changes to the database.
                dellAppDB.SubmitChanges();
            }
            else
            {
                MessageBoxButton buttons = MessageBoxButton.OKCancel;
                MessageBoxResult result = MessageBox.Show("There are still projects which references to this customer. Do you wanna try to delete all of them?", "", buttons);

                if (result == MessageBoxResult.OK)
                {
                    var projectsOfCustomer = from Project project in dellAppDB.Projects
                                             where project.CustomerID == customer.CustomerID
                                             select project;

                    ObservableCollection<Project> tmpCollection = new ObservableCollection<Project>(projectsOfCustomer);
                    foreach (Project singleProject in tmpCollection)
                    {
                        deleteProject(singleProject);
                    }

                    customerCollection.Remove(customer);
                    dellAppDB.Customer.DeleteOnSubmit(customer);

                    // Save changes to the database.
                    dellAppDB.SubmitChanges();
                }
            }
        }

        #endregion

        #region helper
        private IDBObjects getDBObjectForID(DBObjects objectType, String objectID)
        {
            switch (objectType)
            {
                case DBObjects.Customer:
                    var queryResult = from Customer customer in dellAppDB.Customer
                                      where customer.CustomerID == objectID
                                      select customer;

                    if (queryResult.Count() != 0)
                        return queryResult.Single();
                    else
                        return null;

                case DBObjects.Project:
                    var queryResult1 = from Project project in dellAppDB.Projects
                                       where project.ProjectID == objectID
                                       select project;

                    if (queryResult1.Count() != 0)
                        return queryResult1.Single();
                    else
                        return null;

                case DBObjects.Worktask:
                    var queryResult2 = from WorkTask worktask in dellAppDB.WorkTasks
                                       where worktask.WorkTaskID == objectID
                                       select worktask;

                    if (queryResult2.Count() != 0)
                        return queryResult2.Single();
                    else
                        return null;

                case DBObjects.Timecard:
                    var queryResult3 = from Timecard timecard in dellAppDB.Timecards
                                       where timecard.TimecardID == objectID
                                       select timecard;

                    if (queryResult3.Count() != 0)
                        return queryResult3.Single();
                    else
                        return null;

                case DBObjects.Setting:
                    var queryResult4 = from Setting setting in dellAppDB.Setting
                                       where setting.SettingID == objectID
                                       select setting;
                    if (queryResult4.Count() != 0)
                        return queryResult4.Single();
                    else
                        return null;
                default:
                    return null;
            }
        }

        public void RestoreDatabase(string tmpPathDatabase)
        {
            dellAppDB.Dispose();
            //check downloaded database for version

            string tmpDBConnectionString = "Data Source=isostore:/" + tmpPathDatabase;
            // Create the database if it does not exist.
            using (DBClass tmpDB = new DBClass(tmpDBConnectionString))
            {
                if (tmpDB.DatabaseExists() == true)
                {
                    using (DBMigrator migrator = new DBMigrator(tmpDBConnectionString, App.DB_VERSION))
                    {
                        try
                        {
                            if (migrator.hasToMigrate())
                            {
                                migrator.MigrateDatabase();
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error during migration. Migration failed.");
                        }
                    }

                    // version are equal or db was migrated -> replace database files 
                    tmpDB.Dispose();
                    IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
                    iso.CopyFile(tmpPathDatabase, AppResources.DatabaseName + ".sdf", true);

                    iso.Dispose();
                }
                else
                    MessageBox.Show("Restore failed because the downloaded file is no database for this app.");
            }

            this.ConnectDB();
        }

        #endregion

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
