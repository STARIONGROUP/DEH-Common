// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelRowViewModelTestFixture.cs"company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.UserInterfaces.Rows
{
    using System;

    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.UserInterfaces.ViewModels.Rows;

    using NUnit.Framework;

    [TestFixture]
    public class EngineeringModelRowViewModelTestFixture
    {
        [Test]
        public void VerifyProperties()
        {
            var model = new EngineeringModelSetup(Guid.NewGuid(), null, null);

            var row = new EngineeringModelRowViewModel(model);

            Assert.AreSame(row.Thing, model);
            Assert.AreEqual(row.Iid, model.Iid);
            Assert.AreSame(row.Name, model.Name);
            Assert.AreSame(row.ShortName, model.ShortName);
            Assert.AreEqual(row.RevisionNumber, model.RevisionNumber);
            Assert.AreEqual(row.Kind, model.Kind);
            Assert.IsTrue(row.IsSelected);
        }
    }
}
