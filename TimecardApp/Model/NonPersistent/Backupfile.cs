using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.Model.NonPersistent
{
    public class Backupfile : INotifyPropertyChanged
    {
        private string filename;
        public string Filename
        {
            get { return filename; }
            set
            {
                if (filename != value)
                { filename = value; }
            }

        }

        private string fileID;
        public string FileID
        {
            get { return fileID; }
            set
            {
                if (fileID != value)
                { fileID = value; }
            }
        }

        private string fileType;
        public string FileType
        {
            get { return fileType; }
            set
            {
                if (fileType != value)
                { fileType = value; }
            }
        }

        private string fileUpdated;
        public string FileUpdated
        {
            get { return fileUpdated; }
            set
            {
                if (fileUpdated != value)
                { fileUpdated = value; }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
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
