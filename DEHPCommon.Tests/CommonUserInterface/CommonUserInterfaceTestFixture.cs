// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonUserInterfaceTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.CommonUserInterface
{
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Threading.Tasks;

    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using DEHPCommon.CommonUserInterface.ViewModels.Common;
    using DEHPCommon.CommonUserInterface.ViewModels.Tabs;
    using DEHPCommon.HubController;
    using DEHPCommon.HubController.Interfaces;

    using NUnit.Framework;

    using Moq;

    using ReactiveUI;

    /// <summary>
    /// Suite of tests for the <see cref="CommonUserInterfaceTestFixture"/> class.
    /// </summary>
    [TestFixture]
    public class CommonUserInterfaceTestFixture
    {
        private Mock<ISession> session;
        private Mock<IHubController> controller;
        private LoginViewModel loginViewModel;
        public LoginLayoutGroupViewModel loginLayoutGroupViewModel;


        private KeyValuePair<string, string> serverType;
        private string uri;
        private string userName;
        private string password;

        [SetUp]
        public void SetUp()
        {
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;

            this.session = new Mock<ISession>();
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(new SiteDirectory());
            this.controller = new Mock<IHubController>();
            this.controller.Setup(x => x.Session).Returns(this.session.Object);
            this.loginViewModel = new LoginViewModel(this.controller.Object);
            this.loginLayoutGroupViewModel = new LoginLayoutGroupViewModel();
            this.loginLayoutGroupViewModel.LoginViewModel = this.loginViewModel;

            this.serverType = new KeyValuePair<string, string>("CDP", "CDP4 WebServices");
            this.uri = "http://localhost:4000";
            this.userName = "DEHP-UserNew";
            this.password = "1234";
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Verify_that_LoginViewModel_can_be_create_correctly_and_LoginCommand_working_properly()
        {
            Assert.IsFalse(this.loginViewModel.LoginCommand.CanExecute(null));

            this.loginViewModel.ServerType = this.serverType;
            this.loginViewModel.Uri = this.uri;
            this.loginViewModel.UserName = this.userName;
            this.loginViewModel.Password = this.password;

            Assert.IsTrue(this.loginViewModel.LoginCommand.CanExecute(null));
        }

        [Test]
        public async Task Verify_that_ExecuteLogin_in_LoginViewModel_is_execute_correctly()
        {
            Assert.That(this.loginViewModel.LoginFailed, Is.False);
            Assert.That(this.loginViewModel.LoginSuccessfully, Is.False);

            await this.loginViewModel.LoginCommand.ExecuteAsyncTask();

            Assert.That(this.loginViewModel.LoginFailed, Is.True);
            Assert.That(this.loginViewModel.LoginSuccessfully, Is.False);

            this.loginViewModel.ServerType = this.serverType;
            this.loginViewModel.Uri = this.uri;
            this.loginViewModel.UserName = this.userName;
            this.loginViewModel.Password = this.password;

            await this.loginViewModel.LoginCommand.ExecuteAsyncTask();

            Assert.That(this.loginViewModel.LoginSuccessfully, Is.True);
            Assert.That(this.loginViewModel.LoginFailed, Is.False);
        }

        [Test]
        public async Task Verify_that_LoginLayoutGroupViewModel_can_be_create_correctly_and_working_properly()
        {
            Assert.That(this.loginLayoutGroupViewModel.ServerIsChecked, Is.False);

            this.loginViewModel.ServerType = this.serverType;
            this.loginViewModel.Uri = this.uri;
            this.loginViewModel.UserName = this.userName;
            this.loginViewModel.Password = this.password;

            await this.loginViewModel.LoginCommand.ExecuteAsyncTask();

            Assert.That(this.loginLayoutGroupViewModel.ServerIsChecked, Is.True);
        }
    }
}
