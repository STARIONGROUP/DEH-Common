// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicationBrowserViewModelTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Permission;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.PublicationBrowser;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    [TestFixture]
    public class PublicationBrowserViewModelTestFixture
    {
        private PublicationBrowserViewModel viewModel;

        private Iteration iteration;
        private DomainOfExpertise domain;
        private IterationSetup iterationSetup;
        private EngineeringModelSetup engineeringSetup;

        private Mock<ISession> session;
        private Mock<IHubController> hubController;
        private Mock<IPermissionService> permissionService;

        [SetUp]
        public void Setup()
        {
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;

            this.iterationSetup = new IterationSetup(Guid.NewGuid(), null, null);

            this.engineeringSetup = new EngineeringModelSetup(Guid.NewGuid(), null, null)
            {
                IterationSetup = { this.iterationSetup }
            };

            this.iteration = new Iteration(Guid.NewGuid(), null, null)
            {
                IterationSetup = this.iterationSetup
            };

            this.domain = new DomainOfExpertise(Guid.NewGuid(), null, null) { Name = "TestDomain", ShortName = "TD" };

            this.session = new Mock<ISession>();
            this.session.Setup(x => x.ActivePerson).Returns(new Person());
            this.session.Setup(x => x.DataSourceUri).Returns("dataSourceUri");

            this.permissionService = new Mock<IPermissionService>();
            this.permissionService.Setup(x => x.Session).Returns(this.session.Object);
            this.permissionService.Setup(x => x.CanRead(It.IsAny<Thing>())).Returns(true);
            this.permissionService.Setup(x => x.CanWrite(It.IsAny<Thing>())).Returns(true);
            this.session.Setup(x => x.PermissionService).Returns(this.permissionService.Object);

            this.session.Setup(x => x.OpenIterations).Returns(new ConcurrentDictionary<Iteration, Tuple<DomainOfExpertise, Participant>>(
                new List<KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>>()
                {
                    new KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>(this.iteration, new Tuple<DomainOfExpertise, Participant>(this.domain, new Participant()))
                }));

            this.hubController = new Mock<IHubController>();
            this.hubController.Setup(x => x.GetIteration()).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>() { { this.iteration, new Tuple<DomainOfExpertise, Participant>(null, null) } });

            this.viewModel = new PublicationBrowserViewModel(this.hubController.Object);

            AppContainer.BuildContainer();
        }

        [Test]
        public void VerifyProperties()
        {
            Assert.IsNotNull(this.viewModel.Caption);
            Assert.IsNull(this.viewModel.ToolTip);
            Assert.IsNull(this.viewModel.PublicationsViewModel);

            this.hubController.Setup(x => x.Session).Returns(this.session.Object);
            this.hubController.Setup(x => x.OpenIteration).Returns(this.iteration);
            this.hubController.Setup(x => x.IsSessionOpen).Returns(true);

            this.viewModel = new PublicationBrowserViewModel(this.hubController.Object);

            Assert.IsNotNull(this.viewModel.ToolTip);
            Assert.IsFalse(this.viewModel.IsBusy);

            Assert.IsNotNull(this.viewModel.PublicationsViewModel);
            Assert.IsEmpty(this.viewModel.PublicationsViewModel.Publications);
            Assert.IsEmpty(this.viewModel.PublicationsViewModel.Domains);
            Assert.IsEmpty(this.viewModel.PublicationsViewModel.Parameters);
            Assert.AreEqual(this.engineeringSetup, this.viewModel.PublicationsViewModel.CurrentEngineeringModelSetup);
        }

        [Test]
        public void VerifyDispose()
        {
            Assert.DoesNotThrow(() => this.viewModel.Dispose());
        }
    }
}
