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
using TimecardApp.Model;
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.View
{
    public partial class TimelogPage : PhoneApplicationPage
    {
        private TimelogViewModel timelogViewModel;

        private ApplicationBarIconButton appBarHomeButton;
        private ApplicationBarIconButton appBarSaveButton;

        public TimelogPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
            appBarHomeButton = new ApplicationBarIconButton(new Uri("Icons/map.neighborhood.png", UriKind.Relative));
            appBarHomeButton.Text = "Home";
            appBarHomeButton.Click += new System.EventHandler(this.homeButton_Click);


            appBarSaveButton = new ApplicationBarIconButton(new Uri("Icons/save.png", UriKind.Relative));
            appBarSaveButton.Text = "Save";
            appBarSaveButton.Click += new System.EventHandler(this.saveButton_Click);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            timelogViewModel.SaveThisTlSetting();
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            App.AppViewModel.DiscardTlSettingViewModel();
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            timelogViewModel.TL_loginToTimelog();
        }

        private void loadDataButton_Click(object sender, EventArgs e)
        {
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timelogViewModel = App.AppViewModel.GetTimelogViewModel();
            
            this.DataContext = timelogViewModel;
            base.OnNavigatedTo(e);
        }

        private void TlSettingPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    ApplicationBar.Buttons.Add(appBarSaveButton);
                    break;
                
            }
        }
    }
}