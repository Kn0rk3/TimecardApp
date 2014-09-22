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
using TimecardApp.ViewModel;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.Live;
using System.Windows.Data;

namespace TimecardApp.View
{
    public partial class SettingPage : PhoneApplicationPage
    {
        private ApplicationBarIconButton appBarHomeButton;
        private ApplicationBarIconButton appBarAddButton;
        private ApplicationBarIconButton appBarSaveButton;
        private ApplicationBarIconButton appBarCancelButton;

        private SettingViewModel settingViewModel;

        public SettingPage()
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

            appBarAddButton = new ApplicationBarIconButton(new Uri("Icons/add.png", UriKind.Relative));
            appBarAddButton.Click += new System.EventHandler(this.newProjectOrCustomerButton_Click);

            appBarCancelButton = new ApplicationBarIconButton(new Uri("Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = "Cancel";
            appBarCancelButton.Click += new System.EventHandler(this.cancelButton_Click);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            App.AppViewModel.DiscardSettingViewModel();
            NavigationService.Navigate(new Uri("/View/SettingPage.xaml?item=2", UriKind.Relative));
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            settingViewModel.saveThisSetting();
            NavigationService.Navigate(new Uri("/View/SettingPage.xaml?item=2", UriKind.Relative));
        }

        private void newProjectOrCustomerButton_Click(object sender, EventArgs e)
        {
            if (this.SettingPagePivot.SelectedIndex == 1)
            {
                NavigationService.Navigate(new Uri("/View/CustomerPage.xaml?customerIDParam=" + System.Guid.NewGuid().ToString(), UriKind.Relative));
            }
            else if (this.SettingPagePivot.SelectedIndex == 0)
            {
                NavigationService.Navigate(new Uri("/View/ProjectPage.xaml?projectIDParam=" + System.Guid.NewGuid().ToString(), UriKind.Relative));
            }
        }

        private void SettingPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    appBarAddButton.Text = "New Project";
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    ApplicationBar.Buttons.Add(appBarAddButton);
                    break;

                case 1:
                    appBarAddButton.Text = "New Customer";
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    ApplicationBar.Buttons.Add(appBarAddButton);
                    break;

                case 2:
                    ApplicationBar = new ApplicationBar();
                    ApplicationBar.Buttons.Add(appBarHomeButton);
                    ApplicationBar.Buttons.Add(appBarSaveButton);
                    ApplicationBar.Buttons.Add(appBarCancelButton);
                    break;
            }
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        private void changeCustomerButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                Customer customer = button.DataContext as Customer;

                NavigationService.Navigate(new Uri("/View/CustomerPage.xaml?customerIDParam=" + customer.CustomerID, UriKind.Relative));
            }
        }

        private void changeProjectButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                Project project = button.DataContext as Project;

                NavigationService.Navigate(new Uri("/View/ProjectPage.xaml?projectIDParam=" + project.ProjectID, UriKind.Relative));
            }
        }

        private void deleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                // Get a handle for the to-do item bound to the button.
                Customer customer = button.DataContext as Customer;

                MessageBoxButton buttons = MessageBoxButton.OKCancel;
                MessageBoxResult result = MessageBox.Show("Are you sure to delete the customer " + customer.CustomerName + "?", "", buttons);

                if (result == MessageBoxResult.OK)
                {
                    App.AppViewModel.deleteCustomer(customer);

                    // Put the focus back to the main page.
                    this.Focus();
                }

            }
        }

        private void deleteProjectButton_Click(object sender, RoutedEventArgs e)
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            settingViewModel = App.AppViewModel.GetSettingViewModel();

            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var index = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(index);
                SettingPagePivot.SelectedIndex = indexParsed;
            }

            this.DataContext = settingViewModel;
            //this.appBarMenuOneDriveLogin.IsEnabled = SetBinding()

            base.OnNavigatedTo(e);
        }
    }
}