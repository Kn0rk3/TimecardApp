using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.TimelogSecurityService;
using TimecardApp.TimelogProjectManagementService;
using System.ServiceModel.Channels;
using System.ServiceModel;



namespace TimecardApp.Model.Timelog
{
    public class TimelogWrapper : ITimelogWrapper 
    {

        public void loginTimelog()
        {
            var _binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            var _endpoint = new EndpointAddress("https://tl.timelog/leuschel" + "/WebServices/Security/V1_2/SecurityServiceSecure.svc");
            SecurityService newService = new SecurityServiceClient(_binding, _endpoint);
            



        }
    }
}
