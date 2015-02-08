using Microsoft.Phone.Data.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.Model.NonPersistent
{
    public class DBMigrator : IDisposable 
    {
        private int latestVersion;
        private DBClass database;
        DatabaseSchemaUpdater dbUpdater;
        private bool m_blnIsDisposed = false;

        public DBMigrator(string dbConnectionString, int latestVers)
        {
            latestVersion = latestVers;

            if (!String.IsNullOrEmpty(dbConnectionString))
                database = new DBClass(dbConnectionString);
        }

        public void MigrateDatabase()
        {
            if (database.DatabaseExists() == true)
            {
                // Check whether a database update is needed.
                dbUpdater = database.CreateDatabaseSchemaUpdater();

                if (dbUpdater.DatabaseSchemaVersion < latestVersion)
                {
                    //performn here the changes for database in higher versions
                    if (dbUpdater.DatabaseSchemaVersion == 1)
                    {
                        migrateFromVersion1();
                    }
                }
            }
            else
                throw new Exception("Connectionstring doesn't work for a database!");
        }

        public bool hasToMigrate()
        {
            if (database.DatabaseExists() == true)
            {
                // Check whether a database update is needed.
                dbUpdater = database.CreateDatabaseSchemaUpdater();

                if (dbUpdater.DatabaseSchemaVersion < latestVersion)
                    return true;
                else
                    return false;
            }
            else
                throw new Exception("Connectionstring doesn't work for a database!");
        }

        private void migrateFromVersion1 ()
        {
            dbUpdater.AddTable<TimelogTask>();
            dbUpdater.AddTable<TimelogSetting>();

            dbUpdater.AddColumn<WorkTask>("TimelogTaskUID");
            dbUpdater.AddAssociation<WorkTask>("TimelogTask");
            dbUpdater.AddColumn<WorkTask>("LastTimelogRegistration");
            dbUpdater.AddColumn<WorkTask>("TimelogWorkunitGUID");
            dbUpdater.AddColumn<WorkTask>("IsForTimelogRegistration");
            dbUpdater.AddColumn<Setting>("IsUsingTimelog");

            dbUpdater.DatabaseSchemaVersion = latestVersion;
            dbUpdater.Execute();
        }
         
        // Destruktor
        ~DBMigrator()
        {
            Dispose(false);
        }
 
        // Implementierung der Schnittstelle IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
 
        protected virtual void Dispose(bool blnDisposing)
        {
            if (!m_blnIsDisposed)
            {
                // Methode wird zum ersten Mal aufgerufen
                if(blnDisposing)
                {
                    database.Dispose();
                }
                // Hier unmanaged Objekte freigeben (z.B. IntPtr)
            }
            // Dafür sorgen, dass Methode nicht mehr aufgerufen werden kann.
            m_blnIsDisposed = true;
        }
    }
}
