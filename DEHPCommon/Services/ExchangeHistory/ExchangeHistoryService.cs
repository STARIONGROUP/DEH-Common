// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeHistoryService.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski.
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

namespace DEHPCommon.Services.ExchangeHistory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.Services.AdapterVersionService;
    using DEHPCommon.UserInterfaces.ViewModels.ExchangeHistory;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using Newtonsoft.Json;

    using NLog;

    using File = System.IO.File;

    /// <summary>
    /// The <see cref="ExchangeHistoryService"/>
    /// </summary>
    public class ExchangeHistoryService : IExchangeHistoryService
    {
        /// <summary>
        /// The <see cref="NLog"/>
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the file name
        /// </summary>
        private const string FileName = "ExchangeHistory.json";

        /// <summary>
        /// The <see cref="IHubController"/>
        /// </summary>
        private readonly IHubController hubController;

        /// <summary>
        /// The <see cref="IStatusBarControlViewModel"/>
        /// </summary>
        private readonly IStatusBarControlViewModel statusBar;

        /// <summary>
        /// The <see cref="IAdapterVersionService"/>
        /// </summary>
        private readonly IAdapterVersionService adapterVersionService;

        /// <summary>
        /// Gets the collection of entries
        /// </summary>
        public List<ExchangeHistoryEntryViewModel> PendingEntries { get; } = new List<ExchangeHistoryEntryViewModel>();

        /// <summary>
        /// Initializes a new <see cref="ExchangeHistoryService"/>
        /// <param name="hubController">The <see cref="IHubController"/></param>
        /// <param name="statusBar">The <see cref="IStatusBarControlViewModel"/></param>
        /// <param name="adapterVersionService">The <see cref="IAdapterVersionService"/></param>
        /// </summary>
        public ExchangeHistoryService(IHubController hubController, IStatusBarControlViewModel statusBar,
            IAdapterVersionService adapterVersionService)
        {
            this.hubController = hubController;
            this.statusBar = statusBar;
            this.adapterVersionService = adapterVersionService;
        }

        /// <summary>
        /// Appends to the history a entry that concernes a difference between two <see cref="IValueSet"/>
        /// </summary>
        /// <param name="valueToUpdate">The valueToUpdate to update</param>
        /// <param name="newValue">The <see cref="IValueSet"/> of reference</param>
        /// <param name="switchKind">The <see cref="ParameterSwitchKind"/> where changes are related</param>
        public void Append(ParameterValueSetBase valueToUpdate, IValueSet newValue, ParameterSwitchKind switchKind = ParameterSwitchKind.COMPUTED)
        {
            var parameter = valueToUpdate.GetContainerOfType<ParameterOrOverrideBase>();
            var scale = parameter.Scale is null ? "-" : parameter.Scale.ShortName;

            var prefix = $"{(valueToUpdate.ActualOption is null ? string.Empty : $" Option: {valueToUpdate.ActualOption.Name}")}" +
                         $"{(valueToUpdate.ActualState is null ? string.Empty : $" State: {valueToUpdate.ActualState.Name}")}";

            var newValueRepresentation = GetValueSetValueRepresentation(parameter, newValue, switchKind);
            var oldValueRepresentation = GetValueSetValueRepresentation(parameter, valueToUpdate, switchKind);

            var newValueString = $"{prefix} {newValueRepresentation} [{scale}]";
            var valueToUpdateString = $"{prefix} {oldValueRepresentation} [{scale}]";

            this.Append($"Value: [{valueToUpdateString}] from Parameter [{parameter?.ModelCode()}] " +
                        $"has been updated to [{newValueString}]");
        }

        /// <summary>
        /// Append to the history an entry that relates of a <see cref="ChangeKind"/> on the <paramref name="thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="changeKind">The <see cref="ChangeKind"/></param>
        public void Append(Thing thing, ChangeKind changeKind)
        {
            switch (thing)
            {
                case INamedThing namedThing:
                    this.Append(namedThing, changeKind);
                    break;
                case ParameterOrOverrideBase parameter:
                    this.Append(parameter, changeKind);
                    break;
                default:
                    this.Append($"{thing.ClassKind} with Id: {thing.Iid} has been {changeKind}d");
                    break;
            }
        }

        /// <summary>
        /// Append to the history an entry that relates of a <see cref="ChangeKind"/> on the <paramref name="thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="INamedThing"/></param>
        /// <param name="changeKind">The <see cref="ChangeKind"/></param>
        private void Append(INamedThing thing, ChangeKind changeKind)
        {
            this.Append($"{(thing as Thing)?.ClassKind} {thing.Name} has been {changeKind}d");
        }

        /// <summary>
        /// Append to the history an entry that relates of a <see cref="ChangeKind"/> on the <paramref name="parameter"/>
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterOrOverrideBase"/></param>
        /// <param name="changeKind">The <see cref="ChangeKind"/></param>
        public void Append(ParameterOrOverrideBase parameter, ChangeKind changeKind)
        {
            this.Append($"{parameter.ClassKind} [{parameter.ParameterType.Name}] from [{parameter.ModelCode()}] has been {changeKind}d");
        }

        /// <summary>
        /// Append to the history
        /// </summary>
        /// <param name="message"></param>
        public void Append(string message)
        {
            this.PendingEntries.Add(new ExchangeHistoryEntryViewModel()
            {
                Message = message,
                Person = this.hubController.Session.ActivePerson.Name,
                Domain = this.hubController.CurrentDomainOfExpertise.ShortName
            });
        }

        /// <summary>
        /// Gets the correct representation of the <paramref name="valueSet"/>
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterBase"/> container</param>
        /// <param name="valueSet">The <see cref="IValueSet"/></param>
        /// <param name="switchKind">The <see cref="ParameterSwitchKind"/> where changes are related</param>
        /// <returns>The representation of the<paramref name="valueSet"/></returns>
        private static string GetValueSetValueRepresentation(ParameterBase parameter, IValueSet valueSet, ParameterSwitchKind switchKind)
        {
            var valueArray = switchKind switch
            {
                ParameterSwitchKind.COMPUTED => valueSet.Computed,
                ParameterSwitchKind.MANUAL => valueSet.Manual,
                ParameterSwitchKind.REFERENCE => valueSet.Reference,
            };

            if (parameter.ParameterType is SampledFunctionParameterType)
            {
                var cols = parameter.ParameterType.NumberOfValues;
                return $"[{valueArray.Count / cols}x{cols}]";
            }

            return valueArray.DefaultIfEmpty("-").Aggregate((x, y) => $"{x},{y}");
        }

        /// <summary>
        /// Writes the <see cref="PendingEntries"/> to the JsonFile
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        public async Task Write()
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var timestamp = DateTime.Now;
                this.PendingEntries.ForEach(x => x.Timestamp = timestamp);
                this.PendingEntries.ForEach(x => x.AdapterVersion = this.adapterVersionService.CurrentAdapterVersion);

                var entries = this.Read() ?? new List<ExchangeHistoryEntryViewModel>();

                entries.AddRange(this.PendingEntries);

                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entries));

                using var fileStream = new FileStream(Path, FileMode.OpenOrCreate);

                await fileStream.WriteAsync(buffer, 0, buffer.Length);

                this.ClearPending();

                stopwatch.Stop();
                this.statusBar.Append($"Exchange history processed in {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception exception)
            {
                this.logger.Error(exception);
                throw;
            }
        }

        /// <summary>
        /// Clears up <see cref="PendingEntries"/>
        /// </summary>
        public void ClearPending()
        {
            this.PendingEntries.Clear();
        }

        /// <summary>
        /// Gets all the <see cref="ExchangeHistoryEntryViewModel"/>
        /// </summary>
        /// <returns>A collection of <see cref="ExchangeHistoryEntryViewModel"/></returns>
        public List<ExchangeHistoryEntryViewModel> Read()
        {
            try
            {
                var entries = JsonConvert.DeserializeObject<List<ExchangeHistoryEntryViewModel>>(File.ReadAllText(Path));
                return entries.OrderByDescending(x => x.Timestamp).ToList();
            }
            catch (Exception exception)
            {
                this.logger.Error(exception);
                return new List<ExchangeHistoryEntryViewModel>();
            }
        }

        /// <summary>
        /// Gets the path for the json file
        /// </summary>
        private static string Path
        {
            get
            {
                Directory.CreateDirectory("ExchangeHistory");
                return System.IO.Path.Combine("ExchangeHistory", FileName);
            }
        }
    }
}
