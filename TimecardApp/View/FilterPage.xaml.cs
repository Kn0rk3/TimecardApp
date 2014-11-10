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
    public partial class FilterPage : PhoneApplicationPage
    {
        private ApplicationBarIconButton appBarHomeButton;
        private ApplicationBarIconButton appBarSaveButton;

        private FilterViewModel filterViewModel;

        public FilterPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
            ApplicationBar = new ApplicationBar();

            appBarHomeButton = new ApplicationBarIconButton(new Uri("Icons/map.neighborhood.png", UriKind.Relative));
            appBarHomeButton.Text = "Home";
            appBarHomeButton.Click += new System.EventHandler(this.homeButton_Click);
            ApplicationBar.Buttons.Add(appBarHomeButton);
            

            appBarSaveButton = new ApplicationBarIconButton(new Uri("Icons/save.png", UriKind.Relative));
            appBarSaveButton.Text = "Save";
            appBarSaveButton.Click += new System.EventHandler(this.saveButton_Click);
            ApplicationBar.Buttons.Add(appBarSaveButton);
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            String filterIDParameter = "";

            if (NavigationContext.QueryString.TryGetValue("filterIDParam", out filterIDParameter))
            {
                filterViewModel = App.AppViewModel.GetFilterViewModel(filterIDParameter);
            }
            this.DataContext = filterViewModel;

            if (filterViewModel.FilterMode == "both")
                bothRadioButton.IsChecked = true;
            else if (filterViewModel.FilterMode == "string")
                stringRadioButton.IsChecked = true;
            else if (filterViewModel.FilterMode == "date")
                dateRadioButton.IsChecked = true;

            // Call the base method, to execute the rest of the navigation event
            base.OnNavigatedTo(e);
        }

        private void objectPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            filterViewModel.saveThisFilter();
            this.NavigationService.GoBack();
        }

        private void RadioButtonBoth_Checked(object sender, RoutedEventArgs e)
        {
            filterViewModel.FilterMode = "both";
        }

        private void RadioButtonDate_Checked(object sender, RoutedEventArgs e)
        {
            filterViewModel.FilterMode = "date";
        }

        private void RadioButtonString_Checked(object sender, RoutedEventArgs e)
        {
            filterViewModel.FilterMode = "string";
        }

    }
}