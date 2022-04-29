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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
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
        private Person person;
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
        private ModelReferenceDataLibrary referenceDataLibrary;
        private EngineeringModelSetup engineeringSetup;

        [SetUp]
        public void Setup()
        {
            this.assembler = new Assembler(this.uri);
            this.domain = new DomainOfExpertise(Guid.NewGuid(), this.assembler.Cache, this.uri);

            this.person = new Person(Guid.NewGuid(), this.assembler.Cache, this.uri);

            this.participant = new Participant(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Person = this.person
            };

            this.referenceDataLibrary = new ModelReferenceDataLibrary(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                ShortName = "ARDL"
            };

            this.engineeringSetup = new EngineeringModelSetup(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                RequiredRdl =
                {
                    this.referenceDataLibrary
                },
                Participant = { this.participant }
            };

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
                        },
                        Participant = { this.participant }
                    }
                },
                IterationSetup = new IterationSetup(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Container = this.engineeringSetup
                },
                DomainFileStore =
                {
                    new DomainFileStore(Guid.NewGuid(), this.assembler.Cache, this.uri) { Owner = this.domain }
                }
            };

            this.session = new Mock<ISession>();

            this.session.Setup(x => x.Assembler).Returns(this.assembler);

            this.openIteration = new ConcurrentDictionary<Iteration, Tuple<DomainOfExpertise, Participant>>(
                new List<KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>>()
                {
                    new KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>(this.iteration, new Tuple<DomainOfExpertise, Participant>(this.domain, this.participant))
                });

            this.session.Setup(x => x.OpenIterations).Returns(this.openIteration);

            this.session.Setup(x => x.ActivePerson).Returns(this.person);

            this.session.Setup(x => x.Write(It.IsAny<OperationContainer>())).Returns(Task.CompletedTask);
            this.session.Setup(x => x.Write(It.IsAny<OperationContainer>(), It.IsAny<IEnumerable<string>>())).Returns(Task.CompletedTask);
            this.session.Setup(x => x.ReadFile(It.IsAny<FileRevision>())).Returns(Task.FromResult(new byte[] { }));

            this.session.Setup(x => x.Close()).Returns(Task.CompletedTask);

            this.session.Setup(x => x.Open()).Returns(Task.CompletedTask);

            this.session.Setup(x => x.DataSourceUri).Returns(this.uri.AbsoluteUri);
            this.session.Setup(x => x.Reload()).Returns(Task.CompletedTask);
            this.session.Setup(x => x.Refresh()).Returns(Task.CompletedTask);

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
        public void VerifyProperties()
        {
            Assert.IsFalse(this.hubController.IsSessionOpen);
            Assert.IsNull(this.hubController.OpenIteration);
            Assert.IsNotNull(this.hubController.Session);
            Assert.IsNull(this.hubController.CurrentDomainOfExpertise);
        }

        [Test]
        public async Task VerifyCreateOrUpdate()
        {
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = this.iteration };

            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = elementDefinition };
            var parameter2 = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = elementDefinition };

            var thingsToWrite = new List<Parameter>()
            {
                parameter, parameter2
            };

            await this.hubController.CreateOrUpdate<ElementDefinition, Parameter>(thingsToWrite, (e, p) => e.Parameter.Add(p));
            this.session.Setup(x => x.Write(It.IsAny<OperationContainer>())).Throws<InvalidCastException>();
            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>()), Times.Exactly(2));

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    await this.hubController.CreateOrUpdate<ElementDefinition, Parameter>(thingsToWrite, (e, p) => e.Parameter.Add(p))
                );
        }

        [Test]
        public void VerifyWriteTransaction()
        {
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = this.iteration };
            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = elementDefinition };
            var elementClone = elementDefinition.Clone(false);
            var transaction = new ThingTransaction(TransactionContextResolver.ResolveContext(elementClone), elementClone);
            transaction.Create(parameter, elementClone);

            elementClone.Parameter.Add(parameter);
            Assert.DoesNotThrowAsync(async () => await this.hubController.Write(transaction));
            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>()), Times.Once);
        }

        [Test]
        public void VerifyDelete()
        {
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = this.iteration };
            var parameter = new Parameter(Guid.NewGuid(), this.assembler.Cache, this.uri) { Container = elementDefinition };

            var thingsToDelete = new List<Parameter>()
            {
                parameter, parameter
            };

            Assert.DoesNotThrowAsync(
                async () => await this.hubController.Delete<ElementDefinition, Parameter>(
                    thingsToDelete, (e, p) => e.Parameter.Remove(p)));

            Assert.DoesNotThrowAsync(
                async () => await this.hubController.Delete<ElementDefinition, Parameter>(
                    new List<Parameter>(), null));

            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>()), Times.Exactly(2));
        }

        [Test]
        public void VerifySessionOpen()
        {
            Assert.IsFalse(this.hubController.IsSessionOpen);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await this.hubController.Open(new Credentials("admin", "pass", this.uri), (ServerType) 32));
            Assert.CatchAsync(async () => await this.hubController.Open(new Credentials("admin", "pass", this.uri), ServerType.Cdp4WebServices));
            Assert.CatchAsync(async () => await this.hubController.Open(new Credentials("admin", "poss", this.uri), ServerType.OcdtWspServer));
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
            this.hubController.IsSessionOpen = true;
            this.session.Setup(x => x.Close()).Returns(Task.CompletedTask);
            Assert.DoesNotThrow(() => this.hubController.Close());
            this.session.Setup(x => x.Close()).Throws<Exception>();
            Assert.DoesNotThrow(() => this.hubController.Close());
            this.hubController.IsSessionOpen = true;
            Assert.DoesNotThrow(() => this.hubController.Close());
        }

        [Test]
        public void VerifyGetIteration()
        {
            Assert.AreSame(this.openIteration, this.hubController.GetIteration());
            this.session.Setup(x => x.OpenIterations).Returns(default(IReadOnlyDictionary<Iteration, Tuple<DomainOfExpertise, Participant>>));
            Assert.IsNull(this.hubController.GetIteration());
            this.session.Setup(x => x.Read(It.IsAny<Iteration>(), It.IsAny<DomainOfExpertise>())).Returns(Task.CompletedTask);

            this.session.Setup(x => x.OpenIterations).Returns(new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>()
            {
                { this.iteration, new Tuple<DomainOfExpertise, Participant>(this.domain, this.participant)}

            });

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
            this.hubController.OpenIteration = this.iteration;
            this.hubController.CurrentDomainOfExpertise = this.domain;

            Assert.DoesNotThrowAsync(async () => await this.hubController.Upload());

            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>(), It.IsAny<IEnumerable<string>>()), Times.Once);

            this.fileDialogService.Verify(this.openFileDialogExpression, Times.Once);
        }

        [Test]
        public void VerifyUploadFromPath()
        {
            this.hubController.OpenIteration = this.iteration;
            this.hubController.CurrentDomainOfExpertise = this.domain;

            Assert.DoesNotThrowAsync(async () => await this.hubController.Upload(this.uploadTestFilePath));

            this.session.Verify(x => x.Write(It.IsAny<OperationContainer>(), It.IsAny<IEnumerable<string>>()), Times.Once);

            this.fileDialogService.Verify(this.openFileDialogExpression, Times.Never);
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

        [Test]
        public void VerifyDownloadFileStream()
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

            var destinationFile = new FileStream(this.downloadTestFilePath, FileMode.Create);

            Assert.DoesNotThrowAsync(async () => await this.hubController.Download(default(File), destinationFile));
            Assert.DoesNotThrowAsync(async () => await this.hubController.Download(default(FileRevision), destinationFile));

            Assert.DoesNotThrowAsync(async () => await this.hubController.Download(file, destinationFile));
            destinationFile.Close();

            var result = System.IO.File.ReadAllBytes(this.downloadTestFilePath);
            System.IO.File.Delete(this.downloadTestFilePath);

            Assert.AreEqual(FileContent, Encoding.ASCII.GetString(result));
            Assert.IsFalse(System.IO.File.Exists(this.downloadTestFilePath));

            destinationFile = new FileStream(this.downloadTestFilePath, FileMode.Create);
            Assert.DoesNotThrowAsync(async () => await this.hubController.Download(fileRevision, destinationFile));
            destinationFile.Close();

            result = System.IO.File.ReadAllBytes(this.downloadTestFilePath);
            System.IO.File.Delete(this.downloadTestFilePath);

            Assert.AreEqual(FileContent, Encoding.ASCII.GetString(result));
            Assert.IsFalse(System.IO.File.Exists(this.downloadTestFilePath));

            this.session.Verify(x => x.ReadFile(It.IsAny<FileRevision>()), Times.Exactly(2));

            this.fileDialogService.Verify(this.saveFileDialogExpression, Times.Never);
        }

        [Test]
        public void VerifyExternalIdentifierMap()
        {
            var toolName = "tool";

            this.iteration.ExternalIdentifierMap.AddRange(new List<ExternalIdentifierMap>()
            {
                new ExternalIdentifierMap() { ExternalToolName = toolName},
                new ExternalIdentifierMap() { ExternalToolName = string.Empty}
            });

            this.hubController.OpenIteration = this.iteration;

            Assert.AreEqual(1, this.hubController.AvailableExternalIdentifierMap(toolName).Count());
            Assert.Zero(this.hubController.AvailableExternalIdentifierMap(null).Count());
        }

        [Test]
        public void VerifyReloadRefresh()
        {
            Assert.DoesNotThrowAsync(() => this.hubController.Refresh());
            Assert.DoesNotThrowAsync(() => this.hubController.Reload());
            this.session.Verify(x => x.Reload(), Times.Once);
            this.session.Verify(x => x.Refresh(), Times.Once);
        }

        [Test]
        public void VerifyRegisterNewLogEntryToTransaction()
        {
            this.assembler.Cache.TryAdd(new CacheKey(this.iteration.Iid, null), new Lazy<Thing>(() => this.iteration));

            var model = (EngineeringModel)this.iteration.Container;
            this.assembler.Cache.TryAdd(new CacheKey(model.Iid, null), new Lazy<Thing>(() => this.iteration.Container));

            var iterationClone = this.iteration.Clone(false);
            var transaction = new ThingTransaction(TransactionContextResolver.ResolveContext(iterationClone), iterationClone);

            this.hubController.OpenIteration = this.iteration;
            this.hubController.CurrentDomainOfExpertise = this.domain;

            Assert.Throws<ArgumentNullException>(() => this.hubController.RegisterNewLogEntryToTransaction("Dummy justification", null));

            Assert.DoesNotThrow(() => this.hubController.RegisterNewLogEntryToTransaction(null, transaction));
            Assert.AreEqual(0, transaction.AddedThing.Count());

            Assert.DoesNotThrow(() => this.hubController.RegisterNewLogEntryToTransaction("Dummy justification", transaction));

            Assert.AreEqual(1, transaction.AddedThing.Count());
            Assert.AreEqual(2, transaction.UpdatedThing.Count);

            var addedModelLogEntry = (ModelLogEntry)transaction.AddedThing.SingleOrDefault();
            Assert.NotNull(addedModelLogEntry);
            Assert.AreEqual("en-GB", addedModelLogEntry.LanguageCode);
            Assert.AreEqual("Dummy justification", addedModelLogEntry.Content);
            Assert.AreEqual(LogLevelKind.USER, addedModelLogEntry.Level);
            Assert.AreEqual(this.person, addedModelLogEntry.Author);
        }

        [Test]
        public void VerifyGetDehpOrModelReferenceDataLibrary()
        {
            this.hubController.OpenIteration = this.iteration;
            Assert.AreEqual(this.referenceDataLibrary, this.hubController.GetDehpOrModelReferenceDataLibrary());
            var dehpRdl = new ModelReferenceDataLibrary() { ShortName = "DeHpRDL" };

            this.engineeringSetup = new EngineeringModelSetup(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                RequiredRdl =
                {
                    dehpRdl
                },
                Participant = { this.participant }
            };

            this.iteration = new Iteration(Guid.NewGuid(), this.assembler.Cache, this.uri)
            {
                Container = new EngineeringModel(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    EngineeringModelSetup = this.engineeringSetup,
                },
                IterationSetup = new IterationSetup(Guid.NewGuid(), this.assembler.Cache, this.uri)
                {
                    Container = this.engineeringSetup
                }
            };

            this.hubController.OpenIteration = this.iteration;

            Assert.AreEqual(dehpRdl, this.hubController.GetDehpOrModelReferenceDataLibrary());
        }

        [Test]
        public void VerifyRefreshReferenceDataLibrary()
        {
            Assert.DoesNotThrowAsync(async () => await this.hubController.RefreshReferenceDataLibrary(this.referenceDataLibrary));
            this.session.Verify(x => x.Read(It.IsAny<ReferenceDataLibrary>()), Times.Once);
        }

        [Test]
        public void VerifyTryGetThingBy()
        {
            this.hubController.OpenIteration = this.iteration;

            var category = new Category()
            {
                Iid = Guid.NewGuid(),
                ShortName = "acategory"
            };

            this.referenceDataLibrary.DefinedCategory.Add(category);

            Assert.IsTrue(this.hubController.TryGetThingById(category.Iid, ClassKind.Category, out Thing thing));
            Assert.AreEqual(category, thing);
            Assert.IsTrue(this.hubController.TryGetThingById(category.Iid, ClassKind.Category, out Category categoryRetrieved));
            Assert.AreEqual(category, categoryRetrieved);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.Rule, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.Constant, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.FileType, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.Glossary, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.MeasurementUnit, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.MeasurementScale, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.ReferenceSource, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.UnitPrefix, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.QuantityKind, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.Relationship, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.ParameterType, out thing));
            Assert.IsNull(thing);
            Assert.IsFalse(this.hubController.TryGetThingById(category.Iid, ClassKind.Parameter, out thing));
            Assert.IsNull(thing);
        }
    }
}
