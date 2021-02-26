// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePreviewBasedOnSelectionBaseEvent.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;

    using CDP4Dal;

    /// <summary>
    /// An event for <see cref="CDPMessageBus"/> that will tell the target net change preview to update
    /// its children based on the things passed
    /// </summary>
    /// <typeparam name="TThing">The <see cref="System.Type"/> of things in the collection</typeparam>
    /// <typeparam name="TTarget">The target <see cref="System.Type"/></typeparam>
    public abstract class UpdatePreviewBasedOnSelectionBaseEvent<TThing, TTarget> : UpdateTreeBaseEvent
    {
        /// <summary>
        /// Gets the collection of object that reflects the selection
        /// </summary>
        public IEnumerable<TThing> Selection { get; }

        /// <summary>
        /// Gets the target type
        /// </summary>
        public TTarget Target { get; }

        /// <summary>
        /// Initializes a new <see cref="UpdatePreviewBasedOnSelectionBaseEvent{T,T}"/>
        /// </summary>
        /// <param name="things">The collection of <typeparamref name="TThing"/> selection</param>
        /// <param name="target">The target <see cref="System.Type"/></param>
        /// <param name="reset">a value indicating whether the listener should reset its tree</param>
        protected UpdatePreviewBasedOnSelectionBaseEvent(IEnumerable<TThing> things, TTarget target, bool reset) : base(reset)
        {
            this.Selection = things;
            this.Target = target;
        }
    }
}
