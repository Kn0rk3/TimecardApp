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

        public void GetTimelogTasks()
        {
            tlViewModel.ChangeState(ETimelogState.Running, ETimelogOperation.GetTasks, String.Empty);

            TimelogSession tlSession = TimelogSession.Instance;

            ProjectManagementServiceClient tlProjectClient = tlSession.ProjectManagementClient;
            tlProjectClient.GetTasksAllocatedToEmployeeCompleted += tlProjectClient_GetTasksCompleted;
            tlProjectClient.GetTasksAllocatedToEmployeeAsync(tlSession.ProjectManagementToken.Initials, tlSession.ProjectManagementToken);
        }

        public void UploadWorkunits(ObservableCollection<WorkUnit> insertUnits, ObservableCollection<WorkUnit> updateUnits)
        {
            tlViewModel.ChangeState(ETimelogState.Running, ETimelogOperation.UploadWorkunits, String.Empty);

            TimelogSession tlSession = TimelogSession.Instance;

            ProjectManagementServiceClient tlProjectClient = tlSession.ProjectManagementClient;
            tlProjectClient.InsertWorkCompleted += tlProjectClient_InsertWorkCompleted;
            tlProjectClient.UpdateWorkCompleted += tlProjectClient_UpdateWorkCompleted;

            if (insertUnits.Count > 0)
                tlProjectClient.InsertWorkAsync(insertUnits, 0, tlSession.ProjectManagementToken);
            if (updateUnits.Count > 0)
                tlProjectClient.UpdateWorkAsync(updateUnits, 0, tlSession.ProjectManagementToken);
        }

        public bool IsValidSecurityToken()
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

        private void tlProjectClient_UpdateWorkCompleted(object sender, UpdateWorkCompletedEventArgs e)
        {
            try
            {
                ObservableCollection<BatchContainerOfWorkUnit> returnWorkunits = e.Result.Return;
                if (returnWorkunits != null)
                    tlViewModel.UpdateWorktasksForReturnedWorkunits(returnWorkunits);

                if (e.Result.ResponseState == TimelogProjectManagementService.ExecutionStatus.Success)
                    tlViewModel.ChangeState(ETimelogState.Successfull, ETimelogOperation.UploadWorkunits, String.Empty);
                else
                    if (e.Error != null)
                        tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.UploadWorkunits, e.Error.Message);
                    else
                        tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.UploadWorkunits, String.Empty);
            }
            catch (Exception ex)
            {
                if (e.Error != null)
                    tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.UploadWorkunits, e.Error.Message);
                else
                    tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.UploadWorkunits, ex.Message);
            }
        }

        private void tlProjectClient_InsertWorkCompleted(object sender, InsertWorkCompletedEventArgs e)
        {
            try
            {
                ObservableCollection<BatchContainerOfWorkUnit> returnWorkunits = e.Result.Return;
                if (returnWorkunits != null)
                    tlViewModel.UpdateWorktasksForReturnedWorkunits(returnWorkunits);

                if (e.Result.ResponseState == TimelogProjectManagementService.ExecutionStatus.Success)
                    tlViewModel.ChangeState(ETimelogState.Successfull, ETimelogOperation.UploadWorkunits, String.Empty);
                else
                    if (e.Error != null)
                        tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.UploadWorkunits, e.Error.Message);
                    else
                        tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.UploadWorkunits, String.Empty);
            }
            catch (Exception ex)
            {
                if (e.Error != null)
                    tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.UploadWorkunits, e.Error.Message);
                else
                    tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.UploadWorkunits, ex.Message);
            }
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
                    }
                    tlViewModel.ChangeState(ETimelogState.Successfull, ETimelogOperation.Login, String.Empty);
                }
                else
                    if (e.Error != null)
                        tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.Login, e.Error.Message);
                    else
                        tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.Login, String.Empty);                    
            }
            catch (Exception ex)
            {
                if (e.Error != null)
                    tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.Login, e.Error.Message); 
                else
                    tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.Login, ex.Message);                
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
                        tlViewModel.SetTimelogTasks(colTasks);
                    tlViewModel.ChangeState(ETimelogState.Successfull, ETimelogOperation.GetTasks, String.Empty);
                }
                else
                    if (e.Error != null)
                        tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.GetTasks, e.Error.Message);
                    else
                        tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.GetTasks, String.Empty);    
            }
            catch (Exception ex)
            {
                if (e.Error != null)
                    tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.GetTasks, e.Error.Message);
                else
                    tlViewModel.ChangeState(ETimelogState.Error, ETimelogOperation.GetTasks, ex.Message);
            }
        }
    }
}
