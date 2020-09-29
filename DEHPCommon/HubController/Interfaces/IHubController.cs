// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHubController.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.HubController.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.DAL;

    using DEHPCommon.UserPreferenceHandler.Enums;

    /// <summary>
    /// Definition of the interface for the <see cref="HubController"/>
    /// </summary>
    public interface IHubController
    {
        /// <summary>
        /// Gets the <see cref="Session"/> object that is encapsulated by the current <see cref="HubController"/>.
        /// </summary>
        ISession Session { get; }

        /// <summary>
        /// Checks whether the session is open by checking if
        /// the <see cref="CDP4Common.SiteDirectoryData.SiteDirectory" /> is available
        /// </summary>
        bool IsSessionOpen { get; }

        /// <summary>
        /// Gets the <see cref="EngineeringModelSetup"/> contained in the site directory
        /// </summary>
        IEnumerable<EngineeringModelSetup> GetEngineeringModels();

        /// <summary>
        /// Retrieves the <see cref="SiteDirectory"/> that is loaded in the <see cref="ISession"/>
        /// </summary>
        /// <returns>The <see cref="SiteDirectory"/></returns>
        SiteDirectory GetSiteDirectory();

        /// <summary>
        /// Gets the <see cref="Thing"/> by its <see cref="iid"/> from the cache
        /// </summary>
        /// <typeparam name="TThing">The Type of <see cref="Thing"/> to get</typeparam>
        /// <param name="iid">The id of the <see cref="Thing"/></param>
        /// <param name="iteration"></param>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>An assert whether the <see cref="thing"/> has been found</returns>
        bool GetThingById<TThing>(Guid iid, Iteration iteration, out TThing thing) where TThing : Thing;

        /// <summary>
        /// Gets the <see cref="Thing"/> by its <see cref="iid"/> from rdls
        /// </summary>
        /// <typeparam name="TThing">The Type of <see cref="Thing"/> to get</typeparam>
        /// <param name="iid">The id of the <see cref="Thing"/></param>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>An assert whether the <see cref="thing"/> has been found</returns>
        bool GetThingById<TThing>(Guid iid, out TThing thing) where TThing : Thing, new();

        /// <summary>
        /// Opens a session
        /// </summary>
        /// <param name="serverType">The selected <see cref="ServerType"/></param>
        /// <param name="credentials">The <see cref="Credentials"/></param>
        /// <returns>An assert whether the session is open</returns>
        Task<bool> Open(Credentials credentials, ServerType serverType);

        /// <summary>
        /// Closes connection to the data-source and end the execution of this app
        /// </summary>
        void Close();

        /// <summary>
        /// Reads an <see cref="Iteration"/> and set the active <see cref="DomainOfExpertise"/> for the Iteration
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="domain">The <see cref="DomainOfExpertise"/></param>
        /// <returns>A <see cref="Task"/></returns>
        Task GetIteration(Iteration iteration, DomainOfExpertise domain);

        /// <summary>
        /// Reads an <see cref="Iteration"/> and set the active <see cref="DomainOfExpertise"/> for the Iteration
        /// </summary>
        /// <returns>A <see cref="IReadOnlyDictionary{T,T}"/> of <see cref="Iteration"/> and <see cref="Tuple{T,T}"/> of <see cref="DomainOfExpertise"/> and <see cref="Participant"/></returns>
        IReadOnlyDictionary<Iteration, Tuple<DomainOfExpertise, Participant>> GetIteration();

        /// <summary>
        /// Creates or updates all <see cref="Thing"/> from the provided <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="things">The <see cref="IEnumerable{T}"/> of <see cref="Thing"/></param>
        /// <param name="deep">Assert whether to create nested things</param>
        /// <returns>A <see cref="Task"/></returns>
        Task CreateOrUpdate(IEnumerable<Thing> things, bool deep = false);

        /// <summary>
        /// Creates or updates the provided <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="deep">Assert whether to create nested things</param>
        /// <returns>A <see cref="Task"/></returns>
        Task CreateOrUpdate(Thing thing, bool deep = false);

        /// <summary>
        /// Deletes all the <see cref="Thing"/> from the provided <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="things">The <see cref="Thing"/> to delete</param>
        /// <returns>A <see cref="Task"/></returns>
        Task Delete(IEnumerable<Thing> things);

        /// <summary>
        /// Deletes a <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> to delete</param>
        /// <returns>A <see cref="Task"/></returns>
        Task Delete(Thing thing);

        /// <summary>
        /// Generates the nested element based on the provided <see cref="Option"/>
        /// </summary>
        /// <param name="option">The <see cref="Option"/></param>
        /// <param name="domainOfExpertise">The <see cref="DomainOfExpertise"/></param>
        /// <param name="updateOption">An assert whether the <see cref="Option"/> shall be updated with the created <see cref="NestedElement"/></param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="NestedElement"/></returns>
        IEnumerable<NestedElement> GetNestedElementTree(Option option, DomainOfExpertise domainOfExpertise, bool updateOption);
    }
}
