using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wisecorp.Context;
using wisecorp.Models.DBModels;
using wisecorp.ViewModels;

namespace test_wisecorp
{
    [TestClass]
    public class TestVMAdmin
    {
        private WisecorpContext context;
        private VMAdminTest viewmodel;
        private DatabaseHelper dbHelper;

        [TestInitialize]
        public async Task Initialize()
        {
            dbHelper = new DatabaseHelper();
            context = dbHelper.CreateContext();
            await dbHelper.CreateTables(context);
            viewmodel = new VMAdminTest(context);
        }

        [TestMethod]
        public async Task RoleChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Changement
            viewmodel.DeepCopyAccout.RoleId = 2;

            //Call la commande du changements
            viewmodel.Edit_Execute();

            //Assert
            Account c = await context.Accounts.FirstOrDefaultAsync();
            c.RoleId.Should().Be(2);

        }

        [TestMethod]
        public async Task DepChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Changement
            viewmodel.DeepCopyAccout.DepartementId = 2;

            //Call la commande du changements
            viewmodel.Edit_Execute();

            //Assert
            Account c = await context.Accounts.FirstOrDefaultAsync();
            c.DepartementId.Should().Be(2);
        }

        [TestMethod]
        public async Task TitleIdChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Changement
            viewmodel.DeepCopyAccout.TitleId = 4;


            viewmodel.Edit_Execute();

            //Assert
            Account updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            updatedAccount.TitleId.Should().Be(4);
        }

        [TestMethod]
        public async Task EmploymentDateChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Change
            DateTime newDate = new DateTime(2024, 1, 1);
            viewmodel.DeepCopyAccout.EmploymentDate = newDate;

            //Execute
            viewmodel.Edit_Execute();

            //Assert
            Account updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            updatedAccount.EmploymentDate.Should().Be(newDate);
        }

        [TestMethod]
        public async Task FullNameChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Change
            viewmodel.DeepCopyAccout.FullName = "John Doe";

            //Execute
            viewmodel.Edit_Execute();

            //Assert
            Account updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            updatedAccount.FullName.Should().Be("John Doe");
        }

        [TestMethod]
        public async Task PhoneChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Change
            viewmodel.DeepCopyAccout.Phone = "123-456-7890";

            //Execute
            viewmodel.Edit_Execute();

            //Assert
            Account updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            updatedAccount.Phone.Should().Be("123-456-7890");
        }

        [TestMethod]
        public async Task SalaryChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Change
            viewmodel.DeepCopyAccout.Salary = 50000;

            //Execute
            viewmodel.Edit_Execute();

            //Assert
            Account updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            updatedAccount.Salary.Should().Be(50000);
        }

        [TestMethod]
        public async Task NbHourChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Change
            viewmodel.DeepCopyAccout.NbHour = 40;

            //Execute
            viewmodel.Edit_Execute();

            //Assert
            Account updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            updatedAccount.NbHour.Should().Be(40);
        }

        [TestMethod]
        public async Task HourBankChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Change
            viewmodel.DeepCopyAccout.HourBank = 80;

            //Execute
            viewmodel.Edit_Execute();

            //Assert
            Account updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            updatedAccount.HourBank.Should().Be(80);
        }

        [TestMethod]
        public async Task EmailChange()
        {
            //Setup
            viewmodel.SelectedAccount = await context.Accounts.FirstOrDefaultAsync();
            int id = viewmodel.SelectedAccount.Id;
            viewmodel.DeepCopyAccout = viewmodel.SelectedAccount;

            //Change
            viewmodel.DeepCopyAccout.Email = "newemail@example.com";

            //Execute
            viewmodel.Edit_Execute();

            //Assert
            Account updatedAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            updatedAccount.Email.Should().Be("newemail@example.com");
        }


        [TestCleanup]
        public async Task Cleanup()
        {
            await dbHelper.DropTestTables(context);
        }
    }
}
