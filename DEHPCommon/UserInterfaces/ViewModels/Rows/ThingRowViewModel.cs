// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThingRowViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.Rows
{
    using System;

    using CDP4Common.CommonData;

    using ReactiveUI;

    /// <summary>
    /// Row class representing a <see cref="Thing"/> as a plain object
    /// </summary>
    public abstract class ThingRowViewModel<T> : ReactiveObject where T : Thing
    {
        /// <summary>
        /// Backing field for <see cref="Iid"/>
        /// </summary>
        private Guid iid;

        /// <summary>
        /// Backing field for <see cref="RevisionNumber"/>
        /// </summary>
        private int revisionNumber;

        /// <summary>
        /// Get or sets the <see cref="Thing"/>
        /// </summary>
        public T Thing { get; private set; }

        /// <summary>
        /// Gets or sets the iid
        /// </summary>
        public Guid Iid
        {
            get => this.iid;
            private set => this.RaiseAndSetIfChanged(ref this.iid, value);
        }

        /// <summary>
        /// Gets or sets the revisionNumber
        /// </summary>
        public int RevisionNumber
        {
            get => this.revisionNumber;
            private set => this.RaiseAndSetIfChanged(ref this.revisionNumber, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThingRowViewModel"/> class
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> represented</param>
        protected ThingRowViewModel(T thing)
        {
            this.Thing = thing;
            this.Iid = thing.Iid;
            this.RevisionNumber = thing.RevisionNumber;
        }
    }
}
