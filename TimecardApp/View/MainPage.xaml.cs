using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TimecardApp.Resources;
using TimecardApp.Model;
using TimecardApp.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.Live;
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.View
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {

        private ApplicationBarIconButton appBarSettingsButton;
        private ApplicationBarIconButton appBarFilterButton;
        private ApplicationBarIconButton appBarAddWorktaskButton;
        private ApplicationBarIconButton appBarRefreshButton;
        private ApplicationBarIconButton appBarSendMailButton;
        private ApplicationBarIconButton appBarTimelogButton;

        private ApplicationBarMenuItem appBarMenuBackup;
        private ApplicationBarMenuItem appBarMenuTimelog;
        
        private int weekNum;
        public int WeekNum
        {
            get { return weekNum; }
            set
            {
                weekNum = value;
                App.AppViewModel.WeekText = "CW " + weekNum.ToString();
                App.AppViewModel.TimecardName = "CW " + weekNum.ToString() + " " + yearOfDate;
                NotifyPropertyChanged("WeekNum");
            }
        }
        
        private int yearOfDate;

        // Constructor
        public MainPage()
        {
            // Data context and observable collection are children of the main page.
            this.DataContext = App.AppViewModel;
            App.AppViewModel.DiscardTimecardViewModel();
            yearOfDate = DateTime.Today.Year;
            WeekNum = HelperClass.NumberOfWeek(DateTime.Today);
            
            InitializeComponent();

            BuildLocalizedApplicationBar();
        }

        /*EventHandler
         * 
         ************************/

        private void changeTimecardButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                Timecard timecard = button.DataContext as Timecard;

                // hier muss noch die ID mitgegeben werden, dass die Timecard korrekt geladen werden kann
                NavigationService.Navigate(new Uri("/View/TimecardPage.xaml?timecardIDParam=" + timecard.TimecardID, UriKind.Relative));
            }
        }

        private void newTimecardAddButton_Click(object sender, RoutedEventArgs e)
        {

            DateTime date = (DateTime)datePicker.Value;

            String timecardID = System.Guid.NewGuid().ToString();

            // Create a new timecard based on the text box.
            Timecard newTimecard = new Timecard { TimecardName = newTimecardTextBox.Text , IsComplete = false, TimecardID = timecardID, StartDate = HelperClass.GetFirstDayOfWeek(date) };

            App.AppViewModel.AddNewTimecard(newTimecard);
        }

        private void deleteTimecardButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast parameter as a button.
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                Timecard timecardForDelete = button.DataContext as Timecard;

                MessageBoxButton buttons = MessageBoxButton.OKCancel;
                MessageBoxResult result = MessageBox.Show("Are you sure to delete the timesheet " + timecardForDelete.TimecardName + "?", "", buttons);

                if (result == MessageBoxResult.OK)
                {
                    App.AppViewModel.deleteTimecard(timecardForDelete);

                    // Put the focus back to the main page.
                    this.Focus();
                }
            }
        }

        void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime date = (DateTime)e.NewDateTime;
            yearOfDate = date.Year;
            WeekNum = HelperClass.NumberOfWeek(date);
        }


        private void BuildLocalizedApplicationBar()
        {
            appBarMenuBackup  = new ApplicationBarMenuItem();
            appBarMenuBackup.Text = "Backup/Restore (OneDrive)";
            appBarMenuBackup.Click += new System.EventHandler(this.backupButton_Click);

            if (App.AppViewModel.UsingTimelogInterface)
            {
                appBarMenuTimelog = new ApplicationBarMenuItem();
                appBarMenuTimelog.Text = "Timelog";
                appBarMenuTimelog.Click += new System.EventHandler(this.timelogButton_Click);

                appBarTimelogButton = new ApplicationBarIconButton(new Uri("Icons/feature.alarm.png", UriKind.Relative));
                appBarTimelogButton.Text = "Timelog";
                appBarTimelogButton.Click += new System.EventHandler(this.timelogButton_Click);
            }            

            appBarSettingsButton = new ApplicationBarIconButton(new Uri("Icons/feature.settings.png", UriKind.Relative));
            appBarSettingsButton.Text = "Settings";
            appBarSettingsButton.Click += new System.EventHandler(this.settingsButton_Click);

            appBarSendMailButton = new ApplicationBarIconButton(new Uri("Icons/feature.email.png", UriKind.Relative));
            appBarSendMailButton.Text = "Send Feedback";
            appBarSendMailButton.Click += new System.EventHandler(this.sendFeedback_Click);

            appBarRefreshButton = new ApplicationBarIconButton(new Uri("Icons/refresh.png", UriKind.Relative));
            appBarRefreshButton.Text = "Reload";
            appBarRefreshButton.Click += new System.EventHandler(this.refreshButton_Click);

            appBarFilterButton = new ApplicationBarIconButton(new Uri("Icons/search.refine.png", UriKind.Relative));
            appBarFilterButton.Text = "Filter";
            appBarFilterButton.Click += new System.EventHandler(this.filterButton_Click);

            appBarAddWorktaskButton = new ApplicationBarIconButton(new Uri("Icons/add.png", UriKind.Relative));
            appBarAddWorktaskButton.Text = "New Worktask";
            appBarAddWorktaskButton.Click += new System.EventHandler(this.newWorktask_Click);

        }

        private void sendFeedback_Click(object sender, EventArgs e)
        {
            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            MessageBoxResult result = MessageBox.Show("Make this app better. Send me feedback or report bugs, so I can fix or improve the app. Do you wanna send an email?", "", buttons);

            if (result == MessageBoxResult.OK)
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask();

                emailComposeTask.Subject = "fix or feature request TimecardApp";
                emailComposeTask.Body = "in case of reporting a bug to me: Describe as many details as possible.";
                emailComposeTask.To = AppResources.MailDev;

                emailComposeTask.Show();

                // Put the focus back to the main page.
                this.Focus();
            }
        }

        private void backupButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/BackupPage.xaml", UriKind.Relative));
        }

        private void timelogButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/TimelogPage.xaml", UriKind.Relative));
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            App.AppViewModel.SaveChangesToDB();
            App.AppViewModel.LoadOpenTimecardCollectionFromDatabase();
        }

        private void newWorktask_Click(object sender, EventArgs e)
        {
            if (App.AppViewModel.getTimecardForDate(DateTime.Today.Date) == null)
                MessageBox.Show("There is no timecard for this date. Create one first for today.");
            else
                NavigationService.Navigate(new Uri("/View/WorktaskPage.xaml?worktaskIDParam=" + System.Guid.NewGuid().ToString(), UriKind.Relative));
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/FilterPage.xaml?filterIDParam=", UriKind.Relative));
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/SettingPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            App.AppViewModel.SaveChangesToDB();
            // Call the base method.
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            App.AppViewModel.LoadCurrentTimecardsFromDatabase();
            // Call the base method, to execute the rest of the navigation event
            base.OnNavigatedTo(e);
        }

        private void MainPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.AppViewModel.SaveChangesToDB();
            ApplicationBar = new ApplicationBar();
            ApplicationBar.MenuItems.Add(appBarMenuBackup);
            if (App.AppViewModel.UsingTimelogInterface)
                ApplicationBar.MenuItems.Add(appBarMenuTimelog);
            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    ApplicationBar.Buttons.Add(appBarSettingsButton);
                    ApplicationBar.Buttons.Add(appBarAddWorktaskButton);
                    ApplicationBar.Buttons.Add(appBarSendMailButton);
                    if (App.AppViewModel.UsingTimelogInterface)
                        ApplicationBar.Buttons.Add(appBarTimelogButton);
                    break;

                case 1:
                    
                    ApplicationBar.Buttons.Add(appBarSettingsButton);
                    ApplicationBar.Buttons.Add(appBarRefreshButton);
                    App.AppViewModel.LoadOpenTimecardCollectionFromDatabase();
                    break;

                case 2:
                    ApplicationBar.Buttons.Add(appBarSettingsButton);
                    ApplicationBar.Buttons.Add(appBarFilterButton);
                    App.AppViewModel.LoadFilterTimecardCollectionFromDatabase(null);
                    break;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // diese Funktion bewirkt, dass die App ein Event bekommt,
        // dass sich eine Eigenschaft (wie zum Beispiel die Collection mit den Timecards) sich geändert hat
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


    }
}