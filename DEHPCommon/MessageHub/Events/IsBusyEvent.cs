// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsBusyEvent.cs"company="RHEA System S.A.">
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

namespace DEHPCommon.MessageHub.Events
{
    /// <summary>
    /// The purpose of the <see cref="IsBusyEvent"/> is to notify an observer
    /// that the loading notification should be displayed to prevent the application to freeze
    /// </summary>
    public class IsBusyEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsBusyEvent"/> class.
        /// </summary>
        /// <param name="isBusy">
        /// The payload
        /// </param>
        /// <param name="message">
        /// The optional message
        /// </param>
        public IsBusyEvent(bool isBusy, string message = "")
        {
            this.IsBusy = isBusy;
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets a value indicating whether application has busy status
        /// </summary>
        public bool IsBusy { get; set; }

        /// <summary>
        /// Gets or sets a message indicating whether application has busy status
        /// </summary>
        public string Message { get; set; }
    }
}
