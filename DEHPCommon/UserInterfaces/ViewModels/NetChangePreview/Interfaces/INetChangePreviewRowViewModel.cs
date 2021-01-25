// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INetChangePreviewRowViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.NetChangePreview.Interfaces
{
    using CDP4Common.CommonData;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.NetChangePreview.Rows;

    using ReactiveUI;

    /// <summary>
    /// Interface definition for the <see cref="BaseNetChangePreviewRowViewModel{T}"/>
    /// </summary>
    public interface INetChangePreviewRowViewModel
    {
        /// <summary>
        /// Gets or sets this row <see cref="ClassKind"/>
        /// </summary>
        /// <remarks>If this view model represents a dst object <see cref="ThingKind"/> = <see cref="ClassKind.NotThing"/> which is the default value</remarks>
        ClassKind ThingKind { get; set; }

        /// <summary>
        /// Gets or sets this row new values
        /// </summary>
        string NewValue { get; set; }

        /// <summary>
        /// Gets or sets this row new values
        /// </summary>
        string OldValue { get; set; }

        /// <summary>
        /// Gets or sets this row new values
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets this row contained rows
        /// </summary>
        ReactiveList<INetChangePreviewViewModel> ContainedRows { get; set; }
    }
}
