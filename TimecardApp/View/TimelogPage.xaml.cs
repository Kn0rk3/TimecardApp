using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TimecardApp.ViewModel;
using TimecardApp.Model.Timelog;
using TimecardApp.Model;
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.View
{
    public partial class TimelogPage : PhoneApplicationPage, ITimelogUsingView
    {
        private string helperStringUpload = "-Help- Upload your work into Timelog: " +
            "\n\nFirst things first: Login into Timelog and download your assinged tasks. " + 
            "\n\nSecond: Edit the worktasks and assign them to the corresponding timelog task. Save them." + 
            "\n\nThird: Upload all worktasks shown here in this list into timelog with one single click. That's all." + 
            "\n\nImportant: You need to check and close the week in timelog manually via your usual website!" + 
            "\n\nHint: In case you forgot something, you can reset a worktask for an additional upload.";
        
        private TimelogViewModel timelogViewModel;
        private ApplicationBarIconButton appBarHome;
        private ApplicationBarIconButton appBarSyncData;
        private ApplicationBarIconButton appBarEdit;
        private ApplicationBarIconButton appBarUpload;
        private ApplicationBarIconButton appBarHelperUpload;

        public TimelogPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            appBarHome = new ApplicationBarIconButton(new Uri("Icons/map.neighborhood.png", UriKind.Relative));
            appBarHome.Text = "Home";
            appBarHome.Click += new System.EventHandler(this.homeButton_Click);

            appBarSyncData = new ApplicationBarIconButton(new Uri("Icons/download.png", UriKind.Relative));
            appBarSyncData.Text = "Load Tl tasks";
            appBarSyncData.Click += new System.EventHandler(this.downloadDataButton_Click);

            appBarEdit = new ApplicationBarIconButton(new Uri("Resources/edit.png", UriKind.Relative));
            appBarEdit.Text = "Edit Login";
            appBarEdit.Click += new System.EventHandler(this.editTimelogSetting_Click);

            appBarUpload = new ApplicationBarIconButton(new Uri("Icons/upload.png", UriKind.Relative));
            appBarUpload.Text = "Upload";
            appBarUpload.Click += new System.EventHandler(this.uploadWorkunitsButton_Click);

            appBarHelperUpload = new ApplicationBarIconButton(new Uri("Icons/questionmark.png", UriKind.Relative));
            appBarHelperUpload.Text = "Help";
            appBarHelperUpload.Click += new System.EventHandler(this.appBarHelperUpload_Click);
        }

        private void TimelogPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHome);
                    ApplicationBar.Buttons.Add(appBarEdit);
                    ApplicationBar.Buttons.Add(appBarUpload);
                    ApplicationBar.Buttons.Add(appBarHelperUpload);
                    break;
                case 1:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHome);
                    ApplicationBar.Buttons.Add(appBarEdit);
                    ApplicationBar.Buttons.Add(appBarSyncData);
                    ApplicationBar.Buttons.Add(appBarHelperUpload);
                    break;
            }
        }

        private void appBarHelperUpload_Click(object sender, EventArgs e)
        {
            MessageBox.Show(helperStringUpload);
        }
        
        private void editTimelogSetting_Click(object sender, EventArgs e)
        {
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {
                timelogViewModel.SaveThisTlSetting();
                App.AppViewModel.DiscardTlSettingViewModel();
                NavigationService.Navigate(new Uri("/View/TimelogLoginPage.xaml", UriKind.Relative));
            }
        }

        private void uploadWorkunitsButton_Click(object sender, EventArgs e)
        {
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {
                timelogViewModel.ExecuteTlOperation(ETimelogOperation.UploadWorkunits);
            }
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {
                timelogViewModel.SaveThisTlSetting();
                App.AppViewModel.DiscardTlSettingViewModel();
                NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
            }
        }

        private void downloadDataButton_Click(object sender, EventArgs e)
        {
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {
                timelogViewModel.ExecuteTlOperation(ETimelogOperation.GetTasks);
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
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {
                App.AppViewModel.DiscardTlSettingViewModel();
                NavigationService.Navigate(new Uri("/View/TimelogLoginPage.xaml", UriKind.Relative));
            }
        }

        public void NavigateBack()
        {
            
        }

        private void editWorktaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {            
                var button = sender as Button;

                if (button != null)
                {
                    // Get a handle for the to-do item bound to the button.
                    WorkTask worktask = button.DataContext as WorkTask;

                    timelogViewModel.SaveThisTlSetting();
                    App.AppViewModel.DiscardTlSettingViewModel();

                    // hier muss noch die ID mitgegeben werden, dass die Timecard korrekt geladen werden kann
                    NavigationService.Navigate(new Uri("/View/WorktaskPage.xaml?worktaskIDParam=" + worktask.WorkTaskID, UriKind.Relative));
                }
            }
        }
    }
}