// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectBrowserTreeSelectorServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.EngineeringModelData;

    using DEHPCommon.Services.ObjectBrowserTreeSelectorService;

    [TestFixture]
    public class ObjectBrowserTreeSelectorServiceTestFixture
    {
        private ObjectBrowserTreeSelectorService treeSelectorService;

        [SetUp]
        public void Setup()
        {
            this.treeSelectorService = new ObjectBrowserTreeSelectorService();
        }

        [Test]
        public void VerifyProperties()
        {
            Assert.IsNotEmpty(this.treeSelectorService.AllowedTypes);
            Assert.IsNotEmpty(this.treeSelectorService.ThingKinds);
        }

        [Test]
        public void VerifyAddMethod()
        {
            Assert.Throws<InvalidCastException>(() => ((List<Type>)this.treeSelectorService.ThingKinds).Add(typeof(ObjectBrowserTreeSelectorServiceTestFixture)));
            this.treeSelectorService.Add<Parameter>();
            Assert.IsFalse(this.treeSelectorService.ThingKinds.Any(x => x == typeof(Parameter))); 
            this.treeSelectorService.Add<RequirementsSpecification>();
            Assert.Contains(typeof(RequirementsSpecification), this.treeSelectorService.ThingKinds.ToList());
        }

        [Test]
        public void VerifyRemoveMethod()
        {
            Assert.Throws<InvalidCastException>(() => ((List<Type>)this.treeSelectorService.ThingKinds).Add(typeof(ObjectBrowserTreeSelectorServiceTestFixture)));
            this.treeSelectorService.Remove<ElementDefinition>();
            Assert.IsEmpty(this.treeSelectorService.ThingKinds);
            Assert.DoesNotThrow(() => this.treeSelectorService.Remove<ElementDefinition>());
        }
    }
}
