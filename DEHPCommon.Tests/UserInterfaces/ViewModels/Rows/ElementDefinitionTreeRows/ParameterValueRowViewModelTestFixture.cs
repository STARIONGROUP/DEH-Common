// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueRowViewModelTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows
{
    using System;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;
    using DEHPCommon.Utilities;

    using Moq;

    [TestFixture]
    public class ParameterValueRowViewModelTestFixture
    {
        private ParameterValueRowViewModel viewModel;
        private Mock<ISession> session;
        private Parameter parameter;
        private Option option;

        [SetUp]
        public void Setup()
        {
            this.parameter = new Parameter(Guid.NewGuid(), null, null)
            {
                ParameterType = new BooleanParameterType(Guid.NewGuid(), null, null)
            };

            this.session = new Mock<ISession>();
            this.option = new Option() { Name="OptionName" };
            this.viewModel = new ParameterOptionRowViewModel(this.parameter, this.option, this.session.Object, null);
        }

        [Test]
        public void VerifyProperties()
        {
            Assert.AreEqual(this.option, this.viewModel.ActualOption);
            Assert.AreEqual(this.option, this.viewModel.Option);
            Assert.AreEqual(ClassKind.BooleanParameterType, this.viewModel.ParameterTypeClassKind);
            Assert.IsNull(this.viewModel.ActualState);
            Assert.AreEqual(this.option.Name, this.viewModel.Name);
            Assert.IsNotNull(this.viewModel.ThingStatus);
        }
    }
}
