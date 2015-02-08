using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TimecardApp.Model;
using TimecardApp.Model.NonPersistent;
using TimecardApp.ViewModel;
using Microsoft.Phone.Data.Linq;
using System.IO.IsolatedStorage;
using TimecardApp.Resources;
using System.Linq;

namespace TimecardApp
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        // The static ViewModel, to be used across the application.
        private static AppViewModel appViewModel;
        public static AppViewModel AppViewModel
        {
            get { return appViewModel; }
        }

        // The current version of the application.
        //version 2: New object TimelogTask, TimelogProject (from Timelog), marking projects and customers as Timelog objects and worktask now refers to phase from timelog
        public static int DB_VERSION = 2;
        
        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            // Specify the local database connection string.
            string DBConnectionString = "Data Source=isostore:/" + AppResources.DatabaseName + ".sdf";
            // old database name : DellApp.sdf
            string oldDatabasename = "DellApp.sdf";

            // Create the database if it does not exist.
            using (DBClass db = new DBClass(DBConnectionString))
            {
                if (db.DatabaseExists() == false)
                {
                    string oldConnectionString = "Data Source=isostore:/" + oldDatabasename ;
                    using (DBClass oldDB = new DBClass(oldConnectionString))
                    {
                        if (oldDB.DatabaseExists() == true)
                        {
                            // in case old database exists use this
                            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
                            iso.CopyFile(oldDatabasename , AppResources.DatabaseName + ".sdf");
                        }
                        else
                        {
                            //Create the database
                            db.CreateDatabase();
                            db.DeferredLoadingEnabled = true;
#if DEBUG
                            fillDummyDataInDatabase(db);
#else
                    fillTutorialDataInDatabase(db);
#endif

                            DatabaseSchemaUpdater dbNewUpdater = db.CreateDatabaseSchemaUpdater();
                            dbNewUpdater.DatabaseSchemaVersion = DB_VERSION;
                            dbNewUpdater.Execute();
                        }
                    }
                }

                using (DBMigrator migrator = new DBMigrator(DBConnectionString , DB_VERSION ))
                {
                    try
                    {
                        if (migrator.hasToMigrate())
                        {
                            migrator.MigrateDatabase();
                        }
                    }
                    catch
                    {
                        throw new Exception("Database error in Timecard App: Connectionstring doesn't point to a existing database. Migration failed.");
                    }
                }
            }

            // Create the ViewModel object.
            appViewModel = new AppViewModel(DBConnectionString);
        }

        private void fillDummyDataInDatabase(DBClass dellAppDB)
        {
            Customer firstCustomer = new Customer() { CustomerID = System.Guid.NewGuid().ToString(), CustomerName = "Krones AG", CustomerShort = "KRO" };
            Customer secondCustomer = new Customer() { CustomerID = System.Guid.NewGuid().ToString(), CustomerName = "MunichRe", CustomerShort = "MR" };
            Customer thirdCustomer = new Customer() { CustomerID = System.Guid.NewGuid().ToString(), CustomerName = "Miltenyi Biotec", CustomerShort = "MB" };
            Customer forthCustomer = new Customer() { CustomerID = System.Guid.NewGuid().ToString(), CustomerName = "Paul Hartmann", CustomerShort = "PH" };
            Customer fifthCustomer = new Customer() { CustomerID = System.Guid.NewGuid().ToString(), CustomerName = "Bell", CustomerShort = "BL" };
            Customer sixthCustomer = new Customer() { CustomerID = System.Guid.NewGuid().ToString(), CustomerName = "Wüstenrot", CustomerShort = "WW" };
            
            dellAppDB.Customer.InsertOnSubmit(firstCustomer);
            dellAppDB.Customer.InsertOnSubmit(secondCustomer);
            dellAppDB.Customer.InsertOnSubmit(thirdCustomer);
            dellAppDB.Customer.InsertOnSubmit(forthCustomer);
            dellAppDB.Customer.InsertOnSubmit(fifthCustomer);
            dellAppDB.Customer.InsertOnSubmit(sixthCustomer);

            dellAppDB.SubmitChanges();

            Project firstProject = new Project() { ProjectID = System.Guid.NewGuid().ToString(), CustomerID = firstCustomer.CustomerID, ProjectCode = "123456M", ProjectShort = "IAM", ProjectName = "IAM Projekt", ProjectDescription = "Implementierung und Konzeption des IAM Projekts", Ident_Project = firstCustomer.CustomerShort + "-IAM" };
            Project secondProject = new Project() { ProjectID = System.Guid.NewGuid().ToString(), CustomerID = secondCustomer.CustomerID, ProjectCode = "223457M", ProjectShort = "IAM", ProjectName = "IAM Projekt", ProjectDescription = "Implementierung und Konzeption des IAM Projekts", Ident_Project = secondCustomer.CustomerShort + "-IAM" };
            Project thirdProject = new Project() { ProjectID = System.Guid.NewGuid().ToString(), CustomerID = firstCustomer.CustomerID, ProjectCode = "123457M", ProjectShort = "REISE", ProjectName = "Reisezeit", ProjectDescription = "Reisezeit für das Projekt", Ident_Project = firstCustomer.CustomerShort + "-REI" };
            Project forthProject = new Project() { ProjectID = System.Guid.NewGuid().ToString(), CustomerID = secondCustomer.CustomerID, ProjectCode = "223456M", ProjectShort = "REISE", ProjectName = "Reisezeit", ProjectDescription = "Reisezeit für das Projekt", Ident_Project = secondCustomer.CustomerShort + "-REI" };
            Project fifthProject = new Project() { ProjectID = System.Guid.NewGuid().ToString(), CustomerID = thirdCustomer.CustomerID, ProjectCode = "323456M", ProjectShort = "IAM", ProjectName = "IAM Projekt", ProjectDescription = "Implementierung und Konzeption des IAM Projekts", Ident_Project = thirdCustomer.CustomerShort + "-IAM" };
            Project sixthProject = new Project() { ProjectID = System.Guid.NewGuid().ToString(), CustomerID = thirdCustomer.CustomerID, ProjectCode = "323457M", ProjectShort = "REI", ProjectName = "Reisezeit", ProjectDescription = "Reisezeit für das Projekt", Ident_Project = thirdCustomer.CustomerShort + "-REI" };


            Timecard firstTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 10", StartDate = new DateTime(2014, 3, 3) };
            Timecard secondTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 09", StartDate = new DateTime(2014, 3, 3).AddDays(-7) };
            Timecard thirdTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 08", StartDate = new DateTime(2014, 3, 3).AddDays(-14) };
            Timecard forthTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 07", StartDate = new DateTime(2014, 3, 3).AddDays(-21) };
            Timecard fifthTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 06", StartDate = new DateTime(2014, 3, 3).AddDays(-28) };
            Timecard sixthTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 05", StartDate = new DateTime(2014, 3, 3).AddDays(-35) };
            Timecard seventhTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 04", StartDate = new DateTime(2014, 3, 3).AddDays(-42) };
            Timecard eigthTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 03", StartDate = new DateTime(2014, 3, 3).AddDays(-49) };
            Timecard ningthTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "KW 21", StartDate = new DateTime(2014, 5, 19) };

            dellAppDB.Projects.InsertOnSubmit(firstProject);
            dellAppDB.Projects.InsertOnSubmit(secondProject);
            dellAppDB.Projects.InsertOnSubmit(thirdProject);
            dellAppDB.Projects.InsertOnSubmit(forthProject);
            dellAppDB.Projects.InsertOnSubmit(fifthProject);
            dellAppDB.Projects.InsertOnSubmit(sixthProject);

            dellAppDB.SubmitChanges();

            dellAppDB.Timecards.InsertOnSubmit(firstTimecard);
            dellAppDB.Timecards.InsertOnSubmit(secondTimecard);
            dellAppDB.Timecards.InsertOnSubmit(thirdTimecard);
            dellAppDB.Timecards.InsertOnSubmit(forthTimecard);
            dellAppDB.Timecards.InsertOnSubmit(fifthTimecard);
            dellAppDB.Timecards.InsertOnSubmit(sixthTimecard);
            dellAppDB.Timecards.InsertOnSubmit(seventhTimecard);
            dellAppDB.Timecards.InsertOnSubmit(eigthTimecard);
            dellAppDB.Timecards.InsertOnSubmit(ningthTimecard);

            dellAppDB.SubmitChanges();

            DEBUG_NewTimecard(dellAppDB, firstTimecard);
            DEBUG_NewTimecard(dellAppDB, secondTimecard);
            DEBUG_NewTimecard(dellAppDB, thirdTimecard);
            DEBUG_NewTimecard(dellAppDB, forthTimecard);
            DEBUG_NewTimecard(dellAppDB, fifthTimecard);
            DEBUG_NewTimecard(dellAppDB, sixthTimecard);
            DEBUG_NewTimecard(dellAppDB, seventhTimecard);
            DEBUG_NewTimecard(dellAppDB, eigthTimecard);
            DEBUG_NewTimecard(dellAppDB, ningthTimecard);

            TimelogTask firstTask = new TimelogTask() { 
                TimelogTaskUID = System.Guid.NewGuid().ToString(), 
                TimelogProjectID = 15, 
                TimelogProjectName = "Allianz", 
                TimelogTaskName = "Implementerung Q4",
                StartDate = new DateTime(2014,10,01), 
                EndDate = new DateTime(2014,12,31)};
            TimelogTask secondTask = new TimelogTask() { 
                TimelogTaskUID = System.Guid.NewGuid().ToString(), 
                TimelogProjectID = 15, 
                TimelogProjectName = "Allianz", 
                TimelogTaskName = "Implementerung Q3",
                StartDate = new DateTime(2014,07,01), 
                EndDate = new DateTime(2014,09,30)};
            TimelogTask thirdTask = new TimelogTask() { 
                TimelogTaskUID = System.Guid.NewGuid().ToString(), 
                TimelogProjectID = 16, 
                TimelogProjectName = "MunichRe",
                TimelogTaskName = "Implementerung",
                StartDate = new DateTime(2014, 01, 01),
                EndDate = new DateTime(2014, 09, 30)};
            TimelogTask forthTask = new TimelogTask()
            {
                TimelogTaskUID = System.Guid.NewGuid().ToString(),
                TimelogProjectID = 15,
                TimelogProjectName = "Allianz",
                TimelogTaskName = "Implementerung Q4-1",
                StartDate = new DateTime(2014, 10, 01),
                EndDate = new DateTime(2014, 12, 31)
            };
            TimelogTask fifthTask = new TimelogTask()
            {
                TimelogTaskUID = System.Guid.NewGuid().ToString(),
                TimelogProjectID = 15,
                TimelogProjectName = "Allianz",
                TimelogTaskName = "Implementerung Q4-2",
                StartDate = new DateTime(2014, 10, 01),
                EndDate = new DateTime(2014, 12, 31)
            };
            TimelogTask sixthTask = new TimelogTask()
            {
                TimelogTaskUID = System.Guid.NewGuid().ToString(),
                TimelogProjectID = 15,
                TimelogProjectName = "Allianz",
                TimelogTaskName = "Implementerung Q4-3",
                StartDate = new DateTime(2014, 10, 01),
                EndDate = new DateTime(2014, 12, 31)
            };

            dellAppDB.TimelogTasks.InsertOnSubmit(firstTask);
            dellAppDB.TimelogTasks.InsertOnSubmit(secondTask);
            dellAppDB.TimelogTasks.InsertOnSubmit(thirdTask);
            dellAppDB.TimelogTasks.InsertOnSubmit(forthTask);
            dellAppDB.TimelogTasks.InsertOnSubmit(fifthTask);
            dellAppDB.TimelogTasks.InsertOnSubmit(sixthTask);
            dellAppDB.SubmitChanges();
        
        }

        private void fillTutorialDataInDatabase(DBClass dellAppDB)
        {
            Customer tutorialCustomer = new Customer() { CustomerID = System.Guid.NewGuid().ToString(), CustomerName = "First Use Inc.", CustomerShort = "FUI" };
            dellAppDB.Customer.InsertOnSubmit(tutorialCustomer);

            dellAppDB.SubmitChanges();

            Project firstProject = new Project() { ProjectID = System.Guid.NewGuid().ToString(), CustomerID = tutorialCustomer.CustomerID, ProjectCode = "TUTORIAL_CODE", ProjectShort = "TUT", ProjectName = "Tutorial Projekt", ProjectDescription = "This is just for start of this app. Can be deleted.", Ident_Project = tutorialCustomer.CustomerShort + "-TUT" };
            Timecard firstTimecard = new Timecard() { TimecardID = System.Guid.NewGuid().ToString(), TimecardName = "CW 10 2014", StartDate = new DateTime(2014, 3, 3) };

            dellAppDB.Projects.InsertOnSubmit(firstProject);
            dellAppDB.SubmitChanges();

            dellAppDB.Timecards.InsertOnSubmit(firstTimecard);
            dellAppDB.SubmitChanges();

            DEBUG_NewTimecard(dellAppDB, firstTimecard);
        }

        private void DEBUG_NewTimecard(DBClass dellAppDB, Timecard newTimecard)
        {
            WorkTask moWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = newTimecard.TimecardID, DayDate = newTimecard.StartDate, WorkDescription = "description what you have done ", Ident_WorkTask = HelperClass.GetIdentForWorktask(newTimecard.StartDate, "") };
            WorkTask diWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = newTimecard.TimecardID, DayDate = newTimecard.StartDate.AddDays(1), WorkDescription = "description what you have done ", Ident_WorkTask = HelperClass.GetIdentForWorktask(newTimecard.StartDate.AddDays(1), "") };
            WorkTask miWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = newTimecard.TimecardID, DayDate = newTimecard.StartDate.AddDays(2), WorkDescription = "description what you have done ", Ident_WorkTask = HelperClass.GetIdentForWorktask(newTimecard.StartDate.AddDays(2), "") };
            WorkTask doWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = newTimecard.TimecardID, DayDate = newTimecard.StartDate.AddDays(3), WorkDescription = "description what you have done ", Ident_WorkTask = HelperClass.GetIdentForWorktask(newTimecard.StartDate.AddDays(3), "") };
            WorkTask frWorktask = new WorkTask { WorkTaskID = System.Guid.NewGuid().ToString(), IsComplete = false, TimecardID = newTimecard.TimecardID, DayDate = newTimecard.StartDate.AddDays(4), WorkDescription = "description what you have done ", Ident_WorkTask = HelperClass.GetIdentForWorktask(newTimecard.StartDate.AddDays(4), "") };

            dellAppDB.WorkTasks.InsertOnSubmit(moWorktask);
            dellAppDB.WorkTasks.InsertOnSubmit(diWorktask);
            dellAppDB.WorkTasks.InsertOnSubmit(miWorktask);
            dellAppDB.WorkTasks.InsertOnSubmit(doWorktask);
            dellAppDB.WorkTasks.InsertOnSubmit(frWorktask);

            dellAppDB.SubmitChanges();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
            else
            {
                appViewModel.SendExceptionReport(e.Exception);
            }

        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
            else
            {
                appViewModel.SendExceptionReport(e.ExceptionObject);
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}