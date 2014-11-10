using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimecardApp.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Live;
using System.Windows;
using System.IO.IsolatedStorage;
using System.IO;
using TimecardApp.Resources;
using Microsoft.Phone.BackgroundTransfer;
using TimecardApp.Model.NonPersistent;

namespace TimecardApp.ViewModel
{
    public class BackupViewModel : INotifyPropertyChanged
    {
        private string databaseBackupname;
        public string DatabaseBackupname
        {
            get
            {
                return databaseBackupname;
            }
            set
            {
                databaseBackupname = value;
                NotifyPropertyChanged("DatabaseBackupname");
            }
        }

        private string loginButtonText;
        public string LoginButtonText
        {
            get
            {
                return loginButtonText;
            }
            set
            {
                loginButtonText = value;
                NotifyPropertyChanged("LoginButtonText");
            }
        }

        private bool oneDriveConnected;
        public bool OneDriveConnected
        {
            get
            {
                return oneDriveConnected;
            }
            set
            {
                oneDriveConnected = value;
                OneDriveNotConnected = !value;
                if (!value)
                {
                    OneDriveFolderIDEnabled = false;
                    LoginButtonText = "Login (OneDrive)";
                }
                else
                    LoginButtonText = "Logged in";

                NotifyPropertyChanged("OneDriveConnected");
            }
        }

        private bool oneDriveFolderIDEnabled;
        public bool OneDriveFolderIDEnabled
        {
            get
            {
                return oneDriveFolderIDEnabled;
            }
            set
            {
                oneDriveFolderIDEnabled = value;
                NotifyPropertyChanged("OneDriveFolderIDEnabled");
            }
        }

        private bool oneDriveNotConnected;
        public bool OneDriveNotConnected
        {
            get
            {
                return oneDriveNotConnected;
            }
            set
            {
                oneDriveNotConnected = value;
                NotifyPropertyChanged("OneDriveNotConnected");
            }
        }

        private bool restoreLock;
        public bool RestoreLock
        {
            get
            {
                return restoreLock;
            }
            set
            {
                restoreLock = value;
                NotifyPropertyChanged("RestoreLock");
            }
        }

        private string oneDriveInfo;
        public string OneDriveInfo
        {
            get
            {
                return oneDriveInfo;
            }
            set
            {
                oneDriveInfo = value;
                NotifyPropertyChanged("OneDriveInfo");
            }
        }

        private ObservableCollection<Backupfile> backupfileCollection;
        public ObservableCollection<Backupfile> BackupfileCollection
        {
            get
            {
                return backupfileCollection;
            }
            set
            {
                backupfileCollection = value;
                NotifyPropertyChanged("BackupfileCollection");
            }
        }


        private string oneDriveFolder = "TimecardAppBackup";

        private LiveAuthClient oneDriveAuthClient;
        private string oneDriveFolderId = null;

        public BackupViewModel()
        {
            OneDriveConnected = false;
        }

        public async Task LoginOneDrive()
        {

            try
            {
                oneDriveAuthClient = new LiveAuthClient(AppResources.ClientID);
                LiveLoginResult result = await oneDriveAuthClient.LoginAsync(new string[] { "wl.skydrive_update" });

                if (result.Status == LiveConnectSessionStatus.Connected)
                {
                    await initialCreateFolder();

                    OneDriveConnected = true;
                    if (oneDriveFolderIDEnabled)
                    {
                        await getFileFromBackupFolderAsync();
                    }
                }
            }
            catch (LiveAuthException ex)
            {
                MessageBox.Show("Error during loing process:" + ex.Message);

            }
            catch (LiveConnectException ex)
            {
                App.AppViewModel.SendExceptionReport(ex);

            }

        }

        public void LogoutOneDrive()
        {
            try
            {
                oneDriveAuthClient.Logout();
                oneDriveFolderId = "";
                oneDriveFolderIDEnabled = false;
                OneDriveConnected = false;
            }
            catch (LiveAuthException ex)
            {
                MessageBox.Show("Error during loing process:" + ex.Message);

            }
            catch (LiveConnectException ex)
            {
                App.AppViewModel.SendExceptionReport(ex);

            }

        }

        public async Task ReloadAllBackupfiles()
        {

            if (oneDriveConnected != false)
            {
                if (oneDriveFolderIDEnabled != false)
                {
                    BackupfileCollection = null;
                    await getFileFromBackupFolderAsync();
                }
                else
                {
                    MessageBoxButton buttons = MessageBoxButton.OKCancel;
                    MessageBoxResult result = MessageBox.Show("There is no folder TimecardAppBackup in OneDrive, which is necessary. " +
                        "Do you allow, that this app creates the folder? Without the folder in OneDrive, no backup or restore is possible.", "", buttons);

                    if (result == MessageBoxResult.OK)
                    {
                        await createFolder();
                        await getFileFromBackupFolderAsync();
                    }
                    else
                        MessageBox.Show("Cannot reload the existing files, because there is no folder in OneDrive.");
                }
            }
            else
                MessageBox.Show("You are not logged in.");

        }

        public async Task RestoreBackupfile(Backupfile file)
        {
            RestoreLock = true;
            string tmpPathDatabase = "downloadedDatabase.sdf";
            //release all resources from DB 

            LiveConnectClient liveClient = new LiveConnectClient(oneDriveAuthClient.Session);

            try
            {
                LiveDownloadOperationResult downloadResult = await liveClient.DownloadAsync(file.FileID + "/content");
                using (Stream downloadStream = downloadResult.Stream)
                {
                    if (downloadStream != null)
                    {
                        using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            using (IsolatedStorageFileStream output = storage.CreateFile(tmpPathDatabase))
                            {
                                // Initialize the buffer.
                                byte[] readBuffer = new byte[4096];
                                int bytesRead = -1;

                                // Copy the file from the installation folder to the local folder. 
                                while ((bytesRead = downloadStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                                {
                                    output.Write(readBuffer, 0, bytesRead);
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("Download successful. Restore started.");

                IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();

                App.AppViewModel.RestoreDatabase(tmpPathDatabase);

                if( iso.FileExists(tmpPathDatabase))
                   iso.DeleteFile(tmpPathDatabase);

                MessageBox.Show("Restore succesfull.");
                RestoreLock = false;
            }
            catch (LiveConnectException ex)
            {
                App.AppViewModel.SendExceptionReport(ex);
                RestoreLock = false;
            }
        }

        public async Task BackupLocalDatabase()
        {
            string toUploadDatabaseName = "toUploadDatabase.sdf";
            //release all resources from DB 
            App.AppViewModel.DisposeCurrentDB();
            // Obtain the virtual store for the application.            
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            iso.CopyFile(AppResources.DatabaseName + ".sdf", toUploadDatabaseName , true);
            App.AppViewModel.ConnectDB();

            LiveConnectClient liveClient = new LiveConnectClient(oneDriveAuthClient.Session);
            try
            {
                using (Stream uploadStream = iso.OpenFile(toUploadDatabaseName , FileMode.Open))
                {
                    if (uploadStream != null)
                    {
                        LiveOperationResult uploadResult = await liveClient.UploadAsync(oneDriveFolderId, databaseBackupname + ".sdf", uploadStream, OverwriteOption.Overwrite);
                        MessageBox.Show("Upload successful.");
                    }
                }
                iso.DeleteFile(toUploadDatabaseName);
                await getFileFromBackupFolderAsync();
            }
            catch (LiveConnectException ex)
            {
                App.AppViewModel.SendExceptionReport(ex);
            }
        }

        private async Task createFolder()
        {
            try
            {
                var folderData = new Dictionary<string, object>();
                folderData.Add("name", oneDriveFolder);
                LiveConnectClient liveClient = new LiveConnectClient(oneDriveAuthClient.Session);
                LiveOperationResult operationResult = await liveClient.PostAsync("me/skydrive", folderData);
                dynamic createResult = operationResult.Result;
                oneDriveFolderId = createResult.id;
                MessageBox.Show(string.Join(" ", "Created folder:", createResult.name));
                OneDriveFolderIDEnabled = true;
            }
            catch (LiveConnectException ex)
            {
                App.AppViewModel.SendExceptionReport(ex);
            }
        }

        private async Task initialCreateFolder()
        {
            try
            {
                var folderData = new Dictionary<string, object>();
                folderData.Add("name", oneDriveFolder);
                LiveConnectClient liveClient = new LiveConnectClient(oneDriveAuthClient.Session);

                // Retrieves all the directories.
                var queryFolder = "me/skydrive/files?filter=folders";
                var opResult = await liveClient.GetAsync(queryFolder);
                var items = opResult.Result["data"] as List<object>;

                foreach (object item in items)
                {
                    IDictionary<string, object> folder = item as IDictionary<string, object>;
                    // Checks if current folder has the passed name.
                    if (folder["name"].ToString().ToLowerInvariant() == oneDriveFolder.ToLowerInvariant())
                    {
                        oneDriveFolderId = folder["id"].ToString();
                        OneDriveFolderIDEnabled = true;
                        break;
                    }
                }

                if (oneDriveFolderId == null)
                {
                    MessageBoxButton buttons = MessageBoxButton.OKCancel;
                    MessageBoxResult result = MessageBox.Show("There is no folder TimecardAppBackup in OneDrive, which is necessary. " +
                        "Do you allow, that this app creates the folder? Without the folder in OneDrive, no backup or restore is possible.", "", buttons);

                    if (result == MessageBoxResult.OK)
                    {
                        await createFolder();
                    }
                    else
                        MessageBox.Show("If you change your mind, click on reload files and the app will ask again for creating the folder.");
                }
            }
            catch (LiveConnectException ex)
            {
                App.AppViewModel.SendExceptionReport(ex);
            }
        }

        private async Task getFileFromBackupFolderAsync()
        {
            try
            {
                LiveConnectClient liveClient = new LiveConnectClient(oneDriveAuthClient.Session);
                var queryFiles = oneDriveFolderId + "/files";
                var operation = await liveClient.GetAsync(queryFiles);

                var items = operation.Result["data"] as List<object>;
                BackupfileCollection = new ObservableCollection<Backupfile>();

                foreach (object item in items)
                {
                    IDictionary<string, object> file = item as IDictionary<string, object>;
                    if (file["name"].ToString().EndsWith(".sdf"))
                    {
                        Backupfile bFile = new Backupfile();
                        bFile.FileID = file["id"].ToString();
                        bFile.Filename = file["name"].ToString();
                        bFile.FileType = file["type"].ToString();
                        BackupfileCollection.Add(bFile);
                    }
                }
            }
            catch (LiveConnectException ex)
            {
                App.AppViewModel.SendExceptionReport(ex);
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
