using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TimecardApp.Model.NonPersistent;

namespace TimecardAppUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PasswordSaveTest1()
        {
            string password = "s";
            string encryptedPW = HelperClass.GetEncryptedPWString(password);
            string decryptedPW = HelperClass.GetDecryptedPWString(encryptedPW);
            Assert.AreEqual(password, decryptedPW, "Passwords are not the same!");
        }

        [TestMethod]
        public void PasswordSaveTest2()
        {

            string password = "msaxdcnd3494_.723ndwed39)2e3n3edmweofihfwl";
            string encryptedPW = HelperClass.GetEncryptedPWString(password);
            string decryptedPW = HelperClass.GetDecryptedPWString(encryptedPW);
            Assert.AreEqual(password, decryptedPW, "Passwords are not the same!");
        }

        [TestMethod]
        public void PasswordSaveTest3()
        {
            string password = "test2test";
            string encryptedPW = HelperClass.GetEncryptedPWString(password);
            string decryptedPW = HelperClass.GetDecryptedPWString(encryptedPW);
            Assert.AreEqual(password, decryptedPW, "Passwords are not the same!");
        }

        [TestMethod]
        public void PasswordSaveTest4()
        {
            string password = "!§$%&/()=?`*'_:;";
            string encryptedPW = HelperClass.GetEncryptedPWString(password);
            string decryptedPW = HelperClass.GetDecryptedPWString(encryptedPW);
            Assert.AreEqual(password, decryptedPW, "Passwords are not the same!");
        }

        [TestMethod]
        public void PasswordSaveTest5()
        {
            string password = "";
            string encryptedPW = HelperClass.GetEncryptedPWString(password);
            string decryptedPW = HelperClass.GetDecryptedPWString(encryptedPW);
            Assert.AreEqual(password, decryptedPW, "Passwords are not the same!");
        }
    }
}
