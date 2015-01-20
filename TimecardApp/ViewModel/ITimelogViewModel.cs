using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.Model.Timelog;

namespace TimecardApp.ViewModel
{
    public interface ITimelogViewModel
    {
        void ChangeState(ETimelogState newState, ETimelogOperation runningOperation, string message);
        void SetTimelogTasks(ObservableCollection<TimelogProjectManagementService.Task> tasks);
        void UpdateWorktasksForReturnedWorkunits(ObservableCollection<TimelogProjectManagementService.BatchContainerOfWorkUnit> returnWorkunits);
    }
}
