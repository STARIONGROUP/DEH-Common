// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionBrowserViewModelTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2020 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Merlin Bieze, Naron Phou, Patxi Ozkoidi, Alexander van Delft
//            Nathanael Smiechowski, Kamil Wojnowski
//
//    This file is part of CDP4-IME Community Edition. 
//    The CDP4-IME Community Edition is the RHEA Concurrent Design Desktop Application and Excel Integration
//    compliant with ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-IME Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or any later version.
//
//    The CDP4-IME Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.UserInterfaces.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.ReportingData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.Events;
    using CDP4Dal.Operations;
    using CDP4Dal.Permission;

    using DEHPCommon.Events;
    using DEHPCommon.UserInterfaces.ViewModels;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    using DevExpress.Xpf.Docking.Platform;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    [TestFixture]
    public class ElementDefinitionBrowserViewModelTestFixture
    {
        private SiteDirectory sitedir;
        private EngineeringModel model;
        private Person person;
        private List<Thing> cache;

        private EngineeringModelSetup engineeringModelSetup;
        private ModelReferenceDataLibrary mrdl;
        private SiteReferenceDataLibrary srdl;
        private Iteration iteration;
        private ElementDefinition elementDef;
        private Participant participant;
        private Uri uri;
        private DomainOfExpertise domain;
        private Mock<ISession> session;
        private Assembler assembler;
        private Mock<IPermissionService> permissionService;
        private TextParameterType pt;
        private readonly PropertyInfo rev = typeof (Thing).GetProperty("RevisionNumber");

        [SetUp]
        public void Setup()
        {
            this.cache = new List<Thing>();
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;
            this.session = new Mock<ISession>();
            this.uri = new Uri("http://test.com");
            this.assembler = new Assembler(this.uri);

            this.sitedir = new SiteDirectory(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.pt = new TextParameterType(Guid.NewGuid(), this.assembler.Cache, this.uri);

            this.person = new Person(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.sitedir.Person.Add(this.person);
            this.domain = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri){Name = "TestDoE"};
            this.sitedir.Domain.Add(this.domain);

            this.session.Setup(x => x.ActivePerson).Returns(this.person);
            this.model = new EngineeringModel(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.iteration = new Iteration(Guid.NewGuid(), this.assembler.Cache, this.uri);
            
            this.elementDef = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "1", 
                Owner = this.domain, 
                Container = this.iteration
            };
            
            this.participant = new Participant(Guid.NewGuid(), this.assembler.Cache, this.uri) { Person = this.person };
            this.participant.Domain.Add(this.domain);

            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Container = this.elementDef,
                ParameterType = this.pt,
                Owner = this.elementDef.Owner
            };

            var parameterOverride = new ParameterOverride(Guid.NewGuid(), this.assembler.Cache, this.uri) { Owner = this.domain, Parameter = parameter };
            var elementUsage = new ElementUsage(Guid.NewGuid(), this.assembler.Cache, this.uri){  ElementDefinition = this.elementDef };
            elementUsage.ParameterOverride.Add(parameterOverride);
            this.elementDef.ContainedElement.Add(elementUsage);
            
            this.model.Iteration.Add(this.iteration);
            
            var iterationSetup = new IterationSetup(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.iteration.IterationSetup = iterationSetup;

            this.engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "ModelSetup" };
            this.engineeringModelSetup.IterationSetup.Add(iterationSetup);

            this.srdl = new SiteReferenceDataLibrary(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.sitedir.SiteReferenceDataLibrary.Add(this.srdl);
            this.srdl.ParameterType.Add(this.pt);

            this.mrdl = new ModelReferenceDataLibrary(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                RequiredRdl = this.srdl
            };

            this.engineeringModelSetup.RequiredRdl.Add(this.mrdl);

            this.model.EngineeringModelSetup = this.engineeringModelSetup;
            this.model.EngineeringModelSetup.Participant.Add(this.participant);
            this.permissionService = new Mock<IPermissionService>();

            this.permissionService.Setup(x => x.CanRead(It.IsAny<Thing>())).Returns(true);
            this.permissionService.Setup(x => x.CanWrite(It.IsAny<Thing>())).Returns(true);
            this.permissionService.Setup(x => x.CanWrite(It.IsAny<ClassKind>(), It.IsAny<Thing>())).Returns(true);
            this.session.Setup(x => x.PermissionService).Returns(this.permissionService.Object);
            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>> { { this.iteration, new Tuple<DomainOfExpertise, Participant>(this.domain, this.participant) } });

            this.assembler.Cache.TryAdd(new CacheKey(this.iteration.Iid, null), new Lazy<Thing>(() => this.iteration));
            this.assembler.Cache.TryAdd(new CacheKey(this.model.Iid, null), new Lazy<Thing>(() => this.model));

            this.session.Setup(x => x.Assembler).Returns(this.assembler);
            this.session.Setup(x => x.Refresh());
        }

        [TearDown]
        public void TearDown()
        {
            CDPMessageBus.Current.ClearSubscriptions();
        }

        [Test]
        public void VerifyRowExpansion()
        {
            this.iteration.Element.Add(this.elementDef);
            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);
            vm.ExpandAllRows();
            Assert.IsTrue(vm.IsExpanded);
            Assert.IsTrue(vm.ContainedRows.FirstOrDefault()?.IsExpanded);
            vm.CollapseAllRows();
            Assert.IsFalse(vm.IsExpanded);
            Assert.IsFalse(vm.ContainedRows.FirstOrDefault()?.IsExpanded);
        }

        [Test]
        public void VerifyThatElementDefArePopulatedFromEvent()
        {
            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);

            this.rev.SetValue(this.iteration, 50);
            this.iteration.Element.Add(this.elementDef);
            CDPMessageBus.Current.SendObjectChangeEvent(this.iteration, EventKind.Updated);
            Assert.AreEqual(1, vm.ContainedRows.Count);

            this.rev.SetValue(this.iteration, 51);
            this.iteration.Element.Remove(this.elementDef);
            CDPMessageBus.Current.SendObjectChangeEvent(this.iteration, EventKind.Updated);

            Assert.AreEqual(0, vm.ContainedRows.Count);
        }

        [Test]
        public void VerifyThatElementDefArePopulated()
        {
            this.iteration.Element.Add(this.elementDef);
            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);

            Assert.AreEqual(1, vm.ContainedRows.Count);

            Assert.IsNotNull(vm.DataSource);
            Assert.IsNotNull(vm.CurrentModel);
            Assert.IsNotNull(vm.DomainOfExpertise);
            Assert.IsFalse(vm.HasUpdateStarted);
            Assert.IsNull(vm.FocusedRow);
            Assert.IsNull(vm.Feedback);
            Assert.IsNotNull(vm.Identifier);

            Assert.AreEqual(0, vm.CurrentIteration);

            var row = (ElementDefinitionRowViewModel) vm.ContainedRows.First();            
            Assert.That(row.Name, Is.Not.Null.Or.Empty);
            Assert.IsNotNull(row.Owner);

            this.elementDef.Name = "updated";
            this.elementDef.Owner = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "test" };

            // workaround to modify a read-only field
            this.rev.SetValue(this.elementDef, 50);
            CDPMessageBus.Current.SendObjectChangeEvent(this.elementDef, EventKind.Updated);

            Assert.AreEqual(this.elementDef.Name, row.Name);
            Assert.AreSame(this.elementDef.Owner, row.Owner);
        }

        [Test]
        public void VerifyHighlightElementUsagesEventIsSent()
        {
            var eu1 = new ElementDefinition();
            var eu2 = new ElementDefinition();

            CDPMessageBus.Current.Listen<ElementUsageHighlightEvent>().Subscribe(x => this.OnElementUsageHighlightEvent(x.ElementDefinition));

            CDPMessageBus.Current.SendMessage(new ElementUsageHighlightEvent(eu1));
            Assert.AreEqual(1, this.cache.Count);

            CDPMessageBus.Current.SendMessage(new ElementUsageHighlightEvent(eu2));
            Assert.AreEqual(2, this.cache.Count);
        }

        private void OnElementUsageHighlightEvent(Thing highlightedThings)
        {
            this.cache.Add(highlightedThings);
        }

        [Test]
        public void VerifyThatNoneIsReturnedUponNullDomain()
        {
            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>> {{ this.iteration, new Tuple<DomainOfExpertise, Participant>(null,null) }} );
            this.iteration.Element.Add(this.elementDef);
            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);

            Assert.AreEqual("None", vm.DomainOfExpertise);
        }

        [Test]
        public void VerifyThatActiveDomainIsDisplayed()
        {
            var domainOfExpertise = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "domain" };

            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>
            {
                { this.iteration, new Tuple<DomainOfExpertise, Participant>(domainOfExpertise, this.participant) }
            });

            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);
            Assert.AreEqual("domain []", vm.DomainOfExpertise);

            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>
            {
                { this.iteration, new Tuple<DomainOfExpertise, Participant>(null, null) }
            });

            vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);
            Assert.AreEqual("None", vm.DomainOfExpertise);
        }

        [Test]
        public void VerifThatIfDomainIsRenamedBrowserIsUpdated()
        {
            var domainOfExpertise = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "System", ShortName = "SYS" };

            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>
            {
                { this.iteration, new Tuple<DomainOfExpertise, Participant>(domainOfExpertise, null) }
            });

            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);
            Assert.AreEqual("System [SYS]", vm.DomainOfExpertise);

            domainOfExpertise.Name = "Systems";
            this.rev.SetValue(domainOfExpertise, 50);

            CDPMessageBus.Current.SendObjectChangeEvent(domainOfExpertise, EventKind.Updated);
            Assert.AreEqual("Systems [SYS]", vm.DomainOfExpertise);
        }

        [Test]
        public void VerifyThatIfEngineeringModelSetupIsChangedBrowserIsUpdated()
        {
            var domainOfExpertise = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri) { Name = "System", ShortName = "SYS" };

            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>
            {
                { this.iteration, new Tuple<DomainOfExpertise, Participant>(domainOfExpertise, null) }
            });

            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);
            Assert.AreEqual("ModelSetup", vm.CurrentModel);

            this.engineeringModelSetup.Name = "testing";
            this.rev.SetValue(this.engineeringModelSetup, 50);

            CDPMessageBus.Current.SendObjectChangeEvent(this.engineeringModelSetup, EventKind.Updated);
            Assert.AreEqual("testing", vm.CurrentModel);
        }

        [Test]
        public void VerifyThatContextMenuIsPopulated()
        {
            var group = new ParameterGroup(Guid.NewGuid(), this.assembler.Cache, this.uri);
            
            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ParameterType = this.pt
            };

            var def2 = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var parameter2 = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var usage = new ElementUsage(Guid.NewGuid(), this.assembler.Cache, this.uri);
            
            var paramOverride = new ParameterOverride(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Parameter = parameter2
            };

            parameter2.ParameterType = this.pt;

            var usage2 = new ElementUsage(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ElementDefinition = def2
            };

            def2.Parameter.Add(parameter2);
            usage.ParameterOverride.Add(paramOverride);
            usage.ElementDefinition = def2;

            this.elementDef.Parameter.Add(parameter);
            this.elementDef.ParameterGroup.Add(group);
            this.elementDef.ContainedElement.Add(usage);
            this.elementDef.ContainedElement.Add(usage2);

            this.iteration.Element.Add(this.elementDef);
            this.iteration.Element.Add(def2);

            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);
            vm.PopulateContextMenu();

            Assert.AreEqual(0, vm.ContextMenu.Count);

            var defRow = vm.ContainedRows.Last();

            vm.SelectedThing = defRow;
            vm.PopulateContextMenu();
            Assert.AreEqual(3, vm.ContextMenu.Count);

            vm.SelectedThing = defRow.ContainedRows[0];
            vm.PopulateContextMenu();
            Assert.AreEqual(1, vm.ContextMenu.Count);

            vm.SelectedThing = defRow.ContainedRows[1];
            vm.PopulateContextMenu();
            Assert.AreEqual(0, vm.ContextMenu.Count);

            var usageRow = defRow.ContainedRows[2];
            var usage2Row = defRow.ContainedRows[3];

            vm.SelectedThing = usageRow;
            vm.PopulateContextMenu();
            Assert.AreEqual(5, vm.ContextMenu.Count);

            vm.SelectedThing = usageRow.ContainedRows.Single();
            vm.PopulateContextMenu();
            Assert.AreEqual(1, vm.ContextMenu.Count);

            vm.SelectedThing = usage2Row.ContainedRows.Single();
            vm.PopulateContextMenu();
            Assert.AreEqual(1, vm.ContextMenu.Count);

            vm.Dispose();
        }

        [Test]
        public void VerifyThatSetTopElementWorks()
        {
            var def2 = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri);
            
            this.iteration.Element.Add(this.elementDef);
            this.iteration.Element.Add(def2);

            this.iteration.TopElement = this.elementDef;

            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);
            var defRow = (ElementDefinitionRowViewModel)vm.ContainedRows.Single(x => x.Thing.Iid == this.elementDef.Iid);
            var def2Row = (ElementDefinitionRowViewModel)vm.ContainedRows.Single(x => x.Thing.Iid == def2.Iid);
            Assert.IsTrue(defRow.IsTopElement);
            Assert.IsFalse(def2Row.IsTopElement);

            this.iteration.TopElement = def2;
            this.rev.SetValue(this.iteration, 50);
            CDPMessageBus.Current.SendObjectChangeEvent(this.iteration, EventKind.Updated);
            Assert.IsFalse(defRow.IsTopElement);
            Assert.IsTrue(def2Row.IsTopElement);

            this.iteration.Element.Remove(def2);
            this.rev.SetValue(this.iteration, 51);
            CDPMessageBus.Current.SendObjectChangeEvent(this.iteration, EventKind.Updated);
            Assert.IsTrue(def2Row.IsTopElement);
        }

        [Test]
        public void VerifyCommand()
        {
            var group = new ParameterGroup(Guid.NewGuid(), this.assembler.Cache, this.uri);

            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ParameterType = this.pt
            };

            var def2 = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var parameter2 = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri);
            var usage = new ElementUsage(Guid.NewGuid(), this.assembler.Cache, this.uri);

            var paramOverride = new ParameterOverride(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Parameter = parameter2
            };

            parameter2.ParameterType = this.pt;

            var usage2 = new ElementUsage(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ElementDefinition = def2
            };

            def2.Parameter.Add(parameter2);
            usage.ParameterOverride.Add(paramOverride);
            usage.ElementDefinition = def2;

            this.elementDef.Parameter.Add(parameter);
            this.elementDef.ParameterGroup.Add(group);
            this.elementDef.ContainedElement.Add(usage);
            this.elementDef.ContainedElement.Add(usage2);

            this.iteration.Element.Add(this.elementDef);
            this.iteration.Element.Add(def2);

            var vm = new ElementDefinitionsBrowserViewModel(this.iteration, this.session.Object);
            Assert.IsTrue(vm.ExportCommand.CanExecute(null));
            Assert.DoesNotThrow(() => vm.ExportCommand.Execute(null));

            Assert.IsTrue(vm.HelpCommand.CanExecute(null));
            Assert.DoesNotThrow(() => vm.HelpCommand.Execute(null));

            Assert.IsTrue(vm.RefreshCommand.CanExecute(null));
            Assert.DoesNotThrow(() => vm.RefreshCommand.Execute(null));
            this.session.Verify(x => x.Refresh(), Times.Once);

            Assert.IsTrue(vm.ExpandRowsCommand.CanExecute(null));
            Assert.DoesNotThrow(() => vm.ExpandRowsCommand.Execute(null));

            Assert.IsTrue(vm.CollpaseRowsCommand.CanExecute(null));
            Assert.DoesNotThrow(() => vm.CollpaseRowsCommand.Execute(null));

            vm.SelectedThing = vm.ContainedRows.First();
            Assert.DoesNotThrow(() => vm.ExpandRowsCommand.Execute(null));
            Assert.DoesNotThrow(() => vm.CollpaseRowsCommand.Execute(null));
            Assert.IsTrue(vm.ChangeFocusCommand.CanExecute(null));
            Assert.DoesNotThrow(() => vm.ChangeFocusCommand.Execute(null));
        }
    }
}