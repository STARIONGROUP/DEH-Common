// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingEngineTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.MappingEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
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
        }
    }
}
