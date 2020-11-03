// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubBrowserHeaderViewModelTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels;

    using Moq;

    using NUnit.Framework;

    public class HubBrowserHeaderViewModelTestFixture
    {
        private Mock<IHubController> hubController;
        private Mock<ISession> session;
        private Iteration iteration;

        [SetUp]
        public void Setup()
        {
            this.iteration = new Iteration()
            {
                IterationSetup = new IterationSetup() { IterationNumber = 42},
                Container = new EngineeringModelSetup() { Name = "TestName" },
                DefaultOption = new Option() { Name = "TestName"}
            };

            this.session = new Mock<ISession>();
            this.session.Setup(x => x.ActivePerson).Returns(new Person() { GivenName = "Test", Surname = "Name"});
            this.session.Setup(x => x.DataSourceUri).Returns("http://data.source");
            this.hubController = new Mock<IHubController>();
            this.hubController.Setup(x => x.Session).Returns(this.session.Object);
            this.hubController.Setup(x => x.CurrentDomainOfExpertise).Returns(new DomainOfExpertise() { Name = "TestName" });
        }

        [Test]
        public void VerifyProperties()
        {
            var viewModel = new HubBrowserHeaderViewModel(this.hubController.Object);
            Assert.IsEmpty(viewModel.Person);
            Assert.IsEmpty(viewModel.DataSource);
            Assert.IsEmpty(viewModel.Model);
            Assert.IsEmpty(viewModel.Domain);
            Assert.IsEmpty(viewModel.Option);
            Assert.IsEmpty(viewModel.Iteration);
            this.hubController.Setup(x => x.OpenIteration).Returns(this.iteration);
            viewModel = new HubBrowserHeaderViewModel(this.hubController.Object);
            Assert.IsNotEmpty(viewModel.Person);
            Assert.IsNotEmpty(viewModel.DataSource);
            Assert.IsNotEmpty(viewModel.Model);
            Assert.IsNotEmpty(viewModel.Domain);
            Assert.IsNotEmpty(viewModel.Option);
            Assert.AreEqual("42", viewModel.Iteration);
        }
    }
}
