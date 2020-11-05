// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableReactiveListTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.Mvvm
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.EngineeringModelData;

    using DEHPCommon.Mvvm;

    [TestFixture]
    public class DisposableReactiveListTestFixture
    {
        [Test]
        public void VerifyRemove()
        {
            var list = new DisposableReactiveList<ElementDefinition>()
            {
                new ElementDefinition() { Name = "element0" },
                new ElementDefinition() { Name = "element1" },
                new ElementDefinition() { Name = "element2" },
                new ElementDefinition() { Name = "element3" },
                new ElementDefinition() { Name = "element4" },
                new ElementDefinition() { Name = "element5" },
            };

            list.RemoveAtAndDispose(1);
            Assert.AreEqual(5, list.Count);
            list.RemoveAtWithoutDispose(1);
            Assert.AreEqual(4, list.Count);

            list.RemoveRangeAndDispose(1, 1);
            Assert.AreEqual(3, list.Count);
            list.RemoveRangeWithoutDispose(1,0);
            Assert.AreEqual(3, list.Count);
            list.RemoveRangeAndDispose(1,1);
            Assert.AreEqual(2, list.Count);
            list.RemoveRangeWithoutDispose(1, 1);
            Assert.AreEqual(1, list.Count);
            
            Assert.DoesNotThrow(() => {
                list.ClearWithoutDispose();
                list.RemoveAllAndDispose(list);
                list.RemoveAllWithoutDispose(list);
            });

            Assert.AreEqual(0, list.Count);
        }
    }
}
