﻿using System;
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
            string password1 = "s";
            string encryptedPW1 = HelperClass.GetEncryptedPWString(password1);
            string decryptedPW1 = HelperClass.GetDecryptedPWString(encryptedPW1);
            Assert.AreEqual(password1, decryptedPW1, "Password1 are not the same!");


        }

        [TestMethod]
        public void PasswordSaveTest2()
        {

            string password2 = "msaxdcnd3494_.723ndwed39)2e3n3edmweofihfwl";
            string encryptedPW2 = HelperClass.GetEncryptedPWString(password2);
            string decryptedPW2 = HelperClass.GetDecryptedPWString(encryptedPW2);
            Assert.AreEqual(password2, decryptedPW2, "Password2 are not the same!");


        }

        [TestMethod]
        public void PasswordSaveTest3()
        {
            string password3 = "test2test";
            string encryptedPW3 = HelperClass.GetEncryptedPWString(password3);
            string decryptedPW3 = HelperClass.GetDecryptedPWString(encryptedPW3);
            Assert.AreEqual(password3, decryptedPW3, "Password3 are not the same!");


        }

        [TestMethod]
        public void PasswordSaveTest4()
        {
            string password4 = "!§$%&/()=?`*'_:;";
            string encryptedPW4 = HelperClass.GetEncryptedPWString(password4);
            string decryptedPW4 = HelperClass.GetDecryptedPWString(encryptedPW4);
            Assert.AreEqual(password4, decryptedPW4, "Password4 are not the same!");
        }

        [TestMethod]
        public void TestMethod2()
        {

        }
    }
}
