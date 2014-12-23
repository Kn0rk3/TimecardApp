using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.View
{
    public interface ITimelogUsingView
    {
        void ShowErrorMessage(string message);
        void NavigateLogin();
        void NavigateBack();
    }
}
