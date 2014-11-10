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
    public partial class CustomerPage : PhoneApplicationPage
    {
        private CustomerViewModel customerViewModel;

        private ApplicationBarIconButton appBarHomeButton;
        private ApplicationBarIconButton appBarSaveButton;
        private ApplicationBarIconButton appBarCheckButton;
        private ApplicationBarIconButton appBarCancelButton;
        private ApplicationBarIconButton appBarAddButton;

        public CustomerPage()
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

            appBarCheckButton = new ApplicationBarIconButton(new Uri("Icons/check.png", UriKind.Relative));
            appBarCheckButton.Text = "Check";
            appBarCheckButton.Click += new System.EventHandler(this.checkButton_Click);

            appBarAddButton = new ApplicationBarIconButton(new Uri("Icons/add.png", UriKind.Relative));
            appBarAddButton.Text = "New Project";
            appBarAddButton.Click += new System.EventHandler(this.newProjectButton_Click);

            appBarCancelButton = new ApplicationBarIconButton(new Uri("Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = "Cancel";
            appBarCancelButton.Click += new System.EventHandler(this.cancelButton_Click);
        }

        private void newProjectButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/ProjectPage.xaml?projectIDParam=" + System.Guid.NewGuid().ToString() + "&customerID=" + customerViewModel.CustomerID , UriKind.Relative));
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            App.AppViewModel.DiscardCustomerViewModel();
            this.NavigationService.GoBack();
            //NavigationService.Navigate(new Uri("/View/SettingPage.xaml?item=2", UriKind.Relative));
        }
        
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string customerIDParam = "";

            if (NavigationContext.QueryString.TryGetValue("customerIDParam", out customerIDParam))
            {
                customerViewModel = App.AppViewModel.GetCustomerViewModel(customerIDParam);
            }

            this.DataContext = customerViewModel;

            // Call the base method, to execute the rest of the navigation event
            base.OnNavigatedTo(e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            customerViewModel.saveThisCustomer();
            this.NavigationService.GoBack();
            //NavigationService.Navigate(new Uri("/View/SettingPage.xaml?item=1", UriKind.Relative));
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void newCustomerTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //ApplicationBar.Buttons.Remove(appBarSaveButton);
            ApplicationBar.Buttons.Add(appBarCheckButton);
        }

        private void newCustomerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //ApplicationBar.Buttons.Add(appBarSaveButton);
            ApplicationBar.Buttons.Remove(appBarCheckButton);
        }

        private void customerShort_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (customerShort_Textbox.Text.Length <= 3)
            {
                //ApplicationBar.Buttons.Add(appBarSaveButton);
                ApplicationBar.Buttons.Remove(appBarCheckButton);
            }
            else
            {
                MessageBox.Show("The customer short string cannot be longer than 3 signs.");
                customerShort_Textbox.Text = "";
            }
        }

        private void changeProject_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                Project project = button.DataContext as Project;

                // hier muss noch die ID mitgegeben werden, dass die Timecard korrekt geladen werden kann
                NavigationService.Navigate(new Uri("/View/ProjectPage.xaml?projectIDParam=" + project.ProjectID, UriKind.Relative));
            }
        }

        private void deleteProject_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                Project project = button.DataContext as Project;
                
                MessageBoxButton buttons = MessageBoxButton.OKCancel;
                MessageBoxResult result = MessageBox.Show("Are you sure to delete the project " + project.Ident_Project + "?", "", buttons);

                if (result == MessageBoxResult.OK)
                {
                    App.AppViewModel.deleteProject(project);

                    // Put the focus back to the main page.
                    this.Focus();
                }
            }      
        }

        private void CustomerPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                    ApplicationBar.Buttons.Add(appBarAddButton);
                    ApplicationBar.Buttons.Add(appBarCancelButton);
                    break;

            }
        }
    }
}