// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHubBrowserHeaderControlViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.Interfaces
{
    using System.Reactive;
    using System.Windows.Input;

    using CDP4Dal;

    using ReactiveUI;

    /// <summary>
    /// Interface definition for <see cref="HubSessionControlViewModel"/>
    /// </summary>
    public interface IHubSessionControlViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ISession"/> is open
        /// auto refresh property enabled or disabled.
        /// </summary>
        bool IsSessionOpen { get; set; }

        /// <summary>
        /// Gets or sets the command to refresh the session
        /// </summary>
        ReactiveCommand<Unit> RefreshCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to reload the session
        /// </summary>
        ReactiveCommand<Unit> ReloadCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ISession"/> object has it's
        /// auto refresh property enabled or disabled.
        /// </summary>
        bool IsAutoRefreshEnabled { get; set; }

        /// <summary>
        /// Gets or sets the auto-refresh interval when applicable
        /// </summary>
        uint AutoRefreshInterval { get; set; }

        /// <summary>
        /// Gets or sets the auto-refresh seconds left.
        /// </summary>
        uint AutoRefreshSecondsLeft { get; set; }
    }
}
