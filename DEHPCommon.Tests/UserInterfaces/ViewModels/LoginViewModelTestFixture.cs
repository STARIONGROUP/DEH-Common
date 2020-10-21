// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModelTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// 
//    This file is part of DEHP Common Library
// 
//    The DEHPCommon is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The DEHPCommon is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.UserInterfaces.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Threading.Tasks;

    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.DAL;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels;
    using DEHPCommon.UserInterfaces.ViewModels.Tabs;
    using DEHPCommon.UserPreferenceHandler.Enums;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    /// <summary>
    /// Suite of tests for the <see cref="LoginViewModelTestFixture"/> class.
    /// </summary>
    [TestFixture]
    public class LoginViewModelTestFixture
    {
        private Mock<ISession> session;
        private Mock<IHubController> hubController;
        private LoginViewModel loginViewModel;
        private LoginLayoutGroupViewModel loginLayoutGroupViewModel;
        
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
            
            this.hubController.Setup(x => x.GetSiteDirectory()).Returns(new SiteDirectory(Guid.NewGuid(), null, null)
            {
                Domain = { new DomainOfExpertise(Guid.NewGuid(), null, null)}
            });

            this.hubController.Setup(x => x.GetEngineeringModels()).Returns(
                new List<EngineeringModelSetup>()
                {
                    new EngineeringModelSetup(Guid.NewGuid(), null, null) { Name = "test0", IterationSetup = { new IterationSetup(Guid.NewGuid(), null, null)}},
                    new EngineeringModelSetup(Guid.NewGuid(), null, null) { Name = "test1", IterationSetup = { new IterationSetup(Guid.NewGuid(), null, null)}},
                    new EngineeringModelSetup(Guid.NewGuid(), null, null) { Name = "test2", IterationSetup = { new IterationSetup(Guid.NewGuid(), null, null)}}
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
            Assert.That(this.loginViewModel.LoginSuccessfull, Is.False);

            await this.loginViewModel.LoginCommand.ExecuteAsyncTask();

            Assert.That(this.loginViewModel.LoginFailed, Is.True);
            Assert.That(this.loginViewModel.LoginSuccessfull, Is.False);

            this.loginViewModel.SelectedServerType = this.serverType;
            this.loginViewModel.Uri = this.uri;
            this.loginViewModel.UserName = this.userName;
            this.loginViewModel.Password = this.password;

            await this.loginViewModel.LoginCommand.ExecuteAsyncTask();

            Assert.That(this.loginViewModel.LoginSuccessfull, Is.True);
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

        [Test]
        public async Task VerifyCloseCommandCanExecute()
        {
            this.loginViewModel.SelectedServerType = this.serverType;
            this.loginViewModel.Uri = this.uri;
            this.loginViewModel.UserName = this.userName;
            this.loginViewModel.Password = this.password;

            await this.loginViewModel.LoginCommand.ExecuteAsyncTask();

            Assert.IsTrue(this.loginViewModel.LoginSuccessfull);
            Assert.IsNotEmpty(this.loginViewModel.EngineeringModels);

            this.loginViewModel.SelectedEngineeringModel = this.loginViewModel.EngineeringModels.First();
            Assert.IsNotEmpty(this.loginViewModel.Iterations);

            this.loginViewModel.SelectedIteration = this.loginViewModel.Iterations.First();
            Assert.IsNotEmpty(this.loginViewModel.DomainsOfExpertise);
            
            Assert.IsFalse(this.loginViewModel.CloseCommand.CanExecute(null));
            this.loginViewModel.SelectedDomainOfExpertise = this.loginViewModel.DomainsOfExpertise.FirstOrDefault();
            Assert.IsTrue(this.loginViewModel.CloseCommand.CanExecute(null));
        }
    }
}
