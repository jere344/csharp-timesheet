using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using wisecorp.Context;
using wisecorp.Models.DBModels;
using wisecorp.ViewModels;

namespace test_wisecorp
{
    [TestClass]
    public class TestLogin
    {
        private WisecorpContext context;
        private VMLogin viewmodel;
        private DatabaseHelper dbHelper;

        [TestInitialize]
        public async Task Initialize()
        {
            dbHelper = new DatabaseHelper();
            context = dbHelper.CreateContext();
            await dbHelper.CreateTables(context);
            viewmodel = new VMLogin(context);
        }

        [TestMethod]
        public void Login_WithEmptyEmail()
        {
            //Arrange
            viewmodel.Email = null;
            viewmodel.Password = "Qwerty123";
            viewmodel.SetPassword(CreateSecureString("Qwerty234"));

            //Act
            viewmodel.Login.Execute(null);

            //Assert
            Assert.AreEqual("Le courriel ne peut pas etre vide.", viewmodel.ErrorMessage);
        }

        [TestMethod]
        public void Login_WithEmptyPassword()
        {
            //Arrange
            viewmodel.Email = "test@example.com";
            viewmodel.Password = null;

            //Act
            viewmodel.Login.Execute(null);

            //Assert
            Assert.AreEqual("Le mot de passe ne peut pas etre vide.", viewmodel.ErrorMessage);
        }
        [TestMethod]
        public void Login_WithEmptyPasswordAndEmail()
        {
            //Arrange
            viewmodel.Email = null;
            viewmodel.Password = null;
            viewmodel.SetPassword(CreateSecureString("Qwerty234"));

            //Act
            viewmodel.Login.Execute(null);

            //Assert
            Assert.AreEqual("Le mot de passe et le courriel ne peuvent pas etre vide.", viewmodel.ErrorMessage);
        }

        private SecureString CreateSecureString(string password)
        {
            SecureString secureString = new SecureString();
            foreach (char c in password)
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await dbHelper.DropTestTables(context);
        }
    }
}
