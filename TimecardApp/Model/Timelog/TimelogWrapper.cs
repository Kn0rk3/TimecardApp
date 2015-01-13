using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using TimecardApp.TimelogSecurityService;
using TimecardApp.TimelogProjectManagementService;
using System.ServiceModel.Channels;
using System.ServiceModel;
using TimecardApp.Model.Timelog;
using System.Collections.ObjectModel;
using TimecardApp.ViewModel;

namespace TimecardApp.Model.Timelog
{
    public class TimelogWrapper : ITimelogWrapper 
    {
        // this model reference is for locking the page during network activity
        private ITimelogViewModel tlViewModel;

        public TimelogWrapper(ITimelogViewModel tlViewModel)
        {
            this.tlViewModel = tlViewModel;
        }

        public void LoginTimelog(string url, string initials, string password)
        {
            tlViewModel.ChangeState(ETimelogState.Running, ETimelogOperation.Login, String.Empty);

            TimelogSession tlSession = TimelogSession.Instance;
            tlSession.SessionUrl = url;

            SecurityServiceClient tlSecurityClient = tlSession.SecurityClient;
            tlSecurityClient.GetTokenCompleted += tlSecurityClient_GetTokenCompleted;
            
            tlSecurityClient.GetTokenAsync(initials, password);            
        }

        //public void GetTimelogProjects(bool backgroundLoginBfore)
        //{
        //    tlViewModel.StartTimelogConnection();

        //    ObservableCollection<TimelogProject> projects = new ObservableCollection<TimelogProject>();
        //    TimelogSession tlSession = TimelogSession.Instance;

        //    ProjectManagementServiceClient tlProjectClient = tlSession.ProjectManagementClient;
            
            
        //}

        public void  GetTimelogTasks()
        {
            tlViewModel.ChangeState(ETimelogState.Running, ETimelogOperation.GetTasks, String.Empty);

            TimelogSession tlSession = TimelogSession.Instance;

            ProjectManagementServiceClient tlProjectClient = tlSession.ProjectManagementClient;
            tlProjectClient.GetTasksAllocatedToEmployeeCompleted += tlProjectClient_GetTasksCompleted;
            tlProjectClient.GetTasksAllocatedToEmployeeAsync(tlSession.ProjectManagementToken.Initials , tlSession.ProjectManagementToken);
        }

        public  bool IsValidSecurityToken()
        {
            TimelogSession tlSession = TimelogSession.Instance;
            if (tlSession.SecurityToken != null)
            {
                if (tlSession.SecurityToken.Expires > DateTime.UtcNow)
                    return true;
                else
                    return false;
            }
            else
                return false;
            
        }

        private void tlSecurityClient_GetTokenCompleted(object sender, GetTokenCompletedEventArgs e)
        {
            try
            {
                if (e.Result.ResponseState == TimelogSecurityService.ExecutionStatus.Success)
                {
                    if (!String.IsNullOrEmpty(e.Result.Return[0].ToString()))
                    {
                        TimelogSession tlSession = TimelogSession.Instance;
                        tlSession.SecurityToken = e.Result.Return[0];
                        tlViewModel.ChangeState(ETimelogState.ExectionSuccessfull, ETimelogOperation.Login , String.Empty);                        
                    }
                }
            }
            catch (Exception ex)
            {
                tlViewModel.ChangeState(ETimelogState.UnexpectedError, ETimelogOperation.Login, e.Error.Message );
            }
        }

        private void tlProjectClient_GetTasksCompleted(object sender, GetTasksAllocatedToEmployeeCompletedEventArgs e)
        {
            try
            {
                if (e.Result.ResponseState == TimelogProjectManagementService.ExecutionStatus.Success)
                {
                    ObservableCollection<TimelogProjectManagementService.Task> colTasks = e.Result.Return;
                    if (colTasks != null)
                    {
                        tlViewModel.SetTimelogTasks(colTasks);
                        tlViewModel.ChangeState(ETimelogState.ExectionSuccessfull, ETimelogOperation.GetTasks, String.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                tlViewModel.ChangeState(ETimelogState.UnexpectedError, ETimelogOperation.GetTasks, e.Error.Message );
            }
        }
    }
}
