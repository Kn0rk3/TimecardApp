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
using System.Collections.ObjectModel;
using TimecardApp.ViewModel;

namespace TimecardApp.View
{
    public partial class ProjectPage : PhoneApplicationPage, INotifyPropertyChanged
    {

        private ApplicationBarIconButton appBarHomeButton;
        private ApplicationBarIconButton appBarSaveButton;
        private ApplicationBarIconButton appBarCheckButton;
        private ApplicationBarIconButton appBarCancelButton;
        private ApplicationBarIconButton appBarAddButton;

        private ProjectViewModel projectViewModel;

        public ProjectPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
            //ApplicationBar = new ApplicationBar();

            appBarCancelButton = new ApplicationBarIconButton(new Uri("Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = "Cancel";
            appBarCancelButton.Click += new System.EventHandler(this.cancelButton_Click);

            appBarHomeButton = new ApplicationBarIconButton(new Uri("Icons/map.neighborhood.png", UriKind.Relative));
            appBarHomeButton.Text = "Home";
            appBarHomeButton.Click += new System.EventHandler(this.homeButton_Click);

            appBarCheckButton = new ApplicationBarIconButton(new Uri("Icons/check.png", UriKind.Relative));
            appBarCheckButton.Text = "Check";
            appBarCheckButton.Click += new System.EventHandler(this.checkButton_Click);

            appBarSaveButton = new ApplicationBarIconButton(new Uri("Icons/save.png", UriKind.Relative));
            appBarSaveButton.Text = "Save";
            appBarSaveButton.Click += new System.EventHandler(this.saveButton_Click);

            appBarAddButton = new ApplicationBarIconButton(new Uri("Icons/add.png", UriKind.Relative));
            appBarAddButton.Text = "New Worktask";
            appBarAddButton.Click += new System.EventHandler(this.newWorktaskButton_Click);
        }


        private void cancelButton_Click(object sender, EventArgs e)
        {
            App.AppViewModel.DiscardProjectViewModel();
            this.NavigationService.GoBack();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            projectViewModel.saveThisProject();
            this.NavigationService.GoBack();
        }

        private void newWorktaskButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/WorktaskPage.xaml?worktaskIDParam=" + System.Guid.NewGuid().ToString() + "&projectID=" + projectViewModel.ProjectID, UriKind.Relative));
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void projectShort_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (projectShort_Textbox.Text.Length <= 5)
            {
                ApplicationBar.Buttons.Remove(appBarCheckButton);
            }
            else
            {
                MessageBox.Show("The project short string cannot be longer than 5 signs.");
                projectShort_Textbox.Text = "";
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            String projectIDParameter = "";

            if (NavigationContext.QueryString.TryGetValue("projectIDParam", out projectIDParameter))
            {
                String customerID = "";
                if (NavigationContext.QueryString.TryGetValue("customerID", out customerID))
                    projectViewModel = App.AppViewModel.GetProjectViewModel(projectIDParameter, customerID);
                else
                    projectViewModel = App.AppViewModel.GetProjectViewModel(projectIDParameter, customerID);

                projectViewModel.LoadAndCalsInitial();
            }
            this.DataContext = projectViewModel;
            // Call the base method, to execute the rest of the navigation event
            base.OnNavigatedTo(e);
        }

        private void ProjectPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    ApplicationBar.Buttons.Add(appBarSaveButton);
                    ApplicationBar.Buttons.Add(appBarCancelButton);
                    break;

                case 1:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    break;

                case 2:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    ApplicationBar.Buttons.Add(appBarCancelButton);
                    break;
            }
        }

        private void editWorktaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                WorkTask worktask = button.DataContext as WorkTask;

                // hier muss noch die ID mitgegeben werden, dass die Timecard korrekt geladen werden kann
                NavigationService.Navigate(new Uri("/View/WorktaskPage.xaml?worktaskIDParam=" + worktask.WorkTaskID, UriKind.Relative));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
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