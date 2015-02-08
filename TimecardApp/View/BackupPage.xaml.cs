using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TimecardApp.Model;
using System.ComponentModel;
using TimecardApp.ViewModel;
using System.Windows.Data;
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.View
{
    public partial class BackupPage : PhoneApplicationPage, INotifyPropertyChanged
    {

        private BackupViewModel backupViewModel;

        public BackupPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            this.LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private void BuildLocalizedApplicationBar()
        {
            // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarHomeButton = new ApplicationBarIconButton(new Uri("Icons/map.neighborhood.png", UriKind.Relative));
            appBarHomeButton.Text = "Home";
            appBarHomeButton.Click += new System.EventHandler(this.homeButton_Click);
            ApplicationBar.Buttons.Add(appBarHomeButton);
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            App.AppViewModel.ConnectDB();
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            this.LoadingPanel.Visibility = Visibility.Visible;
            await backupViewModel.LoginOneDrive();
            this.LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            this.LoadingPanel.Visibility = Visibility.Visible;
            backupViewModel.LogoutOneDrive();
            this.LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private async void fileRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoadingPanel.Visibility = Visibility.Visible;
            var button = sender as Button;

            if (button != null)
            {
                Backupfile file = button.DataContext as Backupfile;

                MessageBoxButton buttons = MessageBoxButton.OKCancel;
                MessageBoxResult result = MessageBox.Show("Are you sure to restore the database with the backupfile " + file.Filename
                    + " ? All data in the recent database will be lossed. Best backup them before restore.", "", buttons);

                if (result == MessageBoxResult.OK)
                {
                    MessageBox.Show("Don't close the app nor navigate back or to the home screen. This can cause data loss.");
                    await backupViewModel.RestoreBackupfile(file);
                    // Put the focus back to the main page.
                    this.Focus();
                }
            }
            this.LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private async void backupButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoadingPanel.Visibility = Visibility.Visible;
            HelperClass.FocusedTextBoxUpdateSource();
            await backupViewModel.BackupLocalDatabase();
            this.LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private async void reloadFilesButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoadingPanel.Visibility = Visibility.Visible;
            await backupViewModel.ReloadAllBackupfiles();
            this.LoadingPanel.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            backupViewModel = App.AppViewModel.GetBackupViewModel();

            this.DataContext = backupViewModel;
            //this.appBarMenuOneDriveLogin.IsEnabled = SetBinding()

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!backupViewModel.RestoreLock)
                // Call the base method.
                base.OnNavigatedFrom(e);
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