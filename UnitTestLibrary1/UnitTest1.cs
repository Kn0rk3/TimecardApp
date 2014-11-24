using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTestLibrary1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void PasswordSaveTest()
        {
            string password1 = "s";
            string password2 = "msaxdcnd3494_.723ndwed39)2e3n3edmweofihfwl";
            string password3 = "test2";
            string password4 = "!§$%&/()=?`*'_:;";

            string encryptedPW1 = TimecardApp.Model.NonPersistent.HelperClass.GetEncryptedPWString(password1);
            string decryptedPW1 = HelperClass.GetDecryptedPWString(encryptedPW1);
            Assert.AreEqual(password1, decryptedPW1, "Password1 are not the same!");

            string encryptedPW2 = HelperClass.GetEncryptedPWString(password2);
            string decryptedPW2 = HelperClass.GetDecryptedPWString(encryptedPW2);
            Assert.AreEqual(password2, decryptedPW2, "Password2 are not the same!");

            string encryptedPW3 = HelperClass.GetEncryptedPWString(password3);
            string decryptedPW3 = HelperClass.GetDecryptedPWString(encryptedPW3);
            Assert.AreEqual(password3, decryptedPW3, "Password3 are not the same!");

            string encryptedPW4 = HelperClass.GetEncryptedPWString(password4);
            string decryptedPW4 = HelperClass.GetDecryptedPWString(encryptedPW4);
            Assert.AreEqual(password4, decryptedPW4, "Password4 are not the same!");
        }
    }
}
