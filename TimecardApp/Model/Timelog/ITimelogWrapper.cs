using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.TimelogProjectManagementService;

namespace TimecardApp.Model.Timelog
{
    public interface ITimelogWrapper 
    {
        void LoginTimelog(string url, string initials, string password);
        void GetTimelogTasks();
        bool IsValidSecurityToken();
        void UploadWorkunits(ObservableCollection<WorkUnit> insertUnits, ObservableCollection<WorkUnit> updateUnits);
    }
}
