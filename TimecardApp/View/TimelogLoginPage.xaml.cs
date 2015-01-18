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
        private ApplicationBarIconButton appBarBackButton;
        private ApplicationBarIconButton appBarLoginButton;

        public TimelogLoginPage()
        {
            InitializeComponent(); BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            appBarBackButton = new ApplicationBarIconButton(new Uri("Icons/back.png", UriKind.Relative));
            appBarBackButton.Text = "Back";
            appBarBackButton.Click += new System.EventHandler(this.backButton_Click);

            appBarLoginButton = new ApplicationBarIconButton(new Uri("Icons/check.png", UriKind.Relative));
            appBarLoginButton.Text = "Login";
            appBarLoginButton.Click += new System.EventHandler(this.loginButton_Click);

            ApplicationBar.Buttons.Add(appBarBackButton);
            ApplicationBar.Buttons.Add(appBarLoginButton);      
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            NavigateBack();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {
                HelperClass.FocusedTextBoxUpdateSource();
                timelogViewModel.ExecuteTlOperation(ETimelogOperation.Login);
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

        }


        public void NavigateBack()
        {
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {
                timelogViewModel.SaveThisTlSetting();
                App.AppViewModel.DiscardTlSettingViewModel();
                NavigationService.GoBack();
            }
        }
    }
}