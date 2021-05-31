// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectEvent.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2020 RHEA System S.A.
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
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    /// <summary>
    /// The purpose of the <see cref="SelectEvent"/> is to notify an observer
    /// that the referenced <see cref="ElementBase"/> has to be selected.
    /// </summary>
    public class SelectEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectEvent"/> class.
        /// </summary>
        /// <param name="thing"> The payload <see cref="ElementBase"/> to highlight. </param>
        /// <param name="cancelSelection"> A value indicating whether the listener has to cancel selection </param>
        public SelectEvent(ElementBase thing, bool cancelSelection = false) 
        {
            this.SelectedThing = thing;
            this.CancelSelection = cancelSelection;
        }

        /// <summary>
        /// Gets or sets the selected <see cref="ElementBase"/>
        /// </summary>
        public ElementBase SelectedThing { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether the <see cref="SelectedThing"/> should be unselected
        /// </summary>
        public bool CancelSelection { get; set; }
    }
}