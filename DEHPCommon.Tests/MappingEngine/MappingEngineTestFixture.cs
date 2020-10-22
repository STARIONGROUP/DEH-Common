// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingEngineTestFixture.cs"company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.MappingEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DEHPCommon.Exceptions;
    using DEHPCommon.MappingEngine;
    using DEHPCommon.Tests.MappingEngine.Dst;

    using NUnit.Framework;

    [TestFixture]
    public class MappingEngineTestFixture
    {
        [Test]
        public void VerifyMap()
        {
            var mappinEngine = new MappingEngine(this.GetType().Assembly);
            
            var baseObject = new Cube() { Id = Guid.NewGuid(), Sides = new List<double>() {.1, .2, 2}};
            var result = mappinEngine.Map(baseObject);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Sphere>(result);
            Assert.AreEqual(.1, ((Sphere)result).Points.First().X);

            Assert.IsNull(mappinEngine.Map("0"));
            baseObject.Sides = null;
            Assert.Throws<MappingException>(() => mappinEngine.Map(baseObject));
        }

        [Test]
        public void VerifyGetBaseTypeGenericArgument()
        {
            var mappinEngine = new MappingEngine(this.GetType().Assembly);
            Assert.Throws<ArgumentException>(() => mappinEngine.GetBaseTypeGenericArgument(typeof(object), 1));
            Assert.Throws<IndexOutOfRangeException>(() => mappinEngine.GetBaseTypeGenericArgument(typeof(SphericalRule), 3));
        }
    }
}
