// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHubController.cs"company="RHEA System S.A.">
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
    using CDP4Dal.Operations;

    using DEHPCommon.UserPreferenceHandler.Enums;

    /// <summary>
    /// Definition of the interface for the <see cref="HubController"/>
    /// </summary>
    public interface IHubController
    {
        /// <summary>
        /// Checks whether the session is open
        /// </summary>
        bool IsSessionOpen { get; set; }

        /// <summary>
        /// Get the single open <see cref="Iteration"/>
        /// </summary>
        Iteration OpenIteration { get; set; }

        /// <summary>
        /// Ges or sets the current active <see cref="DomainOfExpertise"/>
        /// </summary>
        DomainOfExpertise CurrentDomainOfExpertise { get; set; }

        /// <summary>
        /// Gets the <see cref="Session"/> object that is encapsulated by the current <see cref="HubController"/>.
        /// </summary>
        ISession Session { get; set; }

        /// <summary>
        /// Gets the <see cref="Thing"/> by its <see cref="Thing.Iid"/> from the cache
        /// </summary>
        /// <typeparam name="TThing">The Type of <see cref="Thing"/> to get</typeparam>
        /// <param name="iid">The id of the <see cref="Thing"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>An assert whether the <paramref name="thing"/> has been found</returns>
        bool GetThingById<TThing>(Guid iid, Iteration iteration, out TThing thing) where TThing : Thing;

        /// <summary>
        /// Gets the <see cref="Thing"/> by its <see cref="Thing.Iid"/> from rdls
        /// </summary>
        /// <typeparam name="TThing">The Type of <see cref="Thing"/> to get</typeparam>
        /// <param name="iid">The id of the <see cref="Thing"/></param>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <returns>An assert whether the <see cref="Thing"/> has been found</returns>
        bool GetThingById<TThing>(Guid iid, out TThing thing) where TThing : Thing, new();

        /// <summary>
        /// Opens a session
        /// </summary>
        /// <param name="serverType">The selected <see cref="ServerType"/></param>
        /// <param name="credentials">The <see cref="Credentials"/></param>
        /// <returns>An assert whether the session is open</returns>
        bool Open(Credentials credentials, ServerType serverType);

        /// <inheritdoc cref="ISession.Reload"/>
        void Reload();

        /// <inheritdoc cref="ISession.Refresh"/>
        void Refresh();

        /// <summary>
        /// Closes connection to the data-source and end the execution of this app
        /// </summary>
        void Close();

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
        /// Reads an <see cref="Iteration"/> and set the active <see cref="DomainOfExpertise"/> for the Iteration
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="domain">The <see cref="DomainOfExpertise"/></param>
        /// <returns>A <see cref="Task"/></returns>
        void GetIteration(Iteration iteration, DomainOfExpertise domain);

        /// <summary>
        /// Reads an <see cref="Iteration"/> and set the active <see cref="DomainOfExpertise"/> for the Iteration
        /// </summary>
        /// <returns>A <see cref="IReadOnlyDictionary{T,T}"/> of <see cref="Iteration"/> and <see cref="Tuple{T,T}"/> of <see cref="DomainOfExpertise"/> and <see cref="Participant"/></returns>
        IReadOnlyDictionary<Iteration, Tuple<DomainOfExpertise, Participant>> GetIteration();

        /// <summary>
        /// Creates or updates all <see cref="Thing"/> from the provided <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="TThing">The type of <see cref="Thing"/></typeparam>
        /// <typeparam name="TContainer">The type of <see cref="Thing"/> which contains <typeparamref name="TThing"/></typeparam>
        /// <param name="things">The <see cref="IEnumerable{T}"/> of <see cref="Thing"/></param>
        /// <param name="actionOnClone">The actual <see cref="Action"/> to perform e.g. <code>Container.Collection.Add(new Parameter())</code><remarks>The first parameter is the container clone</remarks></param>
        /// <param name="deep">Assert whether to create nested things</param>
        /// <returns>A <see cref="Task"/></returns>
        void CreateOrUpdate<TContainer, TThing>(IEnumerable<TThing> things, Action<TContainer, TThing> actionOnClone, bool deep = false) where TThing : Thing where TContainer : Thing;

        /// <summary>
        /// Creates or updates the provided <see cref="Thing"/>
        /// </summary>
        /// <typeparam name="TThing">The type of <see cref="Thing"/></typeparam>
        /// <typeparam name="TContainer">The type of <see cref="Thing"/> which contains <typeparamref name="TThing"/></typeparam>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="actionOnClone">The actual <see cref="Action"/> to perform e.g. <code>Container.Collection.Add(new Parameter())</code><remarks>The first parameter is the container clone</remarks></param>
        /// <param name="deep">Assert whether to create nested things</param>
        /// <returns>A <see cref="Task"/></returns>
        void CreateOrUpdate<TContainer, TThing>(TThing thing, Action<TContainer, TThing> actionOnClone, bool deep = false) where TThing : Thing where TContainer : Thing;

        /// <summary>
        /// Deletes all the <see cref="Thing"/> from the provided <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="TThing">The type of <see cref="Thing"/></typeparam>
        /// <typeparam name="TContainer">The type of <see cref="Thing"/> which contains <typeparamref name="TThing"/></typeparam>
        /// <param name="things">The things to delete</param>
        /// <param name="actionOnClone">The actual <see cref="Action"/> to perform e.g. <code>Container.Collection.Add(new Parameter())</code><remarks>The first parameter is the container clone</remarks></param>
        /// <param name="deep">Assert whether to create nested things</param>
        /// <returns>A <see cref="Task"/></returns>
        void Delete<TContainer, TThing>(IEnumerable<TThing> things, Action<TContainer, TThing> actionOnClone, bool deep = false) where TThing : Thing where TContainer : Thing;

        /// <summary>
        /// Deletes a <see cref="Thing"/>
        /// </summary>
        /// <typeparam name="TThing">The type of <see cref="Thing"/></typeparam>
        /// <typeparam name="TContainer">The type of <see cref="Thing"/> which contains <typeparamref name="TThing"/></typeparam>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="actionOnClone">The actual <see cref="Action"/> to perform e.g. <code>Container.Collection.Add(new Parameter())</code><remarks>The first parameter is the container clone</remarks></param>
        /// <param name="deep">Assert whether to create nested things</param>
        /// <returns>A <see cref="Task"/></returns>
        void Delete<TContainer, TThing>(TThing thing, Action<TContainer, TThing> actionOnClone, bool deep = false) where TThing : Thing where TContainer : Thing;

        /// <summary>
        /// Write the transaction to the session
        /// </summary>
        /// <param name="transaction">The <see cref="ThingTransaction"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        void Write(ThingTransaction transaction);

        /// <summary>
        /// Generates the nested element based on the provided <see cref="Option"/>
        /// </summary>
        /// <param name="option">The <see cref="Option"/></param>
        /// <param name="domainOfExpertise">The <see cref="DomainOfExpertise"/></param>
        /// <param name="updateOption">An assert whether the <see cref="Option"/> shall be updated with the created <see cref="NestedElement"/></param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="NestedElement"/></returns>
        IEnumerable<NestedElement> GetNestedElementTree(Option option, DomainOfExpertise domainOfExpertise, bool updateOption);

        /// <summary>
        /// Upload one file to the <see cref="DomainFileStore"/> of the specified domain or of the active domain
        /// </summary>
        /// <param name="filePath">The full path to a local file to be uploaded</param>
        /// <param name="file">The <see cref="File"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="domain">The <see cref="DomainOfExpertise"/></param>
        /// <returns>A <see cref="Task"/></returns>
        void Upload(string filePath = null, File file = null, Iteration iteration = null, DomainOfExpertise domain = null);

        /// <summary>
        /// Computes all the <see cref="FileType"/> of the file that is to be uploaded
        /// </summary>
        /// <param name="extensions">The file extensions</param>
        /// <param name="allowedFileTypes">The Allowed <see cref="FileType"/></param>
        /// <param name="fileName">The name of the file that is to be uploaded</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="FileType"/></returns>
        IEnumerable<FileType> ComputeFileTypes(string[] extensions, IEnumerable<FileType> allowedFileTypes, ref string fileName);

        /// <summary>
        /// Downloads a <see cref="File.CurrentFileRevision"/>
        /// </summary>
        /// <param name="file">The <see cref="File"/></param>
        /// <returns>A <see cref="Task"/></returns>
        void Download(File file);

        /// <summary>
        /// Downloads a specific <see cref="FileRevision"/>
        /// </summary>
        /// <param name="fileRevision">The <see cref="FileRevision"/></param>
        /// <returns>A <see cref="Task"/></returns>
        void Download(FileRevision fileRevision);

        /// <summary>
        /// Downloads a <see cref="File.CurrentFileRevision"/> into <see cref="System.IO.FileStream"/>
        /// </summary>
        /// <param name="file">The <see cref="File"/></param>
        /// <param name="destination">The <see cref="System.IO.FileStream"/></param>
        /// <returns>A <see cref="Task"/></returns>
        void Download(File file, System.IO.FileStream destination);

        /// <summary>
        /// Downloads a specific <see cref="FileRevision"/> into <see cref="System.IO.FileStream"/>
        /// </summary>
        /// <param name="fileRevision">The <see cref="FileRevision"/></param>
        /// <param name="destination">The <see cref="System.IO.FileStream"/></param>
        /// <returns>A <see cref="Task"/></returns>
        void Download(FileRevision fileRevision, System.IO.FileStream destination);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="ExternalIdentifierMap"/> for the provided dst tool
        /// </summary>
        IEnumerable<ExternalIdentifierMap> AvailableExternalIdentifierMap(string toolName);

        /// <summary>
        /// Adds a new <see cref="ModelLogEntry"/> record to the <see cref="EngineeringModel.LogEntry"/> list of the current <see cref="OpenIteration"/> and registers the change to a <see cref="ThingTransaction"/>
        /// </summary>
        /// <param name="content">The value that will be set to the <see cref="ModelLogEntry.Content"/></param>
        /// <param name="transaction">The <see cref="ThingTransaction"/> that will get the changes registered to</param>
        void RegisterNewLogEntryToTransaction(string content, ThingTransaction transaction);
    }
}
