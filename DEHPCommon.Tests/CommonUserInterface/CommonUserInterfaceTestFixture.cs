// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonUserInterfaceTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.CommonUserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Threading.Tasks;

    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.DAL;

    using DEHPCommon.CommonUserInterface.ViewModels.Common;
    using DEHPCommon.CommonUserInterface.ViewModels.Tabs;
    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.UserPreferenceHandler.Enums;

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
        private Mock<IHubController> hubController;
        private LoginViewModel loginViewModel;
        public LoginLayoutGroupViewModel loginLayoutGroupViewModel;
        
        private KeyValuePair<ServerType, string> serverType;
        private string uri;
        private string userName;
        private string password;

        [SetUp]
        public void SetUp()
        {
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;

            this.session = new Mock<ISession>();
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(new SiteDirectory());
            this.hubController = new Mock<IHubController>();
            this.hubController.Setup(x => x.Session).Returns(this.session.Object);
            this.hubController.Setup(x => x.Open(It.IsAny<Credentials>(), It.IsAny<ServerType>())).Returns(Task.FromResult(true));
            
            this.hubController.Setup(x => x.GetEngineeringModels()).Returns(
                new List<EngineeringModelSetup>()
                {
                    new EngineeringModelSetup(Guid.NewGuid(), null, null) { Name = "test0" },
                    new EngineeringModelSetup(Guid.NewGuid(), null, null) { Name = "test1" },
                    new EngineeringModelSetup(Guid.NewGuid(), null, null) { Name = "test2" }
                });

            this.loginViewModel = new LoginViewModel(this.hubController.Object);
            this.loginLayoutGroupViewModel = new LoginLayoutGroupViewModel();
            this.loginLayoutGroupViewModel.LoginViewModel = this.loginViewModel;

            this.serverType = new KeyValuePair<ServerType, string>(ServerType.Cdp4WebServices, "CDP4 WebServices");
            this.uri = "http://localhost:4000";
            this.userName = "DEHP-UserNew";
            this.password = "1234";
        }

        [Test]
        public void Verify_that_LoginViewModel_can_be_create_correctly_and_LoginCommand_working_properly()
        {
            Assert.IsFalse(this.loginViewModel.LoginCommand.CanExecute(null));

            this.loginViewModel.SelectedServerType = this.serverType;
            this.loginViewModel.Uri = this.uri;
            this.loginViewModel.UserName = this.userName;
            this.loginViewModel.Password = this.password;

            Assert.IsTrue(this.loginViewModel.DataSourceList.TryGetValue(ServerType.OcdtWspServer, out _));
            Assert.IsTrue(this.loginViewModel.DataSourceList.TryGetValue(ServerType.Cdp4WebServices, out _));

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

            this.loginViewModel.SelectedServerType = this.serverType;
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

            this.loginViewModel.SelectedServerType = this.serverType;
            this.loginViewModel.Uri = this.uri;
            this.loginViewModel.UserName = this.userName;
            this.loginViewModel.Password = this.password;

            await this.loginViewModel.LoginCommand.ExecuteAsyncTask();

            Assert.That(this.loginLayoutGroupViewModel.ServerIsChecked, Is.True);
        }
    }
}
