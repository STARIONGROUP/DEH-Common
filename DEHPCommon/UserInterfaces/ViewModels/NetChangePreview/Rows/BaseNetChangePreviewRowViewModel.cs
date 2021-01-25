// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseNetChangePreviewRowViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.NetChangePreview.Rows
{
    using CDP4Common.CommonData;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.NetChangePreview.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// Base Row view model for <see cref="ThingNetChangePreviewRowViewModel"/> and <see cref="DstBaseNetChangePreviewRowViewModel"/>
    /// </summary>
    /// <typeparam name="TThing">The type of the object this row represents</typeparam>
    public abstract class BaseNetChangePreviewRowViewModel<TThing> : ReactiveObject, INetChangePreviewRowViewModel
    {
        /// <summary>
        /// Gets or sets this represented thing
        /// </summary>
        public abstract TThing Thing { get; set; }

        /// <summary>
        /// Backing field for <see cref="ThingKind"/>
        /// </summary>
        private ClassKind thingKind = ClassKind.NotThing;

        /// <summary>
        /// Gets or sets this row <see cref="ClassKind"/>
        /// </summary>
        /// <remarks>If this view model represents a dst object <see cref="ThingKind"/> = <see cref="ClassKind.NotThing"/> which is the default value</remarks>
        public ClassKind ThingKind
        {
            get => this.thingKind;
            set => this.RaiseAndSetIfChanged(ref this.thingKind, value);
        }

        /// <summary>
        /// Backing field for <see cref="NewValue"/>
        /// </summary>
        private string newValue;

        /// <summary>
        /// Gets or sets this row new values
        /// </summary>
        public string NewValue
        {
            get => this.newValue;
            set => this.RaiseAndSetIfChanged(ref this.newValue, value);
        }

        /// <summary>
        /// Backing field for <see cref="NewValue"/>
        /// </summary>
        private string oldValue;

        /// <summary>
        /// Gets or sets this row new values
        /// </summary>
        public string OldValue
        {
            get => this.oldValue;
            set => this.RaiseAndSetIfChanged(ref this.oldValue, value);
        }
        
        /// <summary>
        /// Backing field for <see cref="Name"/>
        /// </summary>
        private string name;

        /// <summary>
        /// Gets or sets this row new values
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets or sets this row contained rows
        /// </summary>
        public ReactiveList<INetChangePreviewViewModel> ContainedRows { get; set; } = new ReactiveList<INetChangePreviewViewModel>();
    }
}