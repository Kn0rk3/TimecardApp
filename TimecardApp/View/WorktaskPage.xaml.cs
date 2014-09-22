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

namespace TimecardApp.View
{
    public partial class WorktaskPage : PhoneApplicationPage
    {
        private WorktaskViewModel workTaskViewModel;
        private ApplicationBarIconButton appBarDeleteButton;
        private ApplicationBarIconButton appBarSaveButton;

        // every time the worktaskpage is created, create a new ViewModel for the Worktask 
        public WorktaskPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
            ApplicationBar = new ApplicationBar();

            appBarSaveButton = new ApplicationBarIconButton(new Uri("Icons/save.png", UriKind.Relative));
            appBarSaveButton.Text = "Save";
            appBarSaveButton.Click += new System.EventHandler(this.saveButton_Click);
            ApplicationBar.Buttons.Add(appBarSaveButton);

            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = "Discard";
            appBarCancelButton.Click += new System.EventHandler(this.discardButton_Click);
            ApplicationBar.Buttons.Add(appBarCancelButton);

            appBarDeleteButton = new ApplicationBarIconButton(new Uri("Toolkit.Content/ApplicationBar.Delete.png", UriKind.Relative));
            appBarDeleteButton.Text = "Delete";
            appBarDeleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            ApplicationBar.Buttons.Add(appBarDeleteButton);
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
                String timecardID = "";
                bool result;
                result = NavigationContext.QueryString.TryGetValue("projectID", out projectID);
                result = NavigationContext.QueryString.TryGetValue("timecardID", out timecardID);

                workTaskViewModel = App.AppViewModel.GetWorkTaskViewModel(worktaskIDParam, projectID, timecardID);
                appBarDeleteButton.IsEnabled = workTaskViewModel.WorktaskPageEnabled;
                appBarSaveButton.IsEnabled = workTaskViewModel.WorktaskPageEnabled;
            }

            this.DataContext = workTaskViewModel;

            // Call the base method, to execute the rest of the navigation event
            base.OnNavigatedTo(e);

        }

        private void workDescriptionTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (String.Equals(workTaskViewModel.WorktaskPageWorkDescription, "What have you done today?"))
            {
                // Clear the text box when it gets focus and only if there is still the standard text
                workTaskViewModel.WorktaskPageWorkDescription = "";
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            workTaskViewModel.WorktaskPageWorkDescription = workDescriptionTextBox.Text;
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

    }
}