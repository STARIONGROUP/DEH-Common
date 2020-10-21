// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubControllerTestFixture.cs"company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.HubController
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Text;
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
    using DEHPCommon.Services.FileDialogService;
    using DEHPCommon.UserPreferenceHandler.Enums;
    
    using Moq;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using File = CDP4Common.EngineeringModelData.File;

    [TestFixture]
    public class HubControllerTestFixture
    {
        private const string FileContent = "This is file content";

        private Mock<ISession> session;
        private Participant participant;
        private DomainOfExpertise domain;
        private Iteration iteration;
        private Assembler assembler;
        private readonly Uri uri = new Uri("http://test.com");
        private BinaryRelationship relationship;
        private ConcurrentDictionary<Iteration, Tuple<DomainOfExpertise, Participant>> openIteration;
        private Mock<IOpenSaveFileDialogService> fileDialogService;
        private Expression<Func<IOpenSaveFileDialogService, string[]>> openFileDialogExpression;
        private Expression<Func<IOpenSaveFileDialogService, string>> saveFileDialogExpression;
        private HubController hubController;
        private string uploadTestFilePath;
        private string downloadTestFilePath;

        [SetUp]
        public void Setup()
        {
            this.assembler = new Assembler(this.uri);
            this.domain = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri);

            this.iteration = new Iteration(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Container = new EngineeringModel(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    EngineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), this.assembler.Cache, this.uri)
                    {
                        RequiredRdl =
                        {
                            new ModelReferenceDataLibrary(Guid.NewGuid(), this.assembler.Cache, this.uri)
                            {
                                FileType =
                                {
                                    new FileType(Guid.NewGuid(), this.assembler.Cache, this.uri) { Extension = "tar" },
                                    new FileType(Guid.NewGuid(), this.assembler.Cache, this.uri) { Extension = "gz" },
                                    new FileType(Guid.NewGuid(), this.assembler.Cache, this.uri) { Extension = "zip" }
                                }
                            }
                        }
                    },
                },
                DomainFileStore =
                {
                    new DomainFileStore(Guid.NewGuid(), this.assembler.Cache, this.uri) { Owner = this.domain }
                }
            };

            this.participant = new Participant(Guid.NewGuid(), this.assembler.Cache, this.uri);

            this.session = new Mock<ISession>();

            this.session.Setup(x => x.Assembler).Returns(this.assembler);

            this.openIteration = new ConcurrentDictionary<Iteration, Tuple<DomainOfExpertise, Participant>>(
                new List<KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>>()
                {
                    new KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>(this.iteration, new Tuple<DomainOfExpertise, Participant>(this.domain, this.participant))
                });

            this.session.Setup(x => x.OpenIterations).Returns(this.openIteration);

            this.session.Setup(x => x.Write(It.IsAny<OperationContainer>())).Returns(Task.CompletedTask);
            this.session.Setup(x => x.Write(It.IsAny<OperationContainer>(), It.IsAny<IEnumerable<string>>())).Returns(Task.CompletedTask);
            this.session.Setup(x => x.ReadFile(It.IsAny<FileRevision>())).Returns(Task.FromResult(new byte[] { }));

            this.session.Setup(x => x.Close()).Returns(Task.CompletedTask);

            this.session.Setup(x => x.Open()).Returns(Task.CompletedTask);

            this.session.Setup(x => x.QueryCurrentDomainOfExpertise()).Returns(this.domain);
            this.session.Setup(x => x.DataSourceUri).Returns(this.uri.AbsoluteUri);

            this.relationship = new BinaryRelationship(Guid.NewGuid(), this.assembler.Cache, this.uri);

            this.session.Setup(x => x.OpenReferenceDataLibraries).Returns(new List<ReferenceDataLibrary>()
                {
                    new SiteReferenceDataLibrary(Guid.NewGuid(), this.assembler.Cache, this.uri)
                }
            );

            this.fileDialogService = new Mock<IOpenSaveFileDialogService>();
            this.openFileDialogExpression = x => x.GetOpenFileDialog(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
            this.saveFileDialogExpression = x => x.GetSaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());

            this.uploadTestFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, nameof(HubController), "file.tar.gz");
            this.downloadTestFilePath = "file.tar.gz";
            this.fileDialogService.Setup(this.openFileDialogExpression).Returns(new[] { this.uploadTestFilePath });
            this.fileDialogService.Setup(this.saveFileDialogExpression).Returns(this.downloadTestFilePath);

            this.hubController = new HubController(this.fileDialogService.Object) { Session = this.session.Object };
        }

        [TearDown]
        public void TakeDown()
        {
            CDPMessageBus.Current.ClearSubscriptions();
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

            await this.hubController.CreateOrUpdate(thingsToWrite);
            elementDefinition.Parameter.Add(parameter);
            await this.hubController.CreateOrUpdate(elementDefinition, true);

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

            Assert.ThrowsAsync<InvalidOperationException>(async () => await this.hubController.Delete(thingsToDelete));

            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>()), Times.Never);
        }

        [Test]
        public void VerifyIsSessionOpen()
        {
            Assert.IsFalse(this.hubController.IsSessionOpen);
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(new SiteDirectory());
            Assert.IsTrue(this.hubController.IsSessionOpen);
        }

        [Test]
        public void VerifySessionOpen()
        {
            Assert.IsFalse(this.hubController.IsSessionOpen);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.hubController.Open(new Credentials("admin", "pass", this.uri), (ServerType) 32));
            Assert.ThrowsAsync<HeaderException>(async () => await this.hubController.Open(new Credentials("admin", "pass", this.uri), ServerType.Cdp4WebServices));
            Assert.ThrowsAsync<JsonReaderException>(async () => await this.hubController.Open(new Credentials("admin", "poss", this.uri), ServerType.OcdtWspServer));
            Assert.IsFalse(this.hubController.IsSessionOpen);
        }

        [Test]
        public void VerifyGetThingFromCache()
        {
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = this.iteration };
            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = elementDefinition };
            this.assembler.Cache.TryAdd(new CacheKey(elementDefinition.Iid, this.iteration.Iid), new Lazy<Thing>(() => elementDefinition));

            Assert.IsTrue(this.hubController.GetThingById(elementDefinition.Iid, this.iteration, out ElementDefinition cachedElementDefinition));
            Assert.AreSame(elementDefinition, cachedElementDefinition);
            Assert.IsFalse(this.hubController.GetThingById(parameter.Iid, this.iteration, out ElementDefinition cachedParameter));
            Assert.IsNull(cachedParameter);

            Assert.IsFalse(this.hubController.GetThingById(parameter.Iid, out Parameter _));

            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out BinaryRelationship cachedRelationship));
            Assert.IsNull(cachedRelationship);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out BinaryRelationshipRule binaryRelationshipRule));
            Assert.IsNull(binaryRelationshipRule);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out Category category));
            Assert.IsNull(category);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out Glossary glossary));
            Assert.IsNull(glossary);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out SimpleUnit simpleUnit));
            Assert.IsNull(simpleUnit);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out UnitPrefix unitPrefix));
            Assert.IsNull(unitPrefix);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out SimpleQuantityKind quantityKind));
            Assert.IsNull(quantityKind);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out BooleanParameterType parameterType));
            Assert.IsNull(parameterType);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out Constant constant));
            Assert.IsNull(constant);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out FileType fileType));
            Assert.IsNull(fileType);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out IntervalScale intervalScale));
            Assert.IsNull(intervalScale);
            Assert.IsFalse(this.hubController.GetThingById(this.relationship.Iid, out ReferenceSource referenceSource));
            Assert.IsNull(referenceSource);
        }

        [Test]
        public void VerifyClose()
        {
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(default(SiteDirectory));
            Assert.DoesNotThrow(() => this.hubController.Close());
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(new SiteDirectory());
            this.session.Setup(x => x.Close()).Returns(Task.CompletedTask);
            Assert.DoesNotThrow(() => this.hubController.Close());
            this.session.Setup(x => x.Close()).Throws<Exception>();
            Assert.DoesNotThrow(() => this.hubController.Close());
        }

        [Test]
        public void VerifyGetIteration()
        {
            Assert.AreSame(this.openIteration, this.hubController.GetIteration());
            this.session.Setup(x => x.OpenIterations).Returns(default(IReadOnlyDictionary<Iteration, Tuple<DomainOfExpertise, Participant>>));
            Assert.IsNull(this.hubController.GetIteration());
            this.session.Setup(x => x.Read(It.IsAny<Iteration>(), It.IsAny<DomainOfExpertise>())).Returns(Task.CompletedTask);
            Assert.DoesNotThrowAsync(async () => await this.hubController.GetIteration(this.iteration, this.domain));
        }

        [Test]
        public void VerifyGetEngineeringModel()
        {
            var siteDirectory = new SiteDirectory() { Model = { new EngineeringModelSetup(Guid.NewGuid(), this.assembler.Cache, this.uri) } };
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(siteDirectory);
            Assert.AreSame(siteDirectory.Model, this.hubController.GetEngineeringModels());
            this.session.Setup(x => x.RetrieveSiteDirectory()).Returns(default(SiteDirectory));
            Assert.DoesNotThrow(() => this.hubController.GetEngineeringModels());
        }

        [Test]
        public void VerifyUpload()
        {
            Assert.DoesNotThrowAsync(async () => await this.hubController.Upload());

            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>(), It.IsAny<IEnumerable<string>>()), Times.Once);

            this.fileDialogService.Verify(this.openFileDialogExpression, Times.Once);
        }

        [Test]
        public void VerifyDownload()
        {
            this.session.Setup(x => x.ReadFile(It.IsAny<FileRevision>())).ReturnsAsync(Encoding.ASCII.GetBytes(FileContent));

            var fileRevision = new FileRevision(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Name = "file.tar.gz",
                Creator = new Participant(),
                ContainingFolder = new Folder(),
                CreatedOn = DateTime.UtcNow.AddHours(-1),
                ContentHash = "contenthash"
            };

            fileRevision.FileType.Add(new FileType());
            
            var file = new File(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                FileRevision = { fileRevision }
            };

            Assert.DoesNotThrowAsync(async () => await this.hubController.Download(default(File)));
            Assert.DoesNotThrowAsync(async () => await this.hubController.Download(default(FileRevision)));

            Assert.DoesNotThrowAsync(async () => await this.hubController.Download(file));
            var result = System.IO.File.ReadAllBytes(this.downloadTestFilePath);
            System.IO.File.Delete(this.downloadTestFilePath);
            Assert.AreEqual(FileContent, Encoding.ASCII.GetString(result));

            Assert.IsFalse(System.IO.File.Exists(this.downloadTestFilePath));

            Assert.DoesNotThrowAsync(async () => await this.hubController.Download(fileRevision));
            result = System.IO.File.ReadAllBytes(this.downloadTestFilePath);
            System.IO.File.Delete(this.downloadTestFilePath);
            Assert.AreEqual(FileContent, Encoding.ASCII.GetString(result));
            
            this.session.Verify(x => x.ReadFile(It.IsAny<FileRevision>()), Times.Exactly(2));

            this.fileDialogService.Verify(this.saveFileDialogExpression, Times.Exactly(2));
        }
    }
}
