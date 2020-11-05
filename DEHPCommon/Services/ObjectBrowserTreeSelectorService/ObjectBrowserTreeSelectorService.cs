// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectBrowserTreeSelectorService.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Services.ObjectBrowserTreeSelectorService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using DEHPCommon.UserInterfaces.Views.ObjectBrowser;

    /// <summary>
    /// The <see cref="ObjectBrowserTreeSelectorService"/> allows to select the kinds of <see cref="Thing"/> to display in the <see cref="UserInterfaces.Views.ObjectBrowser.ObjectBrowser"/>
    /// </summary>
    public class ObjectBrowserTreeSelectorService : IObjectBrowserTreeSelectorService
    {
        /// <summary>
        /// Backing field for <see cref="ThingKinds"/>
        /// </summary>
        private readonly List<Type> thingKinds = new List<Type>() { typeof(ElementDefinition) };

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="Type"/> that defines the type that will be displayed in the <see cref="ObjectBrowser"/>
        /// </summary>
        public IReadOnlyList<Type> ThingKinds => this.thingKinds.AsReadOnly();

        /// <summary>
        /// Gets the allowed <see cref="Type"/>s
        /// </summary>
        public readonly Type[] AllowedTypes = { typeof(ElementDefinition), typeof(RequirementsSpecification) };
        
        /// <summary>
        /// Adds a <see cref="Thing"/> <see cref="Type"/> to <see cref="ThingKinds"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="Thing"/> <see cref="Type"/></typeparam>
        /// <remarks>Only <see cref="AllowedTypes"/> are allowed</remarks>
        public void Add<T>() where T : Thing
        {
            if (!this.thingKinds.Contains(typeof(T)) && this.AllowedTypes.Contains(typeof(T)))
            {
                this.thingKinds.Add(typeof(T));
            }
        }
        
        /// <summary>
        /// Remove a <see cref="Thing"/> <see cref="Type"/> from <see cref="ThingKinds"/>
        /// </summary>
        /// <typeparam name="T">The <see cref="Thing"/> <see cref="Type"/></typeparam>
        public void Remove<T>() where T : Thing
        {
            if (this.thingKinds.Contains(typeof(T)))
            {
                this.thingKinds.Remove(typeof(T));
            }
        }
    }
}
