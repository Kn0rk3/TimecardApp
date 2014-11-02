using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.TimelogSecurityService;
using TimecardApp.TimelogProjectManagementService;
using System.ServiceModel.Channels;
using System.ServiceModel;
using TimecardApp.Model.Timelog;



namespace TimecardApp.Model.Timelog
{
    public class TimelogWrapper : ITimelogWrapper 
    {
        /**
         *  ################ Muss noch:
         *  Es müssen hier noch die entsprechenden Funktionen für Worktasks und so eingetragen werden und alle müssen vor jeder Aktion zum Server über die SessionInstanz prüfen ob der Token noch gültig ist
         *  Wenn nicht mehr gültig, dann muss ein neues Login durchgeführt werden (wenn nötig, mit Navigation auf die Setting seite, wenn speichern der Credentials nicht angekreuzt wurde)
         **/
        private TimelogSession tlSession;
        private SecurityServiceClient tlSecurityClient;
        private ViewModel.TimelogViewModel timelogViewModel;

        public TimelogWrapper(ViewModel.TimelogViewModel timelogViewModel)
        {
            this.timelogViewModel = timelogViewModel;
        }

        public void LoginTimelog(string url, string initials, string password)
        {
            tlSession = TimelogSession.Instance;
            tlSession.SessionUrl = url;
            tlSecurityClient = tlSession.SecurityClient;

            tlSecurityClient.GetTokenCompleted += tlSecurityClient_GetTokenCompleted;          
            tlSecurityClient.GetTokenAsync(initials, password);            

        //    try
        //    {
            //    // Store the URL to support multiple TimeLog Project instances
            //    TimelogSession.Instance.SessionUrl = url.Replace("http://", "https://").Trim('/');

            //    // Fetch the token
            //    TimelogSession.Instance.SecurityClient.GetTokenAsync(initials, password);
                
            //    // Did we get a token?
            //    if (_response.ResponseState == TimelogSecurity.ExecutionStatus.Success)
            //    {
            //        // Store the token for later
            //        SessionHelper.Instance.SecurityToken = _response.Return[0];

            //        // Get the name of the user
            //        var _userResponse = SessionHelper.Instance.SecurityClient.GetUser(_response.Return[0]);
            //        SessionHelper.Instance.FirstName = _userResponse.Return[0].FirstName;
            //        SessionHelper.Instance.Url = url;
            //        SessionHelper.Instance.Initials = initials;
            //        SessionHelper.Instance.Authenticate(initials);

            //        // Authenticate with the application
            //        return RedirectToAction("Index", "Dashboard");
            //    }
            //    else
            //    {
            //        // Loop through the error messages and print them to the user
            //        foreach (var _item in _response.Messages.Where(m => m.ErrorCode > 0))
            //        {
            //            this.ViewData.ModelState.AddModelError(
            //                "Initials",
            //                _item.ErrorCode == 40001 ? "Initials or password wrong" : _item.Message);
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    // Url is most likely wrong
            //    ViewData.ModelState.AddModelError("Initials", "Unable to connect to the service. Please check the URL");
            //}

            //return this.SignOn();
        }

        private void tlSecurityClient_GetTokenCompleted(object sender, GetTokenCompletedEventArgs e)
        {
            try
            {
                if (e.Result.ResponseState == TimelogSecurityService.ExecutionStatus.Success)
                {
                    if (!String.IsNullOrEmpty(e.Result.Return[0].ToString()))
                    {
                        tlSession.SecurityToken = e.Result.Return[0];

                    }
                }
            }
            catch (Exception ex)
            { 
                
            }


           
        }

    }
}
