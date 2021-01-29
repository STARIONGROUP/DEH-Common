// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateObjectBrowserTreeEvent.cs" company="RHEA System S.A.">
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
    using DEHPCommon.UserInterfaces.ViewModels;

    /// <summary>
    /// The purpose of the <see cref="UpdateObjectBrowserTreeEvent"/> is to notify
    /// an <see cref="ObjectBrowserViewModel"/> about whether it should updates its trees
    /// </summary>
    public class UpdateObjectBrowserTreeEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the listener should reset its tree
        /// </summary>
        public bool Reset { get; set; }

        /// <summary>
        /// Initializes a new <see cref="UpdateObjectBrowserTreeEvent"/>
        /// </summary>
        /// <param name="reset">a value indicating whether the listener should reset its tree</param>
        public UpdateObjectBrowserTreeEvent(bool reset = false)
        {
            this.Reset = reset;
        }
    }
}
