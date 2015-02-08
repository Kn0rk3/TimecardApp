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
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.View
{
    public partial class CopyTimecardPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private DateTime selectDate;
        public DateTime SelectDate
        {
            get { return selectDate; }
            set
            {
                selectDate = value;
                App.AppViewModel.TimecardName = "CW " + HelperClass.NumberOfWeek(value).ToString() + " " + value.Year;
                App.AppViewModel.SetWorktaskPreviewCopyTimecard(HelperClass.GetFirstDayOfWeek(value));
            }
        }


        public CopyTimecardPage()
        {
            SelectDate = DateTime.Now;
            DataContext = App.AppViewModel;
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarSaveButton = new ApplicationBarIconButton(new Uri("Icons/save.png", UriKind.Relative));
            appBarSaveButton.Text = "Save";
            appBarSaveButton.Click += new System.EventHandler(this.saveButton_Click);
            ApplicationBar.Buttons.Add(appBarSaveButton);

            ApplicationBarIconButton appBarCancelButton = new ApplicationBarIconButton(new Uri("Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            appBarCancelButton.Text = "Discard";
            appBarCancelButton.Click += new System.EventHandler(this.discardButton_Click);
            ApplicationBar.Buttons.Add(appBarCancelButton);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            HelperClass.FocusedTextBoxUpdateSource();
            String newGUID = System.Guid.NewGuid().ToString();
            Timecard newTimecard = new Timecard() { TimecardID = newGUID, StartDate = HelperClass.GetFirstDayOfWeek(selectDate) };
            bool succeeded = App.AppViewModel.SaveCopiedTimecard(newTimecard);
            if (succeeded) 
                NavigationService.Navigate(new Uri("/View/TimecardPage.xaml?timecardIDParam=" + newGUID, UriKind.Relative));
        }

        private void discardButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            SelectDate = (DateTime)e.NewDateTime;
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