// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubBrowserHeaderControlViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels
{
    using System;
    using System.Reactive;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Threading;

    using CDP4Dal;

    using DEHPCommon.Enumerators;
    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="HubSessionControlViewModel"/> is the view model for the hub session related control
    /// </summary>
    public class HubSessionControlViewModel : ReactiveObject, IHubSessionControlViewModel
    {
        /// <summary>
        /// The <see cref="IHubController"/>
        /// </summary>
        private readonly IHubController hubController;

        /// <summary>
        /// The <see cref="IStatusBarControlViewModel"/>
        /// </summary>
        private readonly IStatusBarControlViewModel statusBar;

        /// <summary>
        /// The timer
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// Backing field for <see cref="AutoRefreshInterval"/>
        /// </summary>
        private uint autoRefreshInterval = 60;

        /// <summary>
        /// Backing field for <see cref="AutoRefreshSecondsLeft"/>
        /// </summary>
        private uint autoRefreshSecondsLeft;

        /// <summary>
        /// Backing field for the <see cref="IsAutoRefreshEnabled"/>
        /// </summary>
        private bool isAutoRefreshEnabled;

        /// <summary>
        /// Backing field for the <see cref="IsSessionOpen"/>
        /// </summary>
        private bool isSessionOpen;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ISession"/> is open
        /// auto refresh property enabled or disabled.
        /// </summary>
        public bool IsSessionOpen
        {
            get => this.isSessionOpen;
            set => this.RaiseAndSetIfChanged(ref this.isSessionOpen, value);
        }

        /// <summary>
        /// Gets or sets the command to refresh the session
        /// </summary>
        public ReactiveCommand<Unit> RefreshCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to reload the session
        /// </summary>
        public ReactiveCommand<Unit> ReloadCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ISession"/> object has it's
        /// auto refresh property enabled or disabled.
        /// </summary>
        public bool IsAutoRefreshEnabled
        {
            get => this.isAutoRefreshEnabled;
            set => this.RaiseAndSetIfChanged(ref this.isAutoRefreshEnabled, value);
        }

        /// <summary>
        /// Gets or sets the auto-refresh interval when applicable
        /// </summary>
        public uint AutoRefreshInterval
        {
            get => this.autoRefreshInterval;
            set => this.RaiseAndSetIfChanged(ref this.autoRefreshInterval, value);
        }

        /// <summary>
        /// Gets or sets the auto-refresh seconds left.
        /// </summary>
        public uint AutoRefreshSecondsLeft
        {
            get => this.autoRefreshSecondsLeft;
            set => this.RaiseAndSetIfChanged(ref this.autoRefreshSecondsLeft, value);
        }

        /// <summary>
        /// Initializes a new <see cref="HubSessionControlViewModel"/>
        /// </summary>
        /// <param name="hubController">The <see cref="IHubController"/></param>
        /// <param name="statusBar">The <see cref="IStatusBarControlViewModel"/></param>
        public HubSessionControlViewModel(IHubController hubController, IStatusBarControlViewModel statusBar)
        {
            this.hubController = hubController;
            this.statusBar = statusBar;

            this.InitializeCommandsAndObservables();
        }
        
        /// <summary>
        /// Initializes this view model <see cref="ICommand"/>
        /// </summary>
        private void InitializeCommandsAndObservables()
        {
            var isConnectedObservable = this.WhenAny(x => x.hubController.OpenIteration,
                    x => x.hubController.IsSessionOpen,
                    (i, o) =>
                        i.Value != null && o.Value);

            this.RefreshCommand = ReactiveCommand.CreateAsyncTask(isConnectedObservable, async _ =>
                await this.RefreshCommandExecute());

            this.ReloadCommand = ReactiveCommand.CreateAsyncTask(isConnectedObservable, async _ =>
                await this.ReloadCommandExecute());

            this.WhenAnyValue(x => x.IsAutoRefreshEnabled)
                .Subscribe(_ => this.SetTimer());

            this.WhenAnyValue(x => x.AutoRefreshInterval)
                .Subscribe(_ => this.SetTimer());

            isConnectedObservable.Subscribe(x => this.IsSessionOpen = x);
        }

        /// <summary>
        /// Executes the <see cref="RefreshCommand"/>
        /// </summary>
        /// <param name="silent">A value indicating whether the status bar displays the result</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task RefreshCommandExecute(bool silent = false)
        {
            try
            {
                await this.hubController.Refresh();

                if (!silent)
                {
                    this.statusBar.Append($"Hub session has been refreshed");
                }
            }
            catch (Exception e)
            {
                this.statusBar.Append(e.Message, StatusBarMessageSeverity.Error);
            }
        }

        /// <summary>
        /// Executes the <see cref="ReloadCommand"/> Command
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        private async Task ReloadCommandExecute()
        {
            try
            {
                await this.hubController.Reload();
                this.statusBar.Append($"Hub session has been reloaded");
            }
            catch (Exception e)
            {
                this.statusBar.Append(e.Message, StatusBarMessageSeverity.Error);
            }
        }

        /// <summary>
        /// Sets the timer according to the appropriate setting
        /// </summary>
        private void SetTimer()
        {
            if (this.IsAutoRefreshEnabled)
            {
                //dispose of previous timer
                this.timer?.Stop();

                this.AutoRefreshSecondsLeft = this.AutoRefreshInterval;

                this.timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                this.timer.Tick += this.OnTimerElapsed;
                this.timer.Start();
            }
            else
            {
                this.timer.Stop();
            }
        }

        /// <summary>
        /// The eventhandler to handle elapse of one second.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments.</param>
        public void OnTimerElapsed(object sender, EventArgs e)
        {
            this.AutoRefreshSecondsLeft -= 1;

            if (this.AutoRefreshSecondsLeft != 0)
            {
                return;
            }

            this.timer.Stop();

            Task.Run(async () => await this.RefreshCommandExecute(true)).ContinueWith(
                _ =>
                {
                    this.AutoRefreshSecondsLeft = this.AutoRefreshInterval;
                    this.timer.Start();
                });
        }
    }
}
