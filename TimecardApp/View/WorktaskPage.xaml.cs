﻿using System;
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
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.View
{
    public partial class WorktaskPage : PhoneApplicationPage
    {
        private string helperStringLogin = "-Help- Assign a Timelog task to a worktask: " +
            "\n\nYou can only assign a Timelog task for which the end and start date fits to the corresponding date of the worktask." +
            "\n\nAn additional upload will overwrite the already logged time in Timelog. Better check the results afterwards.";

        private WorktaskViewModel workTaskViewModel;
        private ApplicationBarIconButton appBarDeleteButton;
        private ApplicationBarIconButton appBarSaveButton;
        private ApplicationBarIconButton appBarCancelButton;
        private ApplicationBarIconButton appBarHelpButton;

        // every time the worktaskpage is created, create a new ViewModel for the Worktask 
        public WorktaskPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            appBarSaveButton = new ApplicationBarIconButton(new Uri("Icons/save.png", UriKind.Relative));
            appBarSaveButton.Text = "Save";
            appBarSaveButton.Click += new System.EventHandler(this.saveButton_Click);
            ApplicationBar.Buttons.Add(appBarSaveButton);

            appBarCancelButton = new ApplicationBarIconButton(new Uri("Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = "Discard";
            appBarCancelButton.Click += new System.EventHandler(this.discardButton_Click);
            ApplicationBar.Buttons.Add(appBarCancelButton);

            appBarDeleteButton = new ApplicationBarIconButton(new Uri("Toolkit.Content/ApplicationBar.Delete.png", UriKind.Relative));
            appBarDeleteButton.Text = "Delete";
            appBarDeleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            ApplicationBar.Buttons.Add(appBarDeleteButton);

            appBarHelpButton = new ApplicationBarIconButton(new Uri("Icons/questionmark.png", UriKind.Relative));
            appBarHelpButton.Text = "Help";
            appBarHelpButton.Click += new System.EventHandler(this.appBarHelperTimelog_Click);
            ApplicationBar.Buttons.Add(appBarHelpButton);

        }

        private void appBarHelperTimelog_Click(object sender, EventArgs e)
        {
            MessageBox.Show(helperStringLogin);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {

            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            MessageBoxResult result = MessageBox.Show("Are you sure to delete this worktask" + workTaskViewModel.WorktaskPageIdent + "?", "", buttons);

            if (result == MessageBoxResult.OK)
            {
                workTaskViewModel.DeleteThisWorktask();
                NavigationService.Navigate(new Uri("/View/TimecardPage.xaml?timecardIDParam=" + workTaskViewModel.WorktaskPageTimecard.TimecardID, UriKind.Relative));
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string worktaskIDParam = "";

            if (NavigationContext.QueryString.TryGetValue("worktaskIDParam", out worktaskIDParam))
            {
                String projectID = "";
                String dayDate = "";
                String timecardID = "";
                bool result;
                result = NavigationContext.QueryString.TryGetValue("projectID", out projectID);
                result = NavigationContext.QueryString.TryGetValue("timecardID", out timecardID);
                result = NavigationContext.QueryString.TryGetValue("dayDate", out dayDate);
                
                DateTime dayDateTime;
                bool dateConvertResult = DateTime.TryParse(dayDate, out dayDateTime);

                workTaskViewModel = App.AppViewModel.GetWorkTaskViewModel(worktaskIDParam, projectID, timecardID, dayDateTime);
                appBarDeleteButton.IsEnabled = workTaskViewModel.WorktaskPageEnabled;
                appBarSaveButton.IsEnabled = workTaskViewModel.WorktaskPageEnabled;
            }

            this.DataContext = workTaskViewModel;

            // Call the base method, to execute the rest of the navigation event
            base.OnNavigatedTo(e);

        }

        private void workDescriptionTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (String.Equals(workTaskViewModel.WorktaskPageWorkDescription, AppResources.ExampleTaskDescription))
            {
                // Clear the text box when it gets focus and only if there is still the standard text
                workTaskViewModel.WorktaskPageWorkDescription = "";
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //workTaskViewModel.WorktaskPageWorkDescription = workDescriptionTextBox.Text;
            HelperClass.FocusedTextBoxUpdateSource();
            workTaskViewModel.SaveThisWorkTask();
            NavigationService.Navigate(new Uri("/View/TimecardPage.xaml?timecardIDParam=" + workTaskViewModel.WorktaskPageTimecard.TimecardID, UriKind.Relative));
        }

        private void discardButton_Click(object sender, EventArgs e)
        {
            App.AppViewModel.DiscardWorktaskViewModel();
            NavigationService.Navigate(new Uri("/View/TimecardPage.xaml?timecardIDParam=" + workTaskViewModel.WorktaskPageTimecard.TimecardID, UriKind.Relative));
        }

        private void newProjectButton_Click(object sender, EventArgs e)
        {
            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            MessageBoxResult result = MessageBox.Show("Do you want to save the current worktask?", "", buttons);

            if (result == MessageBoxResult.OK)
            {
                workTaskViewModel.WorktaskPageWorkDescription = workDescriptionTextBox.Text;
                workTaskViewModel.SaveThisWorkTask();
            }
            else
            {
                App.AppViewModel.DiscardWorktaskViewModel();
            }

            NavigationService.Navigate(new Uri("/View/ProjectPage.xaml?projectIDParam=" + System.Guid.NewGuid().ToString(), UriKind.Relative));
        }


        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            App.AppViewModel.DiscardWorktaskViewModel();
            base.OnBackKeyPress(e);
        }

        private void ResetForTimelogButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            MessageBoxResult result = MessageBox.Show("Do you want to mark this worktask an additional upload into timelog angain?", "", buttons);

            if (result == MessageBoxResult.OK)
            {
                workTaskViewModel.LastTimelogRegistration = "";
            }
        }
    }
}