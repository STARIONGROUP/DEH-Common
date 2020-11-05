// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupChildRowComparerTestFixture.cs" company="RHEA System S.A.">
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

using NUnit.Framework;

namespace DEHPCommon.Tests.UserInterfaces.ViewModels.Comparers
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Permission;

    using DEHPCommon.UserInterfaces.ViewModels.Comparers;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    using Moq;

    [TestFixture]
    public class ParameterGroupChildRowComparerTestFixture
    {
        private Mock<ISession> session;
        private ParameterGroupChildRowComparer comparer;
        private ParameterRowViewModel xThing;
        private ParameterGroupRowViewModel yThing;
        private Parameter xParameter;
        private Parameter yParameter;
        private Iteration iteration;
        private Participant participant;
        private Person person;
        private DomainOfExpertise domain;

        [SetUp]
        public void Setup()
        {
            this.person = new Person(Guid.NewGuid(), null, null);
            this.domain = new DomainOfExpertise(Guid.NewGuid(), null, null) { Name = "active", ShortName = "active" };
            this.participant = new Participant(Guid.NewGuid(), null, null) { Person = this.person, SelectedDomain = this.domain };

            this.iteration = new Iteration(Guid.NewGuid(), null, iDalUri: null)
            {
                Container = new EngineeringModel(Guid.NewGuid(), null, null)
                {
                    EngineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), null, null) { Participant = { this.participant }}
                }
            };

            this.session = new Mock<ISession>();
            this.session.Setup(x => x.PermissionService).Returns(new PermissionService(this.session.Object));

            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>()
            {
                {this.iteration, new Tuple<DomainOfExpertise, Participant>(this.domain, this.participant) }
            });

            this.session.Setup(x => x.ActivePerson).Returns(this.person);

            this.comparer = new ParameterGroupChildRowComparer();

            this.xParameter = new Parameter(Guid.NewGuid(), null, null)
            {
                ParameterType = new TextParameterType(Guid.NewGuid(), null, null),
                Container = new ElementDefinition(Guid.NewGuid(), null, null) { Container = this.iteration }
            };

            this.yParameter = new Parameter(Guid.NewGuid(), null, null)
            {
                ParameterType = new BooleanParameterType(Guid.NewGuid(), null, null),
                Container = new ElementDefinition(Guid.NewGuid(), null, null) { Container = this.iteration }
            };

            this.xThing = new ParameterRowViewModel(this.xParameter, this.session.Object, null);
            
            this.yThing = new ParameterGroupRowViewModel(
                new ParameterGroup(Guid.NewGuid(), null, null), this.domain, this.session.Object, null);
        }

        [Test]
        public void VerifyCompareSameType()
        {
            Assert.AreEqual(0, this.comparer.Compare(this.xThing, this.xThing));
            Assert.AreEqual(0, this.comparer.Compare(this.yThing, this.yThing));
        }
    }
}
