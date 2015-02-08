using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimecardApp.Model.NonPersistent
{
    public static class HelperClass
    {
        public static string ReturnManipulatedURLForTimelog(string inputURL)
        {
            if (inputURL.EndsWith("/") && inputURL.Count() > "https://".Count())
                inputURL = inputURL.Remove(inputURL.Length - 1);
            if (inputURL.StartsWith("http:"))
                inputURL = inputURL.Replace("http:", "https:");
            return inputURL;
        }

        public static void FocusedTextBoxUpdateSource()
        {
            var focusedElement = FocusManager.GetFocusedElement();
            var focusedTextBox = focusedElement as TextBox;

            if (focusedTextBox != null)
            {
                var binding = focusedTextBox.GetBindingExpression(TextBox.TextProperty);

                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }
            else
            {
                var focusedPasswordBox = focusedElement as PasswordBox;

                if (focusedPasswordBox != null)
                {
                    var binding = focusedPasswordBox.GetBindingExpression(PasswordBox.PasswordProperty);

                    if (binding != null)
                    {
                        binding.UpdateSource();
                    }
                }
            }
        }

        public static string GetEncryptedPWString(string clearPW)
        {
            try
            {
                byte[] passwordInByte = UTF8Encoding.UTF8.GetBytes(clearPW);
                byte[] protectedPasswordByte = ProtectedData.Protect(passwordInByte, null);
                return Convert.ToBase64String(protectedPasswordByte, 0, protectedPasswordByte.Length);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during password encryption. Error message:" + ex.Message + " \n InnerException: " + ex.InnerException);
            }
        }

        public static string GetDecryptedPWString(string encrytpedPW)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encrytpedPW);
                byte[] passwordByte = ProtectedData.Unprotect(encryptedBytes, null);
                return UTF8Encoding.UTF8.GetString(passwordByte, 0, passwordByte.Length);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during password encryption. Error message:" + ex.Message + " \n InnerException: " + ex.InnerException);
            }
        }

        public static string GetShortDayName(DateTime date)
        {
            // hier darf immer nur ein Montag gespeichert werden
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Mo";
                case DayOfWeek.Tuesday:
                    return "Tu";
                case DayOfWeek.Wednesday:
                    return "We";
                case DayOfWeek.Thursday:
                    return "Th";
                case DayOfWeek.Friday:
                    return "Fr";
                case DayOfWeek.Saturday:
                    return "Sa";
                case DayOfWeek.Sunday:
                    return "Su";
                default:
                    return "";
            }
        }

        public static DateTime GetFirstDayOfWeek(DateTime date)
        {
            // hier darf immer nur ein Montag gespeichert werden
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return date.Date;
                case DayOfWeek.Tuesday:
                    return date.AddDays(-1).Date;
                case DayOfWeek.Wednesday:
                    return date.AddDays(-2).Date;
                case DayOfWeek.Thursday:
                    return date.AddDays(-3).Date;
                case DayOfWeek.Friday:
                    return date.AddDays(-4).Date;
                case DayOfWeek.Saturday:
                    return date.AddDays(-5).Date;
                case DayOfWeek.Sunday:
                    return date.AddDays(-6).Date;
                default:
                    return date;
            }
        }

        public static string GetIdentForWorktask(DateTime date, string projecIdent)
        {
            return GetShortDayName(date) + "-" + date.ToString("dd/MM/yyyy") + "-" + projecIdent;
        }

        public static string GetIdentForTimelogTask(string taskName, string projectName, int projectID)
        {
            return taskName + " - " + projectName + " (ID:" +  projectID.ToString() + ")";
        }

        public static int NumberOfWeek(DateTime date)
        {
            int weekNumber;
            // Aktuelle Kultur ermitteln
            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            // Aktuellen Kalender ermitteln
            Calendar calendar = currentCulture.Calendar;

            // Kalenderwoche über das Calendar-Objekt ermitteln
            int calendarWeek = calendar.GetWeekOfYear(date,
               currentCulture.DateTimeFormat.CalendarWeekRule,
               currentCulture.DateTimeFormat.FirstDayOfWeek);

            // Überprüfen, ob eine Kalenderwoche größer als 52
            // ermittelt wurde und ob die Kalenderwoche des Datums
            // in einer Woche 2 ergibt: In diesem Fall hat
            // GetWeekOfYear die Kalenderwoche nicht nach ISO 8601 
            // berechnet (Montag, der 31.12.2007 wird z. B.
            // fälschlicherweise als KW 53 berechnet). 
            // Die Kalenderwoche wird dann auf 1 gesetzt
            if (calendarWeek > 52)
            {
                date = date.AddDays(7);
                int testCalendarWeek = calendar.GetWeekOfYear(date,
                   currentCulture.DateTimeFormat.CalendarWeekRule,
                   currentCulture.DateTimeFormat.FirstDayOfWeek);
                if (testCalendarWeek == 2)
                    calendarWeek = 1;
            }
            weekNumber = calendarWeek;

            // Das Jahr der Kalenderwoche ermitteln
            int year = date.Year;
            if (calendarWeek == 1 && date.Month == 12)
                year++;
            if (calendarWeek >= 52 && date.Month == 1)
                year--;

            return weekNumber;

        }
    }
}
