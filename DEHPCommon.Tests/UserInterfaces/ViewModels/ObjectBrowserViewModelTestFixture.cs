// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectBrowserViewModel.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2020 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski.
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
    using DEHPCommon.Services.ObjectBrowserTreeSelectorService;
    using DEHPCommon.UserInterfaces.ViewModels;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    [TestFixture]
    public class ObjectBrowserViewModelTestFixture
    {
        private Mock<IHubController> hubController;
        private Mock<IObjectBrowserTreeSelectorService> objectBrowserTreeSelectorService;
        private ObjectBrowserViewModel viewModel;
        private Mock<ISession> session;
        private ElementDefinition element0;
        private ElementDefinition element1;
        private ElementDefinition element2;
        private ElementDefinition element3;
        private Iteration iteration;
        private DomainOfExpertise domain;
        private IterationSetup iterationSetup;
        private EngineeringModelSetup engineeringSetup;
        private Mock<IPermissionService> permissionService;

        [SetUp]
        public void Setup()
        {
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;
            this.SetupElements();
            
            this.iterationSetup = new IterationSetup(Guid.NewGuid(), null, null);

            this.engineeringSetup = new EngineeringModelSetup(Guid.NewGuid(), null, null)
            {
                IterationSetup = { this.iterationSetup}
            };

            this.iteration = new Iteration(Guid.NewGuid(), null, null)
            {
                Element = { this.element0, this.element1, this.element2, this.element3 }, TopElement = this.element0,
                IterationSetup = this.iterationSetup
            };
            
            this.domain = new DomainOfExpertise(Guid.NewGuid(), null, null) { Name = "TestDomain", ShortName = "TD"};

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
            this.objectBrowserTreeSelectorService = new Mock<IObjectBrowserTreeSelectorService>();
            this.objectBrowserTreeSelectorService.Setup(x => x.ThingKinds).Returns(new List<Type>() { typeof(ElementDefinition) });
            this.viewModel = new ObjectBrowserViewModel(this.hubController.Object, this.objectBrowserTreeSelectorService.Object);
            AppContainer.BuildContainer();
        }

        private void SetupElements()
        {
            this.element0 = new ElementDefinition(Guid.NewGuid(), null, null);
            this.element1 = new ElementDefinition(Guid.NewGuid(), null, null);
            this.element2 = new ElementDefinition(Guid.NewGuid(), null, null);
            this.element3 = new ElementDefinition(Guid.NewGuid(), null, null);
        }

        [Test]
        public void VerifyProperties()
        {
            Assert.IsEmpty(this.viewModel.Things);
            Assert.IsNotNull(this.viewModel.Caption);
            Assert.IsNull(this.viewModel.ToolTip);
            this.hubController.Setup(x => x.Session).Returns(this.session.Object);
            this.hubController.Setup(x => x.OpenIteration).Returns(this.iteration);
            this.hubController.Setup(x => x.IsSessionOpen).Returns(true);
            this.viewModel = new ObjectBrowserViewModel(this.hubController.Object, this.objectBrowserTreeSelectorService.Object);
            Assert.IsNotNull(this.viewModel.ToolTip);
            Assert.IsFalse(this.viewModel.IsBusy);
        }

        [Test]
        public void VerifyTreesGetBuilt()
        {
            this.hubController.Setup(x => x.Session).Returns(this.session.Object);
            this.hubController.Setup(x => x.OpenIteration).Returns(this.iteration);
            this.viewModel = new ObjectBrowserViewModel(this.hubController.Object, this.objectBrowserTreeSelectorService.Object);
            this.viewModel.BuildTrees();
            Assert.IsNotEmpty(this.viewModel.Things);
            Assert.AreEqual(1, this.viewModel.Things.Count);
        }

        [Test]
        public void VerifyDispose()
        {
            Assert.DoesNotThrow(() => this.viewModel.Dispose());
        }
    }
}
