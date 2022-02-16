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
    using System.Reactive.Linq;

    using DEHPCommon.Enumerators;
    using DEHPCommon.Services.NavigationService;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using NLog;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="StatusBarControlViewModel"/> is the base view model for the <see cref="Views.StatusBarControl"/>
    /// </summary>
    public abstract class StatusBarControlViewModel : ReactiveObject, IStatusBarControlViewModel
    {
        /// <summary>
        /// The <see cref="NLog"/> logger
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The <see cref="INavigationService"/>
        /// </summary>
        protected INavigationService NavigationService;

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
        public ReactiveCommand<object> UserSettingCommand { get; protected set; }

        /// <summary>
        /// Initializes a new <see cref="StatusBarControlViewModel"/>
        /// </summary>
        /// <param name="navigationService">The <see cref="NavigationService"/></param>
        protected StatusBarControlViewModel(INavigationService navigationService)
        {
            this.NavigationService = navigationService;
            this.UserSettingCommand = ReactiveCommand.Create(Observable.Empty<bool>().StartWith(false));
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

            switch (messageSeverity)
            {
                case StatusBarMessageSeverity.None:
                    this.logger.Info(textMessage);
                    break;
                case StatusBarMessageSeverity.Info:
                    this.logger.Info(textMessage);
                    break;
                case StatusBarMessageSeverity.Warning:
                    this.logger.Warn(textMessage);
                    break;
                case StatusBarMessageSeverity.Error:
                    this.logger.Error(textMessage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null);
            }
        }

        /// <summary>
        /// Executes the <see cref="UserSettingCommand"/>
        /// </summary>
        protected abstract void ExecuteUserSettingCommand();
    }
}
