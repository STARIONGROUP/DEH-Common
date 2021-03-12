// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeHistoryEntryViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.ExchangeHistory
{
    using System;

    using Newtonsoft.Json;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="ExchangeHistoryEntryViewModel"/> represents one entry in the exchange history
    /// </summary>
    public class ExchangeHistoryEntryViewModel : ReactiveObject
    {
        /// <summary>
        /// Backing field for <see cref="Message"/>
        /// </summary>
        private string message;

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        [JsonProperty]
        public string Message
        {
            get => this.message;
            set => this.RaiseAndSetIfChanged(ref this.message, value);
        }

        /// <summary>
        /// Backing field for <see cref="Domain"/>
        /// </summary>
        private string domain;

        /// <summary>
        /// Gets or sets the domain of expertise
        /// </summary>
        [JsonProperty]
        public string Domain
        {
            get => this.domain;
            set => this.RaiseAndSetIfChanged(ref this.domain, value);
        }

        /// <summary>
        /// Backing field for <see cref="Person"/>
        /// </summary>
        private string person;

        /// <summary>
        /// Gets or sets the domain of expertise
        /// </summary>
        [JsonProperty]
        public string Person
        {
            get => this.person;
            set => this.RaiseAndSetIfChanged(ref this.person, value);
        }

        /// <summary>
        /// Backing field for <see cref="Timestamp"/>
        /// </summary>
        private DateTime timestamp;

        /// <summary>
        /// Gets or sets the time stamp when this entry was registered
        /// </summary>
        [JsonProperty]
        public DateTime Timestamp
        {
            get => this.timestamp;
            set => this.RaiseAndSetIfChanged(ref this.timestamp, value);
        }
    }
}
