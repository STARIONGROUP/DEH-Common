// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubControllerTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.HubController
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.DAL;
    using CDP4Dal.Exceptions;
    using CDP4Dal.Operations;

    using DEHPCommon.HubController;
    using DEHPCommon.UserPreferenceHandler.Enums;
    
    using Moq;

    using Newtonsoft.Json;

    using NUnit.Framework;
    
    [TestFixture]
    public class HubControllerTestFixture
    {
        private Mock<ISession> session;
        private Participant participant;
        private DomainOfExpertise domain;
        private Iteration iteration;
        private Assembler assembler;
        private readonly Uri uri = new Uri("http://test.com");
        private BinaryRelationship relationship;
        private ConcurrentDictionary<Iteration, Tuple<DomainOfExpertise, Participant>> openIteration;

        [SetUp]
        public void Setup()
        {
            this.assembler = new Assembler(this.uri);

            this.iteration = new Iteration(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Container = new EngineeringModel(Guid.NewGuid(), this.assembler.Cache, this.uri)
            };

            this.domain = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri);
            this.participant = new Participant(Guid.NewGuid(), this.assembler.Cache, this.uri);

            this.session = new Mock<ISession>();

            this.session.Setup(x => x.Assembler).Returns(this.assembler);

            this.openIteration = new ConcurrentDictionary<Iteration, Tuple<DomainOfExpertise, Participant>>(
                new List<KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>>()
                {
                    new KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>(this.iteration, new Tuple<DomainOfExpertise, Participant>(this.domain, this.participant))
                });

            this.session.Setup(x => x.OpenIterations).Returns(openIteration);
            
            this.session.Setup(x => x.Write(It.IsAny<OperationContainer>())).Returns(Task.CompletedTask);

            this.session.Setup(x => x.Close()).Returns(Task.CompletedTask);

            this.session.Setup(x => x.Open()).Returns(Task.CompletedTask);

            this.relationship = new BinaryRelationship(Guid.NewGuid(), this.assembler.Cache, this.uri);

            this.session.Setup(x => x.OpenReferenceDataLibraries).Returns(new List<ReferenceDataLibrary>()
                {
                    new SiteReferenceDataLibrary(Guid.NewGuid(), this.assembler.Cache, this.uri)
                }
            );
        }

        [Test]
        public async Task VerifyCreateOrUpdate()
        {
            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = this.iteration };
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = this.iteration };

            var thingsToWrite = new List<Thing>()
            {
                parameter, elementDefinition
            };

            var hubController = new HubController() { Session = this.session.Object };

            await hubController.CreateOrUpdate(thingsToWrite);
            elementDefinition.Parameter.Add(parameter);
            await hubController.CreateOrUpdate(elementDefinition, true);

            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>()), Times.Exactly(3));
        }

        [Test]
        public void VerifyDelete()
        {
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = this.iteration };
            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = elementDefinition };
            
            var thingsToDelete = new List<Thing>()
            {
                parameter
            };

            var hubController = new HubController() { Session = this.session.Object };
            Assert.ThrowsAsync<InvalidOperationException>(async () => await hubController.Delete(thingsToDelete));
            
            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>()), Times.Never);
        }

        [Test]
        public void VerifyIsSessionOpen()
        {
            var hubController = new HubController() { Session = this.session.Object };
            Assert.IsFalse(hubController.IsSessionOpen);
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(new SiteDirectory());
            Assert.IsTrue(hubController.IsSessionOpen);
        }
        
        [Test]
        public void VerifySessionOpen()
        {
            var hubController = new HubController() { Session = this.session.Object };
            Assert.IsFalse(hubController.IsSessionOpen);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await hubController.Open(new Credentials("admin", "pass", this.uri), (ServerType)32));
            Assert.ThrowsAsync<DalReadException>(async () => await hubController.Open(new Credentials("admin", "pass", this.uri), ServerType.Cdp4WebServices));
            Assert.ThrowsAsync<DalReadException>(async () => await hubController.Open(new Credentials("admin", "poss", this.uri), ServerType.OcdtWspServer));
            Assert.IsFalse(hubController.IsSessionOpen);
        }

        [Test]
        public void VerifyGetThingFromCache()
        {
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = this.iteration };
            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = elementDefinition };
            this.assembler.Cache.TryAdd(new CacheKey(elementDefinition.Iid, this.iteration.Iid), new Lazy<Thing>(() => elementDefinition));

            var hubController = new HubController() { Session = this.session.Object };
            Assert.IsTrue(hubController.GetThingById(elementDefinition.Iid, this.iteration, out ElementDefinition cachedElementDefinition));
            Assert.AreSame(elementDefinition, cachedElementDefinition);
            Assert.IsFalse(hubController.GetThingById(parameter.Iid, this.iteration, out ElementDefinition cachedParameter));
            Assert.IsNull(cachedParameter);

            Assert.IsFalse(hubController.GetThingById(parameter.Iid, out Parameter _));

            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out BinaryRelationship cachedRelationship));
            Assert.IsNull(cachedRelationship);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out BinaryRelationshipRule binaryRelationshipRule));
            Assert.IsNull(binaryRelationshipRule);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out Category category));
            Assert.IsNull(category);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out Glossary glossary));
            Assert.IsNull(glossary);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out SimpleUnit simpleUnit));
            Assert.IsNull(simpleUnit);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out UnitPrefix unitPrefix));
            Assert.IsNull(unitPrefix);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out SimpleQuantityKind quantityKind));
            Assert.IsNull(quantityKind);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out BooleanParameterType parameterType));
            Assert.IsNull(parameterType);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out Constant constant));
            Assert.IsNull(constant);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out FileType fileType));
            Assert.IsNull(fileType);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out IntervalScale intervalScale));
            Assert.IsNull(intervalScale);
            Assert.IsFalse(hubController.GetThingById(this.relationship.Iid, out ReferenceSource referenceSource));
            Assert.IsNull(referenceSource);
        }

        [Test]
        public void VerifyClose()
        {
            var hubController = new HubController() { Session = this.session.Object };
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(default(SiteDirectory));
            Assert.DoesNotThrow(() => hubController.Close());
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(new SiteDirectory());
            this.session.Setup(x => x.Close()).Returns(Task.CompletedTask);
            Assert.DoesNotThrow(() => hubController.Close());
            this.session.Setup(x => x.Close()).Throws<Exception>();
            Assert.DoesNotThrow(() => hubController.Close());
        }

        [Test]
        public void VerifyGetIteration()
        {
            var hubController = new HubController() { Session = this.session.Object };
            Assert.AreSame(this.openIteration,hubController.GetIteration());
            this.session.Setup(x => x.OpenIterations).Returns(default(IReadOnlyDictionary<Iteration, Tuple<DomainOfExpertise, Participant>>));
            Assert.IsNull(hubController.GetIteration());
            this.session.Setup(x => x.Read(It.IsAny<Iteration>(), It.IsAny<DomainOfExpertise>())).Returns(Task.CompletedTask);
            Assert.DoesNotThrowAsync(async () => await hubController.GetIteration(this.iteration, this.domain));
        }

        [Test]
        public void VerifyGetEngineeringModel()
        {
            var hubController = new HubController() { Session = this.session.Object };
            var siteDirectory = new SiteDirectory() { Model = { new EngineeringModelSetup(Guid.NewGuid(), this.assembler.Cache, this.uri) }};
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(siteDirectory);
            Assert.AreSame(siteDirectory.Model, hubController.GetEngineeringModels());
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(default(SiteDirectory));
            Assert.DoesNotThrow(() => hubController.GetEngineeringModels());
        }
    }
}
