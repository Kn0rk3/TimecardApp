using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.Model
{
    public interface ITimelogWrapper 
    {
        void LoginTimelog(string url, string initials, string password);
    }
}
