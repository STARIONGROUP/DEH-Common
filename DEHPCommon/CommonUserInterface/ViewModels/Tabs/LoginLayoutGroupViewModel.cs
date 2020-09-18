// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.CommonUserInterface.ViewModels.Tabs
{
    using System;

    using DEHPCommon.CommonUserInterface.ViewModels.Common;

    using ReactiveUI;

    public class LoginLayoutGroupViewModel : ReactiveObject
    {
        /// <summary>
        /// Backing field for <see cref="ServerIsChecked"/>
        /// </summary>
        private bool serverIsChecked;

        /// <summary>
        /// Gets or sets server source as option for migration
        /// </summary>
        public bool ServerIsChecked
        {
            get => this.serverIsChecked;

            set => this.RaiseAndSetIfChanged(ref this.serverIsChecked, value);
        }

        /// <summary>
        /// Backing field for the source view model <see cref="LoginViewModel"/>
        /// </summary>
        private LoginViewModel loginViewModel;

        /// <summary>
        /// Gets or sets the loging source view model
        /// </summary>
        public LoginViewModel LoginViewModel
        {
            get => this.loginViewModel;

            set => this.RaiseAndSetIfChanged(ref this.loginViewModel, value);
        }

        /// <summary>
        /// Out property for the <see cref="CanLogIn"/> property
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> canLogIn;

        /// <summary>
        /// Gets a value indicating whether a migration operation can start
        /// </summary>
        public bool CanLogIn
        {
            get { return this.canLogIn.Value; }
        }
        /// <summary>
        /// Backing field for the the output messages <see cref="Output"/>
        /// </summary>
        private string output;

        /// <summary>
        /// Gets or sets operation output messages
        /// </summary>
        public string Output
        {
            get => this.output;

            set => this.RaiseAndSetIfChanged(ref this.output, value);
        }

        /// <summary>
        /// Add subscription to the login viewmodels
        /// </summary>
        public void AddSubscriptions()
        {
            this.WhenAnyValue(vm => vm.LoginViewModel.Output).Subscribe(message =>
            {
                this.UpdateOutput(message);
            });

            this.WhenAnyValue(vm => vm.LoginViewModel.LoginSuccessfully, vm => vm.LoginViewModel.ServerSession, (loginSuccessfully, dataSourceSession) =>
            {
                return loginSuccessfully && dataSourceSession != null;
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginLayoutGroupViewModel"/> class
        /// </summary>
        public LoginLayoutGroupViewModel()
        {
            this.WhenAnyValue(
                vm => vm.LoginViewModel.LoginSuccessfully,
                vm => vm.LoginViewModel.ServerSession,
                (sourceLoginSuccessfully, sourceSession) =>
                {
                    return sourceLoginSuccessfully && sourceSession != null;
                }).ToProperty(this, vm => vm.CanLogIn, out this.canLogIn);

            this.ServerIsChecked = true;
        }

        /// <summary>
        /// Add text message to the output panel
        /// </summary>
        /// <param name="message">The text message</param>
        private void UpdateOutput(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            this.Output += $"{DateTime.Now:HH:mm:ss} {message}{Environment.NewLine}";
        }
    }
}
