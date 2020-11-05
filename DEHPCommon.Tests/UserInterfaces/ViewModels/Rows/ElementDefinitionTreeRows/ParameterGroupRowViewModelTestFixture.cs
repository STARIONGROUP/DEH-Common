// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupRowViewModelTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows
{
    using System;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Threading.Tasks;
    using System.Windows;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.Operations;
    using CDP4Dal.Permission;

    using DEHPCommon.Enumerators;
    using DEHPCommon.Events;
    using DEHPCommon.UserInterfaces.ViewModels.Rows;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    /// <summary>
    /// Suite of tests for the <see cref="ElementDefinitionRowViewModel"/>
    /// </summary>
    [TestFixture]
    public class ParameterGroupRowViewModelTestFixture
    {
        /// <summary>
        /// A mock of the session.
        /// </summary>
        private Mock<ISession> session;

        /// <summary>
        /// A mock of the <see cref="IPermissionService"/>
        /// </summary>
        private Mock<IPermissionService> permissionService;

        /// <summary>
        /// The uri.
        /// </summary>
        private Uri uri;

        private Assembler assembler;
        private EngineeringModel model;
        private Iteration iteration;

        private SiteDirectory sitedir;
        private EngineeringModelSetup modelsetup;
        private IterationSetup iterationsetup;
        private SiteReferenceDataLibrary srdl;
        private ModelReferenceDataLibrary mrdl;

        [SetUp]
        public void Setup()
        {
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;
            this.session = new Mock<ISession>();
            this.uri = new Uri("http://www.rheagroup.com");
            this.assembler = new Assembler(this.uri);

            this.permissionService = new Mock<IPermissionService>();

            this.session.Setup(x => x.Assembler).Returns(this.assembler);
            this.session.Setup(x => x.PermissionService).Returns(this.permissionService.Object);

            this.sitedir = new SiteDirectory(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.modelsetup = new EngineeringModelSetup(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.iterationsetup = new IterationSetup(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.srdl = new SiteReferenceDataLibrary(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.mrdl = new ModelReferenceDataLibrary(Guid.NewGuid(), this.assembler.Cache, this.uri) { RequiredRdl = this.srdl };

            this.modelsetup.RequiredRdl.Add(this.mrdl);
            this.modelsetup.IterationSetup.Add(this.iterationsetup);
            this.sitedir.Model.Add(this.modelsetup);
            this.sitedir.SiteReferenceDataLibrary.Add(this.srdl);

            this.model = new EngineeringModel(Guid.NewGuid(), this.assembler.Cache, this.uri) {EngineeringModelSetup = this.modelsetup};
            this.iteration = new Iteration(Guid.NewGuid(), this.assembler.Cache, this.uri) {IterationSetup = this.iterationsetup};
            this.model.Iteration.Add(this.iteration);
            this.permissionService.Setup(x => x.CanWrite(It.IsAny<Thing>())).Returns(true);
        }

        [TearDown]
        public void TearDown()
        {
            CDPMessageBus.Current.ClearSubscriptions();
        }

        [Test]
        public void VerifyHiglightingWorks()
        {
            this.permissionService.Setup(x => x.CanWrite(It.IsAny<ClassKind>(), It.IsAny<Thing>())).Returns(true);

            var domainOfExpertise = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Owner = domainOfExpertise };
            var parameterGroup = new ParameterGroup(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "TestName" };
            elementDefinition.ParameterGroup.Add(parameterGroup);

            this.iteration.Element.Add(elementDefinition);

            var row = new ParameterGroupRowViewModel(parameterGroup, domainOfExpertise, this.session.Object, null);

            CDPMessageBus.Current.SendMessage(new HighlightEvent(row.Thing), row.Thing);
            Assert.IsTrue(row.IsHighlighted);
            CDPMessageBus.Current.SendMessage(new CancelHighlightEvent());
            Assert.IsFalse(row.IsHighlighted);
        }

        [Test]
        public void VerifyProperties()
        {
            this.permissionService.Setup(x => x.CanWrite(It.IsAny<ClassKind>(), It.IsAny<Thing>())).Returns(true);

            var domainOfExpertise = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Owner = domainOfExpertise };
            var parameterGroup = new ParameterGroup(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "TestName" };
            elementDefinition.ParameterGroup.Add(parameterGroup);

            this.iteration.Element.Add(elementDefinition);

            var row = new ParameterGroupRowViewModel(parameterGroup, domainOfExpertise, this.session.Object, null);
            Assert.IsNotNull(row.Name);
            Assert.IsNotNull(row.Thing);
            Assert.IsNotNull(row.Session);
            Assert.IsNotNull(row.ContainedRows);
            Assert.IsNull(row.ContainerViewModel);
            Assert.AreEqual(row.IDalUri, this.uri);
            Assert.AreEqual(0, row.Index);
            Assert.IsFalse(row.IsExpanded);
            Assert.IsNull(row.IsBusy);
            Assert.IsFalse(row.IsValueSetEditorActive);
            Assert.IsFalse(row.IsFavorite);
            Assert.IsFalse(row.IsHighlighted);
            Assert.IsNull(row.ContainingGroup);
            Assert.AreEqual(0, row.RevisionNumber);
            Assert.IsNotEmpty(row.RowType);
            Assert.AreEqual(RowStatusKind.Active, row.RowStatus);
            Assert.IsNotNull(row.Tooltip);
            Assert.IsNull(row.TopContainerViewModel);
        }
    }
}
