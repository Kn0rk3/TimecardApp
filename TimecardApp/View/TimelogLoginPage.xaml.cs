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
        private string helperStringLogin = "-Help- Credentials for your Timelog access: " +
            "\n\nFor a smoother experience with this Timelog interface it is recommended to save the credentials and activate the automatically login. " +
            "The credentials will be saved encrypted into this local database and can only be decrypted with this device." +
            "\n\nYou can always navigate to this page, in case you need to change data.";

        private TimelogViewModel timelogViewModel;
        private ApplicationBarIconButton appBarBackButton;
        private ApplicationBarIconButton appBarLoginButton;
        private ApplicationBarIconButton appBarHelpButton;

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

            appBarHelpButton = new ApplicationBarIconButton(new Uri("Icons/questionmark.png", UriKind.Relative));
            appBarHelpButton.Text = "Help";
            appBarHelpButton.Click += new System.EventHandler(this.appBarHelperLogin_Click);

            ApplicationBar.Buttons.Add(appBarBackButton);
            ApplicationBar.Buttons.Add(appBarLoginButton);
            ApplicationBar.Buttons.Add(appBarHelpButton);
        }

        private void appBarHelperLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show(helperStringLogin);
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
            if (!String.IsNullOrEmpty(message))
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