// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeHistoryServiceTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
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

using NUnit.Framework;

namespace DEHPCommon.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;

    using DEHPCommon.Enumerators;
    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.Services.AdapterVersionService;
    using DEHPCommon.Services.ExchangeHistory;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using Moq;

    [TestFixture]
    public class ExchangeHistoryServiceTestFixture
    {
        private Mock<IHubController> hubController;
        private Mock<IStatusBarControlViewModel> statusBar;
        private Mock<IAdapterVersionService> adapterVersionService;
        private ExchangeHistoryService service;
        private DomainOfExpertise domain;
        private Person person;
        private ElementDefinition element;

        [SetUp]
        public void Setup()
        {
            var session = new Mock<ISession>();
            this.person = new Person() { GivenName = "person" };
            session.Setup(x => x.ActivePerson).Returns(this.person);

            this.hubController = new Mock<IHubController>();
            this.hubController.Setup(x => x.Session).Returns(session.Object);
            this.domain = new DomainOfExpertise() { ShortName = "PWR" };
            this.hubController.Setup(x => x.CurrentDomainOfExpertise).Returns(this.domain);

            this.statusBar = new Mock<IStatusBarControlViewModel>();

            this.adapterVersionService = new Mock<IAdapterVersionService>();

            this.service = new ExchangeHistoryService(this.hubController.Object, this.statusBar.Object, this.adapterVersionService.Object);

            this.element = new ElementDefinition() {Name = "element", ShortName = "el"};
        }

        [Test]
        public void VerifyRead()
        {
            Assert.IsEmpty(this.service.Read());
            this.WriteOneEntry();
            var entries = this.service.Read();
            Assert.IsNotEmpty(entries);
            Assert.AreEqual(this.domain.ShortName, entries.FirstOrDefault()?.Domain);
            Assert.AreEqual(this.person.Name, entries.FirstOrDefault()?.Person);
        }

        private void WriteOneEntry()
        {
            this.service.Append("test");
            Assert.DoesNotThrowAsync(async () => await this.service.Write());
        }
        
        [Test]
        public void VerifyAppendValueSetDifferences()
        {
            var booleanParameterType = new BooleanParameterType() { Name = "testParameterType" };

            var measurementScale = new IntervalScale() { Name = "scale" };

            var parameter = new Parameter()
            {
                Scale = measurementScale,
                ParameterType = booleanParameterType,
                ValueSet = 
                { 
                    new ParameterValueSet()
                    {
                        Computed = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Manual = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Reference = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Published = new ValueArray<string>(new List<string>() {"-", "-"}),
                    }
                }
            };

            this.element.Parameter.Add(parameter);
            var clone = parameter.Clone(true);
            clone.ValueSet.First().Computed = new ValueArray<string>(new List<string>() { "false", "true", "true", "false" });
            
            Assert.DoesNotThrow(() => this.service.Append(parameter.ValueSet.First(), clone.ValueSet.First()));
            Assert.IsTrue(this.service.PendingEntries.LastOrDefault()?.Message.Contains(parameter.ModelCode()));
            
            var sampledParameterType = new SampledFunctionParameterType()
            {
                Name = "testParameterType",
                IndependentParameterType = { new IndependentParameterTypeAssignment() { ParameterType = booleanParameterType }},
                DependentParameterType = { new DependentParameterTypeAssignment() { ParameterType = booleanParameterType }}
            };

            var sampledParameter = new Parameter()
            {
                ParameterType = sampledParameterType, Scale = measurementScale, IsOptionDependent = false,
                ValueSet =
                {
                    new ParameterValueSet()
                    {
                        Computed = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Manual = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Reference = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Published = new ValueArray<string>(new List<string>() {"-", "-"}),
                    }
                }
            };

            this.element.Parameter.Add(sampledParameter);

            var sampledClone = sampledParameter.Clone(true);
            clone.ValueSet.First().Computed = new ValueArray<string>(new List<string>() { "false", "true", "true", "false" });

            Assert.DoesNotThrow(() => this.service.Append(sampledParameter.ValueSet.First(), sampledClone.ValueSet.First()));
            Assert.IsTrue(this.service.PendingEntries.LastOrDefault()?.Message.Contains(parameter.ModelCode()));

            var parameterOverride = new ParameterOverride()
            {
                Parameter = sampledParameter,
                ValueSet =
                {
                    new ParameterOverrideValueSet()
                    {
                        ParameterValueSet = sampledParameter.ValueSet.FirstOrDefault(),
                        Computed = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Manual = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Reference = new ValueArray<string>(new List<string>() {"-", "-"}),
                        Published = new ValueArray<string>(new List<string>() {"-", "-"}),
                    }
                }
            };

            var elementUsage = new ElementUsage()
            {
                ParameterOverride = { parameterOverride}
            };

            this.element.ContainedElement.Add(elementUsage);

            var parameterOverrideclone = parameterOverride.Clone(true);
            parameterOverride.ValueSet.First().Computed = new ValueArray<string>(new List<string>() { "false", "true", "true", "false" });

            Assert.DoesNotThrow(() => this.service.Append(parameterOverride.ValueSet.First(), parameterOverrideclone.ValueSet.First()));
            Assert.IsTrue(this.service.PendingEntries.LastOrDefault()?.Message.Contains(parameterOverride.ModelCode()));
        }

        [Test]
        public void VerifyAppend()
        {
            var booleanParameterType = new BooleanParameterType() { Name = "testParameterType" };

            var parameter = new Parameter() { ParameterType = booleanParameterType};
            this.element.Parameter.Add(parameter);

            Assert.DoesNotThrow(() => this.service.Append((Thing)parameter, ChangeKind.Delete));
            
            Assert.IsTrue(this.service.PendingEntries.LastOrDefault()?.Message.Contains(booleanParameterType.Name));
            
            var elementDefinition = new ElementDefinition() { Name = "testElement" };

            Assert.DoesNotThrow(() =>
            {
                this.service.Append(elementDefinition, ChangeKind.Create);
            });
            
            Assert.IsTrue(this.service.PendingEntries.LastOrDefault()?.Message.Contains(elementDefinition.Name));

            var notThing = new NotThing("testNotThing") {Iid = Guid.NewGuid()};

            Assert.DoesNotThrow(() =>
            {
                this.service.Append(notThing, ChangeKind.Update);
            });

            Assert.IsTrue(this.service.PendingEntries.LastOrDefault()?.Message.Contains(notThing.Iid.ToString()));
        }

        [Test]
        public void VerifyWrite()
        {
            Assert.DoesNotThrow(this.WriteOneEntry);

            this.statusBar.Setup(x =>
                    x.Append(It.IsAny<string>(), It.IsAny<StatusBarMessageSeverity>()))
                .Throws<InvalidCastException>();

            this.service.Append("test");
            Assert.ThrowsAsync<InvalidCastException>(async () => await this.service.Write());
        }

        [Test]
        public void VerifyClear()
        {
            this.service.Append("test");
            this.service.ClearPending();
            Assert.IsEmpty(this.service.PendingEntries);
        }

        [Test]
        public void VerifyAppendWithMultipleValuesValueSets()
        {
            var values = new List<string>() { "-", "-", "-" };

            var valueSet = new ParameterValueSet()
            {
                Computed = new ValueArray<string>(values)
            };

            var valueSetClone = valueSet.Clone(false);

            var compoundParameterType = new CompoundParameterType()
            {
                Name = "testParameterType",
                ShortName = "testParameterType",
                Component =
                {
                    new ParameterTypeComponent() { ParameterType = new DateParameterType() { Name = "date" } },
                    new ParameterTypeComponent() { ParameterType = new TextParameterType() { Name = "text" } },
                    new ParameterTypeComponent() { ParameterType = new SimpleQuantityKind() { Name = "number" } }
                }
            };

            var parameter = new Parameter() { ParameterType = compoundParameterType, ValueSet = { valueSet }};
            this.element.Parameter.Add(parameter);
            
            Assert.DoesNotThrow(() => this.service.Append(valueSet, valueSetClone));

            Assert.IsTrue(this.service.PendingEntries.LastOrDefault()?.Message.Contains(compoundParameterType.Name));
        }
    }
}
