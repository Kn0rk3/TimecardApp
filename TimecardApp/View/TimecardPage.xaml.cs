﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using TimecardApp.Resources;
using TimecardApp.Model;
using System.Collections.ObjectModel;
using TimecardApp.ViewModel;
using System.Windows.Data;
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.View
{
    public partial class TimecardPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private TimecardViewModel timecardViewModel;
        
        public TimecardPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }


        private void BuildLocalizedApplicationBar()
        {
            // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarHomeButton = new ApplicationBarIconButton(new Uri("Icons/map.neighborhood.png", UriKind.Relative));
            appBarHomeButton.Text = "Home";
            appBarHomeButton.Click += new System.EventHandler(this.homeButton_Click);
            ApplicationBar.Buttons.Add(appBarHomeButton);

            //ApplicationBarIconButton appBarSettingsButton = new ApplicationBarIconButton(new Uri("Icons/feature.settings.png", UriKind.Relative));
            //appBarSettingsButton.Text = "Settings";
            //appBarSettingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            //ApplicationBar.Buttons.Add(appBarSettingsButton);

            ApplicationBarIconButton appBarNewWorkTaskButton = new ApplicationBarIconButton(new Uri("Icons/add.png", UriKind.Relative));
            appBarNewWorkTaskButton.Text = "New Task";
            appBarNewWorkTaskButton.Click += new System.EventHandler(this.newWorkTaskButton_Click);
            ApplicationBar.Buttons.Add(appBarNewWorkTaskButton);

            ApplicationBarIconButton appBarCopyTimecardButton = new ApplicationBarIconButton(new Uri("Icons/tabs.2.png", UriKind.Relative));
            appBarCopyTimecardButton.Text = "Copy Timecard";
            appBarCopyTimecardButton.Click += new System.EventHandler(this.copyTimecardButton_Click);
            ApplicationBar.Buttons.Add(appBarCopyTimecardButton);
            if (App.AppViewModel.UsingTimelogInterface)
            {
                ApplicationBarIconButton appBarTimelogButton = new ApplicationBarIconButton(new Uri("Icons/feature.alarm.png", UriKind.Relative));
                appBarTimelogButton.Text = "Timelog";
                appBarTimelogButton.Click += new System.EventHandler(this.timelogButton_Click);
                ApplicationBar.Buttons.Add(appBarTimelogButton);
            }

        }

        private void copyTimecardButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/CopyTimecardPage.xaml?copyTimecardID=" + timecardViewModel.TimecardID, UriKind.Relative));
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/SettingPage.xaml?item=0", UriKind.Relative));
        }

        private void newWorkTaskButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/WorktaskPage.xaml?worktaskIDParam=" + System.Guid.NewGuid().ToString() + "&timecardID=" + timecardViewModel.TimecardID + "&dayDate=" + timecardViewModel.CurrentShownDate, UriKind.Relative));
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string timecardIDParameter = "";
            
            if (NavigationContext.QueryString.TryGetValue("timecardIDParam", out timecardIDParameter))
            {
                timecardViewModel = App.AppViewModel.GetTimecardViewModel(timecardIDParameter);
                timecardViewModel.ReloadWorktasks();
                timecardViewModel.CalculateTimeForProjects();
            }

            this.DataContext = timecardViewModel;

            // Call the base method, to execute the rest of the navigation event
            base.OnNavigatedTo(e);
        }


        private void editWorktaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                WorkTask worktask = button.DataContext as WorkTask;

                // hier muss noch die ID mitgegeben werden, dass die Timecard korrekt geladen werden kann
                NavigationService.Navigate(new Uri("/View/WorktaskPage.xaml?worktaskIDParam=" + worktask.WorkTaskID , UriKind.Relative));
            }
        }

        private void timelogButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/TimelogPage.xaml", UriKind.Relative));
        }

        private void TimecardPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    timecardViewModel.CurrentShownDate = timecardViewModel.TimecardStartDate.Date;
                    break;

                case 1:
                    //Monday
                    timecardViewModel.CurrentShownDate = timecardViewModel.TimecardStartDate.Date;
                    break;

                case 2:
                    //Tuesday
                    timecardViewModel.CurrentShownDate = timecardViewModel.TimecardStartDate.AddDays(1).Date;
                    break;

                case 3:
                    //Wednesday
                    timecardViewModel.CurrentShownDate = timecardViewModel.TimecardStartDate.AddDays(2).Date;
                    break;

                case 4:
                    //Thursday
                    timecardViewModel.CurrentShownDate = timecardViewModel.TimecardStartDate.AddDays(3).Date;
                    break;

                case 5:
                    //Friday
                    timecardViewModel.CurrentShownDate = timecardViewModel.TimecardStartDate.AddDays(4).Date;
                    break;

            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            timecardViewModel.saveThisTimecard();
            // Call the base method.
            base.OnNavigatedFrom(e);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // diese Funktion bewirkt, dass die App ein Event bekommt,
        // dass sich eine Eigenschaft sich geändert hat
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