// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusBarControlViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels
{
    using System;

    using DEHPCommon.Enumerators;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="StatusBarControlViewModel"/> is the view model for the <see cref="Views.StatusBarControl"/>
    /// </summary>
    public class StatusBarControlViewModel : ReactiveObject, IStatusBarControlViewModel
    {
        /// <summary>
        /// Backing field for <see cref="Message"/>
        /// </summary>
        private string message;
        
        /// <summary>
        /// Gets or sets the message that is displayed
        /// </summary>
        public string Message
        {
            get => this.message;
            private set => this.RaiseAndSetIfChanged(ref this.message, value);
        }

        /// <summary>
        /// Backing field for <see cref="Severity"/>
        /// </summary>
        private StatusBarMessageSeverity severity;

        /// <summary>
        /// Gets or sets the severity of the current <see cref="Message"/>
        /// </summary>
        public StatusBarMessageSeverity Severity
        {
            get => this.severity;
            private set => this.RaiseAndSetIfChanged(ref this.severity, value);
        }

        /// <summary>
        /// Gets or sets the command that opens the user setting dialog
        /// </summary>
        public ReactiveCommand<object> UserSettingCommand { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="StatusBarControlViewModel"/>
        /// </summary>
        public StatusBarControlViewModel()
        {
            this.UserSettingCommand = ReactiveCommand.Create();
            this.UserSettingCommand.Subscribe(_ => this.ExecuteUserSettingCommand());
        }

        /// <summary>
        /// Appends a new message to the represented status bar
        /// </summary>
        /// <param name="textMessage">The message to display</param>
        /// <param name="messageSeverity">The message to display</param>
        public void Append(string textMessage, StatusBarMessageSeverity messageSeverity = StatusBarMessageSeverity.Info)
        {
            this.Message = $"{DateTime.Now:g} - {textMessage}";
            this.Severity = messageSeverity;
        }

        /// <summary>
        /// Executes the <see cref="UserSettingCommand"/>
        /// </summary>
        private void ExecuteUserSettingCommand()
        {
            this.Append("User setting dialog opened");
        }
    }
}
