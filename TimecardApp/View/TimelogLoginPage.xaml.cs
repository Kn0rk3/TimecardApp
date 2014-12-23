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
        private ApplicationBarIconButton backButton;
        private ApplicationBarIconButton loginButton;

        public TimelogLoginPage()
        {
            InitializeComponent(); BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            backButton = new ApplicationBarIconButton(new Uri("Icons/back.png", UriKind.Relative));
            backButton.Text = "Back";
            backButton.Click += new System.EventHandler(this.backButton_Click);

            loginButton = new ApplicationBarIconButton(new Uri("Icons/upload.png", UriKind.Relative));
            loginButton.Text = "Login";
            loginButton.Click += new System.EventHandler(this.loginButton_Click);

            ApplicationBar.Buttons.Add(backButton);
            ApplicationBar.Buttons.Add(loginButton);
            
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            NavigateBack();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (timelogViewModel.CurrentState != TimelogState.Running)
            {
                HelperClass.FocusedTextBoxUpdateSource();
                timelogViewModel.ExecuteTlOperation(TimelogOperation.Login);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timelogViewModel = App.AppViewModel.GetTimelogViewModel(this);

            this.DataContext = timelogViewModel;
            base.OnNavigatedTo(e);
        }

        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(message);
        }


        public void NavigateLogin()
        {
            if (timelogViewModel.CurrentState != TimelogState.Running)
            {
                App.AppViewModel.DiscardTlSettingViewModel();
                NavigationService.Navigate(new Uri("/View/TimelogLoginPage.xaml", UriKind.Relative));
            }
        }


        public void NavigateBack()
        {
            if (timelogViewModel.CurrentState != TimelogState.Running)
            {
                App.AppViewModel.DiscardTlSettingViewModel();
                NavigationService.GoBack();
            }
        }
    }
}