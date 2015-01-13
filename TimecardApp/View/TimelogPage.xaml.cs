﻿using System;
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
        private TimelogViewModel timelogViewModel;
        private ApplicationBarIconButton appBarHomeButton;
        private ApplicationBarIconButton appBarSyncData;
        private ApplicationBarIconButton appBarEdit;

        public TimelogPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            appBarHomeButton = new ApplicationBarIconButton(new Uri("Icons/map.neighborhood.png", UriKind.Relative));
            appBarHomeButton.Text = "Home";
            appBarHomeButton.Click += new System.EventHandler(this.homeButton_Click);

            appBarSyncData = new ApplicationBarIconButton(new Uri("Icons/refresh.png", UriKind.Relative));
            appBarSyncData.Text = "Load Tl tasks";
            appBarSyncData.Click += new System.EventHandler(this.synchronisationData_Click);

            appBarEdit = new ApplicationBarIconButton(new Uri("Resources/edit.png", UriKind.Relative));
            appBarEdit.Text = "Edit";
            appBarEdit.Click += new System.EventHandler(this.editTimelogSetting_Click);

        }



        private void TimelogPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    break;
                case 1:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    ApplicationBar.Buttons.Add(appBarEdit);
                    ApplicationBar.Buttons.Add(appBarSyncData);
                    break;
            }
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

        private void homeButton_Click(object sender, EventArgs e)
        {
            if (timelogViewModel.CurrentState != ETimelogState.Running)
            {
                timelogViewModel.SaveThisTlSetting();
                App.AppViewModel.DiscardTlSettingViewModel();
                NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
            }
        }

        private void synchronisationData_Click(object sender, EventArgs e)
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
    }
}