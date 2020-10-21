// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinedThingRowViewModel.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
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

namespace DEHPCommon.UserInterfaces.ViewModels.Rows
{
    using CDP4Common.CommonData;

    using ReactiveUI;

    /// <summary>
    /// Row class representing a <see cref="DefinedThing"/> as a plain object
    /// </summary>
    public abstract class DefinedThingRowViewModel<T> : ThingRowViewModel<T> where T : DefinedThing
    {
        /// <summary>
        /// Backing field for <see cref="Name"/>
        /// </summary>
        private string name;

        /// <summary>
        /// Backing field for <see cref="ShortName"/>
        /// </summary>
        private string shortName;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get => this.name;
            private set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets or sets the shortName
        /// </summary>
        public string ShortName
        {
            get => this.shortName;
            private set => this.RaiseAndSetIfChanged(ref this.shortName, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinedThingRowViewModel"/> class
        /// </summary>
        /// <param name="definedThing">The <see cref="DefinedThing"/> represented</param>
        protected DefinedThingRowViewModel(T definedThing) : base(definedThing)
        {
            this.Name = definedThing.Name;
            this.ShortName = definedThing.ShortName;
        }
    }
}
