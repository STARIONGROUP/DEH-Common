// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExchangeHistoryService.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using DEHPCommon.UserInterfaces.ViewModels.ExchangeHistory;

    /// <summary>
    /// Interface definition for the <see cref="ExchangeHistoryService"/>
    /// </summary>
    public interface IExchangeHistoryService
    {
        /// <summary>
        /// Gets the collection of entries
        /// </summary>
        List<ExchangeHistoryEntryViewModel> PendingEntries { get; }

        /// <summary>
        /// Append to the history an entry that relates of a <see cref="ChangeKind"/> on the <paramref name="thing"/>
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="changeKind">The <see cref="ChangeKind"/></param>
        void Append(Thing thing, ChangeKind changeKind);

        /// <summary>
        /// Append to the history an entry that relates of a <see cref="ChangeKind"/> on the <paramref name="parameter"/>
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterOrOverrideBase"/></param>
        /// <param name="changeKind">The <see cref="ChangeKind"/></param>
        void Append(ParameterOrOverrideBase parameter, ChangeKind changeKind);

        /// <summary>
        /// Append to the history
        /// </summary>
        /// <param name="message"></param>
        void Append(string message);

        /// <summary>
        /// Writes the <see cref="ExchangeHistoryService.PendingEntries"/> to the JsonFile
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        Task Write();

        /// <summary>
        /// Clears up <see cref="ExchangeHistoryService.PendingEntries"/>
        /// </summary>
        void ClearPending();

        /// <summary>
        /// Gets all the <see cref="ExchangeHistoryEntryViewModel"/>
        /// </summary>
        /// <returns>A collection of <see cref="ExchangeHistoryEntryViewModel"/></returns>
        List<ExchangeHistoryEntryViewModel> Read();

        /// <summary>
        /// Appends to the history a entry that concernes a difference between two <see cref="IValueSet"/>
        /// </summary>
        /// <param name="valueToUpdate">The valueToUpdate to update</param>
        /// <param name="newValue">The <see cref="IValueSet"/> of reference</param>
        /// <param name="switchKind">The <see cref="ParameterSwitchKind"/> where changes are related</param>
        void Append(ParameterValueSetBase valueToUpdate, IValueSet newValue, ParameterSwitchKind switchKind = ParameterSwitchKind.COMPUTED);
    }
}
