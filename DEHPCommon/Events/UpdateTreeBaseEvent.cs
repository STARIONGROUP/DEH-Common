// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateTreeBaseEvent.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Events
{
    /// <summary>
    /// Base event class for events that affect tree views in dst adapters
    /// </summary>
    public abstract class UpdateTreeBaseEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the listener should reset its tree
        /// </summary>
        public bool Reset { get; }

        /// <summary>
        /// Initializes a new <see cref="UpdateTreeBaseEvent"/>
        /// </summary>
        /// <param name="reset">a value indicating whether the listener should reset its tree</param>
        protected UpdateTreeBaseEvent(bool reset = false)
        {
            this.Reset = reset;
        }
    }
}
