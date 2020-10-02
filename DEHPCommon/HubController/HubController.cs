// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubController.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.HubController
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Helpers;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.DAL;
    using CDP4Dal.Operations;

    using CDP4ServicesDal;

    using CDP4WspDal;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.Services.FileDialogService;
    using DEHPCommon.UserPreferenceHandler.Enums;

    using NLog;

    using File = CDP4Common.EngineeringModelData.File;

    /// <summary>
    /// Definition of the <see cref="HubController"/>, which is responsible to provides <see cref="ISession"/> related functionnalities
    /// </summary>
    public class HubController : IHubController
    {
        /// <summary>
        /// The current class <see cref="NLog.Logger"/>
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The <see cref="IOpenSaveFileDialogService"/>
        /// </summary>
        private readonly IOpenSaveFileDialogService fileDialogService;
        
        /// <summary>
        /// Gets the <see cref="Session"/> object that is encapsulated by the current <see cref="HubController"/>.
        /// </summary>
        public ISession Session { get; set; }
        
        /// <summary>
        /// Initializes a new <see cref="HubController"/>
        /// </summary>
        /// <param name="fileDialogService">The <see cref="IOpenSaveFileDialogService"/></param>
        public HubController(IOpenSaveFileDialogService fileDialogService)
        {
            this.fileDialogService = fileDialogService;
        }

        /// <summary>
        /// Gets the <see cref="Thing"/> by its <see cref="iid"/> from the cache
        /// </summary>
        /// <typeparam name="TThing">The Type of <see cref="Thing"/> to get</typeparam>
        /// <param name="iid">The id of the <see cref="Thing"/></param>
        /// <param name="iteration"></param>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>An assert whether the <see cref="thing"/> has been found</returns>
        public bool GetThingById<TThing>(Guid iid, Iteration iteration, out TThing thing) where TThing : Thing
        {
            thing = default;

            if (this.Session.Assembler.Cache.TryGetValue(new CacheKey(iid, iteration.Iid), out var thingResult))
            {
                thing = (TThing)thingResult.Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="Thing"/> by its <see cref="iid"/> from rdls
        /// </summary>
        /// <typeparam name="TThing">The Type of <see cref="Thing"/> to get</typeparam>
        /// <param name="iid">The id of the <see cref="Thing"/></param>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>An assert whether the <see cref="thing"/> has been found</returns>
        public bool GetThingById<TThing>(Guid iid, out TThing thing) where TThing : Thing, new()
        {
            thing = new TThing();

            Func<ReferenceDataLibrary, IEnumerable<Thing>> collectionSelector = thing switch
            {
                Rule _ => x => x.QueryRulesFromChainOfRdls(),
                Category _ => x => x.QueryCategoriesFromChainOfRdls(),
                Constant _ => x => x.QueryConstantsFromChainOfRdls(),
                FileType _ => x => x.QueryFileTypesFromChainOfRdls(),
                Glossary _ => x => x.QueryGlossariesFromChainOfRdls(),
                MeasurementScale _ => x => x.QueryMeasurementScalesFromChainOfRdls(),
                MeasurementUnit _ => x => x.QueryMeasurementUnitsFromChainOfRdls(),
                ReferenceSource _ => x => x.QueryReferenceSourcesFromChainOfRdls(),
                UnitPrefix _ => x => x.QueryUnitPrefixesFromChainOfRdls(),
                QuantityKind _ => x => x.BaseQuantityKind,
                Relationship _ => x => x.QueryRelationships,
                ParameterType _ => x => x.QueryParameterTypesFromChainOfRdls(),
                _ => null
            };

            if (collectionSelector == null)
            {
                thing = default;
                return false;
            }

            thing = (TThing) this.Session?.OpenReferenceDataLibraries.SelectMany(collectionSelector).FirstOrDefault(x => x.Iid == iid);
            return thing != null;
        }

        /// <summary>
        /// Opens a session
        /// </summary>
        /// <param name="serverType">The selected <see cref="ServerType"/></param>
        /// <param name="credentials">The <see cref="Credentials"/></param>
        /// <returns>An assert whether the session is open</returns>
        public async Task<bool> Open(Credentials credentials, ServerType serverType)
        {
            IDal dal = serverType switch
            {
                ServerType.Cdp4WebServices => new CdpServicesDal(),
                ServerType.OcdtWspServer => new WspDal(),
                _ => throw new ArgumentOutOfRangeException(nameof(serverType), "Invalid Server type selected")
            };

            this.Session = new Session(dal, credentials);
            await this.Session.Open();
            return this.IsSessionOpen;
        }

        /// <summary>
        /// Checks whether the session is open by checking if
        /// the <see cref="CDP4Common.SiteDirectoryData.SiteDirectory" /> is available
        /// </summary>
        public bool IsSessionOpen => this.Session?.RetrieveSiteDirectory() != null;

        /// <summary>
        /// Closes connection to the data-source and end the execution of this app
        /// </summary>
        public void Close()
        {
            if (!this.IsSessionOpen)
            {
                this.logger.Info("At first a connection should be opened.");
                return;
            }

            try
            {
                this.Session.Close().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                this.logger.Error($"During close operation an error has occured: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the <see cref="EngineeringModelSetup"/> contained in the site directory
        /// </summary>
        public IEnumerable<EngineeringModelSetup> GetEngineeringModels() => this.GetSiteDirectory()?.Model;

        /// <summary>
        /// Retrieves the <see cref="SiteDirectory"/> that is loaded in the <see cref="ISession"/>
        /// </summary>
        /// <returns>The <see cref="SiteDirectory"/></returns>
        public SiteDirectory GetSiteDirectory() => this.Session?.RetrieveSiteDirectory();

        /// <summary>
        /// Reads an <see cref="Iteration"/> and set the active <see cref="DomainOfExpertise"/> for the Iteration
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="domain">The <see cref="DomainOfExpertise"/></param>
        /// <returns>A <see cref="Task"/></returns>
        public async Task GetIteration(Iteration iteration, DomainOfExpertise domain) => await this.Session.Read(iteration, domain);

        /// <summary>
        /// Reads an <see cref="Iteration"/> and set the active <see cref="DomainOfExpertise"/> for the Iteration
        /// </summary>
        /// <returns>A <see cref="IReadOnlyDictionary{T,T}"/> of <see cref="Iteration"/> and <see cref="Tuple{T,T}"/> of <see cref="DomainOfExpertise"/> and <see cref="Participant"/></returns>
        public IReadOnlyDictionary<Iteration, Tuple<DomainOfExpertise, Participant>> GetIteration() => this.Session.OpenIterations;

        /// <summary>
        /// Creates or updates all <see cref="Thing"/> from the provided <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="things">The <see cref="IEnumerable{T}"/> of <see cref="Thing"/></param>
        /// <param name="deep">Assert whether to create nested things</param>
        /// <returns>A <see cref="Task"/></returns>
        public async Task CreateOrUpdate(IEnumerable<Thing> things, bool deep = false)
        {
            foreach (var thing in things)
            {
                await this.CreateOrUpdate(thing, deep);
            }
        }
        
        /// <summary>
        /// Creates or updates the provided <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="deep">Assert whether to create nested things</param>
        /// <returns>A <see cref="Task"/></returns>
        public async Task CreateOrUpdate(Thing thing, bool deep = false)
        {
            await this.ExecuteThingTransactionAction((clone, transaction) => transaction.CreateOrUpdate(clone), thing);
        }

        /// <summary>
        /// Deletes all the <see cref="Thing"/> from the provided <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="things">The <see cref="Thing"/> to delete</param>
        /// <returns>A <see cref="Task"/></returns>
        public async Task Delete(IEnumerable<Thing> things)
        {
            foreach (var thing in things)
            {
                await this.Delete(thing);
            }
        }
        
        /// <summary>
        /// Deletes a <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to delete</param>
        /// <returns>A <see cref="Task"/></returns>
        public async Task Delete(Thing thing)
        {
            await this.ExecuteThingTransactionAction((clone, transaction) => transaction.Delete(clone, clone.Container), thing);
        }

        /// <summary>
        /// Creates a <see cref="ThingTransaction"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="clone">The <see cref="Thing"/> clone</param>
        /// <param name="transaction">The new <see cref="ThingTransaction"/></param>
        /// <param name="deep">An assert whether to clone things deep</param>
        private void CreateTransaction(Thing thing, out Thing clone, out ThingTransaction transaction, bool deep = false)
        {
            clone = thing.Clone(deep);
            transaction = new ThingTransaction(TransactionContextResolver.ResolveContext(thing), clone);
        }

        /// <summary>
        /// Executes the specified <see cref="ThingTransaction"/> <see cref="Action"/>
        /// </summary>
        /// <param name="thingTransactionAction">The <see cref="Action{T,T}"/> of types <see cref="Thing"/>, <see cref="ThingTransaction"/></param>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="deep">An assert whether to clone things deep</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task ExecuteThingTransactionAction(Action<Thing, ThingTransaction> thingTransactionAction, Thing thing, bool deep = false)
        {
            try
            {
                this.CreateTransaction(thing, out var clone, out var transaction, deep);
                thingTransactionAction(clone, transaction);
                await this.Session.Write(transaction.FinalizeTransaction());
            }
            catch (Exception exception)
            {
                var errorMessage = $"The {thing.Iid} has failed to {thingTransactionAction.Method.Name}, the following exception occured: {exception.Message}";
                this.logger.Error(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Generates the nested element based on the provided <see cref="Option"/>
        /// </summary>
        /// <param name="option">The <see cref="Option"/></param>
        /// <param name="domainOfExpertise">The <see cref="DomainOfExpertise"/></param>
        /// <param name="updateOption">An assert whether the <see cref="Option"/> shall be updated with the created <see cref="NestedElement"/></param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="NestedElement"/></returns>
        public IEnumerable<NestedElement> GetNestedElementTree(Option option, DomainOfExpertise domainOfExpertise, bool updateOption)
        {
            return new NestedElementTreeGenerator().Generate(option, domainOfExpertise, updateOption);
        }

        /// <summary>
        /// Upload one file to the <see cref="DomainFileStore"/> of the specified domain or of the active domain
        /// </summary>
        /// <param name="file"></param>
        /// <param name="iteration"></param>
        /// <param name="domain"></param>
        public async Task Upload(File file = null, Iteration iteration = null, DomainOfExpertise domain = null)
        {
            iteration ??= this.GetIteration().Keys.First();
            domain ??= this.Session.QueryCurrentDomainOfExpertise();
            var iDalUri = new Uri(this.Session.DataSourceUri);

            var fileStore = iteration.DomainFileStore.FirstOrDefault(x => x.Owner.Iid == domain.Iid);

            if (fileStore is null || !this.GetFile(out var filePath, out var fileName, out var extensions))
            {
                return;
            }
            
            var fileRevision = new FileRevision(Guid.NewGuid(), this.Session.Assembler.Cache, iDalUri)
            {
                CreatedOn = DateTime.UtcNow, Name = fileName, ContentHash = this.CalculateContentHash(filePath), LocalPath = filePath
            };

            fileRevision.FileType.AddRange(this.ComputeFileTypes(extensions, this.GetAllowedFileType(iteration), ref fileName));
            
            if (file is null)
            {
                file = new File(Guid.NewGuid(), this.Session.Assembler.Cache, iDalUri);
                fileStore.File.Add(file);
            }

            file.FileRevision.Add(fileRevision);
            
            var clone = fileStore.Clone(true);
            var transaction = new ThingTransaction(TransactionContextResolver.ResolveContext(fileStore), clone);
            transaction.CreateOrUpdate(fileRevision);
            transaction.CreateOrUpdate(file);

            await this.Session.Write(transaction.FinalizeTransaction(), new[] { fileName });
        }

        /// <summary>
        /// Computes all the <see cref="FileType"/> of the file that is to be uploaded
        /// </summary>
        /// <param name="extensions">The file extensions</param>
        /// <param name="allowedFileTypes">The Allowed <see cref="FileType"/></param>
        /// <param name="fileName">The name of the file that is to be uploaded</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="FileType"/></returns>
        public IEnumerable<FileType> ComputeFileTypes(string[] extensions, IEnumerable<FileType> allowedFileTypes, ref string fileName)
        {
            var fileTypes = new List<FileType>();
            allowedFileTypes = allowedFileTypes.ToList();

            for (var i = extensions.Length - 1; i >= 0; i--)
            {
                var fileType = allowedFileTypes.FirstOrDefault(x => x.Extension.ToLower().Equals(extensions[i].ToLower()));

                if (fileType == null)
                {
                    break;
                }

                fileTypes.Insert(0, fileType);
                fileName = string.Join(".", extensions.Take(i));
            }

            return fileTypes;
        }

        /// <summary>
        /// Opens the Open File Dialog and let the user select one file and returns the file Path the file Name and its extensions
        /// </summary>
        /// <param name="filePath">The File Path</param>
        /// <param name="fileName">The File Name</param>
        /// <param name="extensions">The Extensions</param>
        /// <returns>An assert whether any informations this method returns are compliant</returns>
        private bool GetFile(out string filePath, out string fileName, out string[] extensions)
        {
            filePath = null;
            fileName = null;
            extensions = null;

            var result = this.fileDialogService.GetOpenFileDialog(false, false, false, string.Empty, string.Empty, string.Empty, 1);

            if (result.Count() != 1)
            {
                return false;
            }

            filePath = result.First();
            fileName = System.IO.Path.GetFileName(filePath);

            if (fileName is null)
            {
                return false;
            }
            
            extensions = fileName.Split(new[] { "." }, StringSplitOptions.None);

            return true;
        }

        /// <summary>
        /// Calculate the Hash of the contents of some filecontent
        /// </summary>
        /// <param name="filePath">The complete path of the file</param>
        /// <returns>The Hash as <see cref="string"/></returns>
        private string CalculateContentHash(string filePath)
        {
            if (filePath == null)
            {
                return null;
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return StreamToHashComputer.CalculateSha1HashFromStream(fileStream);
            }
        }

        /// <summary>
        /// Gets the allowed file type
        /// </summary>
        /// <param name="iteration">The iteration</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="FileType"/></returns>
        private IEnumerable<FileType> GetAllowedFileType(Thing iteration)
        {
            var modelReferenceDataLibrary = iteration.GetContainerOfType<EngineeringModel>().EngineeringModelSetup.RequiredRdl.Single();

            var allowedFileTypes = new List<FileType>(modelReferenceDataLibrary.FileType);
            allowedFileTypes.AddRange(modelReferenceDataLibrary.GetRequiredRdls().SelectMany(rdl => rdl.FileType));
            return allowedFileTypes;
        }

        /// <summary>
        /// Downloads a <see cref="File.CurrentFileRevision"/>
        /// </summary>
        /// <param name="file">The <see cref="File"/></param>
        /// <returns>A <see cref="Task"/></returns>
        public async Task Download(File file)
        {
            if (file is null)
            {
                return;
            }

            await this.Download(file.CurrentFileRevision);
        }

        /// <summary>
        /// Downloads a specific <see cref="FileRevision"/>
        /// </summary>
        /// <param name="fileRevision">The <see cref="FileRevision"/></param>
        /// <returns>A <see cref="Task"/></returns>
        public async Task Download(FileRevision fileRevision)
        {
            if (fileRevision is null)
            {
                return;
            }

            var fileName = Path.GetFileNameWithoutExtension(fileRevision.Path);
            var extension = Path.GetExtension(fileRevision.Path);
            var filter = string.IsNullOrWhiteSpace(extension) ? "All files (*.*)|*.*" : $"{extension.Replace(".", "")} files|*{extension}";

            var destinationPath = this.fileDialogService.GetSaveFileDialog(fileName, extension, filter, string.Empty, 1);

            if (!string.IsNullOrWhiteSpace(destinationPath))
            {
                var fileContent = await this.Session.ReadFile(fileRevision);

                if (fileContent != null)
                {
                    System.IO.File.WriteAllBytes(destinationPath, fileContent);
                }
            }
        }
    }
}
