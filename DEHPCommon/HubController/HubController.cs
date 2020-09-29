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
    using DEHPCommon.UserPreferenceHandler.Enums;

    using NLog;

    /// <summary>
    /// Definition of the <see cref="HubController"/>
    /// </summary>
    public class HubController : IHubController
    {
        /// <summary>
        /// The current class <see cref="NLog.Logger"/>
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the <see cref="Session"/> object that is encapsulated by the current <see cref="HubController"/>.
        /// </summary>
        public ISession Session { get; set; }

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
    }
}
