// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObjectBrowserViewModel.cs" company="RHEA System S.A.">
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
    using System;

    using CDP4Common.CommonData;

    using DEHPCommon.Services.ObjectBrowserTreeSelectorService;

    using ReactiveUI;

    /// <summary>
    /// Interface definition for the <see cref="ObjectBrowserViewModel"/>
    /// </summary>
    public interface IObjectBrowserViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the browser is busy
        /// </summary>
        bool? IsBusy { get; set; }

        /// <summary>
        /// Gets or sets the selected thing
        /// </summary>
        object SelectedThing { get; set; }

        /// <summary>
        /// Gets or sets the selected things collection
        /// </summary>
        ReactiveList<object> SelectedThings { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="IRowViewModelBase{T}"/> to be displayed in the tree
        /// </summary>
        ReactiveList<BrowserViewModelBase> Things { get; }

        /// <summary>
        /// Gets the Context Menu for the implementing view model
        /// </summary>
        ReactiveList<ContextMenuItemViewModel> ContextMenu { get; }

        /// <summary>
        /// Gets the command that allows to map the selected things
        /// </summary>
        ReactiveCommand<object> MapCommand { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IObservable{T}"/> of <see cref="bool"/> that is bound to the <see cref="MapCommand"/> <see cref="ReactiveCommand{T}.CanExecute"/> property
        /// </summary>
        /// <remarks>This observable is intended to be Merged with another observable</remarks>
        IObservable<bool> CanMap { get; set; }

        /// <summary>
        /// Gets the Caption of the control
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Gets the tooltip of the control
        /// </summary>
        string ToolTip { get; }

        /// <summary>
        /// Reloads the the trees elements
        /// </summary>
        void Reload();

        /// <summary>
        /// Adds to the <see cref="ObjectBrowserViewModel.Things"/> collection the specified by <see cref="IObjectBrowserTreeSelectorService"/> trees
        /// </summary>
        void BuildTrees();

        /// <summary>
        /// Populate the context menu for the implementing view model
        /// </summary>
        void PopulateContextMenu();

        /// <summary>
        /// Dispose of this <see cref="ObjectBrowserViewModel"/>
        /// </summary>
        void Dispose();
    }
}
