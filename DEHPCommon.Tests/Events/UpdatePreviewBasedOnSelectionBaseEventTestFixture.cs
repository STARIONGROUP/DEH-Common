// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePreviewBasedOnSelectionBaseEventTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.Events
{
    using System.Collections.Generic;

    using DEHPCommon.Events;
    using DEHPCommon.UserInterfaces.ViewModels;
    using DEHPCommon.UserInterfaces.ViewModels.NetChangePreview;
    using DEHPCommon.UserInterfaces.ViewModels.NetChangePreview.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    [TestFixture]
    public class UpdatePreviewBasedOnSelectionBaseEventTestFixture
    {
        private class TestUpdatePreview : UpdatePreviewBasedOnSelectionBaseEvent<IValueSetRow, INetChangePreviewViewModel>
        {
            /// <summary>
            /// Initializes a new <see cref="UpdatePreviewBasedOnSelectionBaseEvent{T,T}"/>
            /// </summary>
            /// <param name="things">The collection of <see cref="IValueSetRow"/> selection</param>
            /// <param name="target">The target <see cref="System.Type"/></param>
            /// <param name="reset">a value indicating whether the listener should reset its tree</param>
            public TestUpdatePreview(IEnumerable<IValueSetRow> things, INetChangePreviewViewModel target, bool reset) : base(things, target, reset)
            {
            }
        }
        
        [Test]
        public void VerifyProperty()
        {
            var expected = new List<ParameterOrOverrideBaseRowViewModel>();
            Assert.AreSame(expected, new TestUpdatePreview(expected, null, false).Selection);
            Assert.AreEqual(false, new TestUpdatePreview(expected, null, false).Reset);
            Assert.AreSame(null, new TestUpdatePreview(expected, null, false).Target);
        }
    }
}
