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
        // Specify the connection string as a static, used in main page and app.xaml.
        //public static string DBConnectionString = "Data Source=isostore:/DellApp.sdf";

        // Pass the connection string to the base class.
        public DBClass(string connectionString) : base(connectionString)
        { }

        public Table<Timecard> Timecards;
        public Table<Project> Projects;
        public Table<Setting> Setting;
        public Table<WorkTask> WorkTasks;
        public Table<Customer> Customer;
        public Table<Filter> Filter;
    }
}
