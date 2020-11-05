// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementUsageTypeToVisibilityConverterTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;

    using CDP4Common.EngineeringModelData;

    using CDP4Dal;
    using CDP4Dal.Permission;

    using DEHPCommon.Converters;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Converts the type of row view model coming in into visibility
    /// </summary>
    [TestFixture]
    public class ElementUsageTypeToVisibilityConverterTestFixture
    {
        private ElementUsageTypeToVisibilityConverter converter;
        private Mock<ISession> session;

        [SetUp]
        public void SetUp()
        {
            this.converter = new ElementUsageTypeToVisibilityConverter();
            this.session = new Mock<ISession>();
            this.session.Setup(x => x.PermissionService).Returns(new Mock<IPermissionService>().Object);
        }

        [Test]
        public void VerifyThatConvertReturnsExpectedResult()
        {
            Assert.AreEqual(Visibility.Collapsed, this.converter.Convert(null, null, null, null));
            var thing = new ElementUsage() { ElementDefinition = new ElementDefinition(), Container = new ElementDefinition() };
            thing.Container.Container = new Iteration();
            var row = new ElementUsageRowViewModel(thing, null, this.session.Object, null);
            Assert.AreEqual(Visibility.Visible, this.converter.Convert(row, null, null, null));
            Assert.AreEqual(Visibility.Collapsed, this.converter.Convert(string.Empty, null, null, null));
        }

        [Test]
        public void VerifyThatConvertBackIsNotSupported()
        {
            Assert.Throws<NotSupportedException>(() => this.converter.ConvertBack(null, null, null, null));
        }
    }
}
