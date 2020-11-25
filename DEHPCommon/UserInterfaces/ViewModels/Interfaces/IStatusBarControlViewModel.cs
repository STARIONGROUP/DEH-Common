// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatusBarControlViewModel.cs" company="RHEA System S.A.">
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
    using DEHPCommon.Enumerators;

    using ReactiveUI;

    /// <summary>
    /// Interface definition for <see cref="StatusBarControlViewModel"/>
    /// </summary>
    public interface IStatusBarControlViewModel
    {
        /// <summary>
        /// Gets or sets the message that is displayed
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets or sets the severity of the current <see cref="Message"/>
        /// </summary>
        StatusBarMessageSeverity Severity { get; }

        /// <summary>
        /// Gets or sets the command that opens the user setting dialog
        /// </summary>
        ReactiveCommand<object> UserSettingCommand { get; }

        /// <summary>
        /// Appends a new message to the represented status bar
        /// </summary>
        /// <param name="textMessage">The message to display</param>
        /// <param name="messageSeverity">The message to display</param>
        void Append(string textMessage, StatusBarMessageSeverity messageSeverity = StatusBarMessageSeverity.Info);
    }
}
