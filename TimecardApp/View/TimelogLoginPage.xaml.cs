using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TimecardApp.Model.NonPersistent;
using TimecardApp.ViewModel;
using TimecardApp.Model.Timelog;

namespace TimecardApp.View
{
    public partial class TimelogLoginPage : PhoneApplicationPage, ITimelogUsingView 
    {
        private TimelogViewModel timelogViewModel;

        public TimelogLoginPage()
        {
            InitializeComponent();
            this.LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            timelogViewModel.ExecuteTlOperation(TimelogOperation.Login);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timelogViewModel = App.AppViewModel.GetTimelogViewModel(this);

            this.DataContext = timelogViewModel;
            base.OnNavigatedTo(e);
        }

        public void connectionFinished()
        {
            this.LoadingPanel.Visibility = Visibility.Collapsed;
        }

        public void connectionStarted()
        {
            this.LoadingPanel.Visibility = Visibility.Visible;
        }
    }
}