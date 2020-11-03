// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRowViewModelBase.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.Interfaces
{
    using CDP4Common.CommonData;
    using CDP4Common.Types;

    /// <summary>
    /// The interface for the row-view-model
    /// </summary>
    /// <typeparam name="T">The <see cref="Thing"/> represented by the row</typeparam>
    public interface IRowViewModelBase<out T> : IViewModelBase<T>, IHaveContainerViewModel, IHaveContainedRows where T : Thing
    {
        /// <summary>
        /// Gets or sets the index of the row
        /// </summary>
        /// <remarks>
        /// this property is used in the case of <see cref="OrderedItemList{T}"/>
        /// </remarks>
        int Index { get; set; }

        /// <summary>
        /// Gets the top container <see cref="IViewModelBase{T}"/>
        /// </summary>
        /// <remarks>
        /// this should either be a <see cref="IDialogViewModelBase{T}"/> or a <see cref="IObjectBrowserViewModel"/>
        /// </remarks>
        IViewModelBase<Thing> TopContainerViewModel { get; }

        /// <summary>
        /// Clears the row highlighting for itself and its children.
        /// </summary>
        void ClearRowHighlighting();

        /// <summary>
        /// Sets a value indicating that the row is expanded
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Expands the current row and all contained rows along the containment hierarchy
        /// </summary>
        void ExpandAllRows();

        /// <summary>
        /// Collapases the current row and all contained rows along the containment hierarchy
        /// </summary>
        void CollapseAllRows();
    }
}
