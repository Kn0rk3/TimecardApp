using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.TimelogProjectManagementService;
using TimecardApp.TimelogSecurityService;

namespace TimecardApp.Model.Timelog
{
    public class TimelogSession
    {
        public string SessionUrl = "https://tl.timelog/leuschel";
        private static TimelogSession instance;

        public static TimelogSession Instance
        {
            get
            {
                return instance ?? (instance = new TimelogSession());
            }
        }

        private TimelogSession()
        {
        }
        
        private ProjectManagementServiceClient projectManagementClient;
        public ProjectManagementServiceClient ProjectManagementClient
        {
            get
            {
                if (projectManagementClient == null)
                {
                    var _binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport) { MaxReceivedMessageSize = 1024000 };
                    var _endpoint = new EndpointAddress(SessionUrl + "/WebServices/ProjectManagement/V1_6/ProjectManagementServiceSecure.svc");
                    projectManagementClient = new ProjectManagementServiceClient(_binding, _endpoint);
                    
                }

                return projectManagementClient;
            }
        }

        private SecurityServiceClient securityClient;
        public SecurityServiceClient SecurityClient
        {
            get
            {
                if (securityClient == null)
                {
                    var _binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                    var _endpoint = new EndpointAddress(SessionUrl + "/WebServices/Security/V1_2/SecurityServiceSecure.svc");
                    securityClient = new SecurityServiceClient(_binding, _endpoint);
                }

                return securityClient;
            }
        }

        private TimelogSecurityService.SecurityToken securityToken;
        public TimelogSecurityService.SecurityToken SecurityToken
        {
            get
            {
                if (securityToken == null)
                {
                    return null;
                }
                return securityToken;
            }
            set
            {
                securityToken = value;
            }
        }

        private TimelogProjectManagementService.SecurityToken projectManagementToken;
        public TimelogProjectManagementService.SecurityToken ProjectManagementToken
        {
            get
            {
                return new TimelogProjectManagementService.SecurityToken
                {
                    Expires = SecurityToken.Expires,
                    Hash = SecurityToken.Hash,
                    Initials = SecurityToken.Initials
                };
            }
        }
    }
}
