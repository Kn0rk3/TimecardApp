using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.Model
{
    public enum DBObjects { Project, Customer, Worktask, Timecard, Setting }

    class DBClass : DataContext
    {
        // Pass the connection string to the base class.
        public DBClass(string connectionString) : base(connectionString)
        { }

        public Table<Timecard> Timecards;
        public Table<Project> Projects;
        public Table<Setting> Setting;
        public Table<WorkTask> WorkTasks;
        public Table<Customer> Customer;
        public Table<Filter> Filter;

        //db version 2
        public Table<TimelogTask> TimelogTasks;
        public Table<TimelogProject> TimelogProjects;
        public Table<TimelogSetting> TimelogSetting;
    }
}
