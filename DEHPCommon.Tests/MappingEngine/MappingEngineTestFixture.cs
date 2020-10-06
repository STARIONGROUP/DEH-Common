// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingEngineTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.MappingEngine
{
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.MappingEngine;
    using DEHPCommon.MappingRules.Core;

    using NUnit.Framework;

    [TestFixture]
    public class MappingEngineTestFixture
    {
        private const string Name = "TheName";

        public class RandomDstObject
        {
            public RandomNestedDstObject RandomProperty { get; set; }
        }

        public class RandomNestedDstObject
        {
            public string RandomName { get; set; }
        }
            
        [Test]
        public void VerifyMap()
        {
            var mappinEngine = new MappingEngine();
            var baseObject = new RandomDstObject() { RandomProperty = new RandomNestedDstObject() { RandomName = Name } };

            var result = mappinEngine.MapForward(baseObject, new ObjectProperty("RandomProperty", typeof(EngineeringModelSetup), new IMappingRule[]
            {
                new ObjectProperty("RandomName", typeof(string), nameof(EngineeringModelSetup.Name))
            }));

            Assert.IsNotNull(result);
            Assert.AreEqual(Name, result.EngineeringModelSetup.Name);
        }
    }
}
