// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectTypeTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.MappingEngine
{
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.MappingRules.Core;

    using NUnit.Framework;

    [TestFixture]
    public class ObjectTypeTestFixture
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
        public void VerifySetProperty()
        {
            var baseObject = new RandomDstObject() { RandomProperty = new RandomNestedDstObject() { RandomName = Name } };

            dynamic instance = new ObjectType(((object)baseObject), new IMappingRule[]
            {
                new ObjectProperty("RandomProperty", typeof(EngineeringModelSetup), new IMappingRule[]
                {
                    new ObjectProperty("RandomName", typeof(string), nameof(EngineeringModelSetup.Name))
                })
            });

            Assert.IsNotNull(instance);
            Assert.AreEqual(Name, instance.RandomProperty.RandomName);
        }
    }
}
