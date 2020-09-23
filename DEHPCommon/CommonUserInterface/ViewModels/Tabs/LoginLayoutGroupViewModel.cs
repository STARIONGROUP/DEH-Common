// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginLayoutGroupViewModel.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.CommonUserInterface.ViewModels.Tabs
{
    using System;

    using DEHPCommon.CommonUserInterface.ViewModels.Common;

    using ReactiveUI;

    /// <summary>
    /// The view-model for the LoginLayoutGroup that allows to manage displayed resources.
    /// </summary>
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
        /// Initializes a new instance of the <see cref="LoginLayoutGroupViewModel"/> class
        /// </summary>
        public LoginLayoutGroupViewModel()
        {
            this.WhenAnyValue(vm => vm.LoginViewModel.LoginSuccessfully).Subscribe(value =>
            {
                this.ServerIsChecked = value;
            });
            
        }
    }
}
