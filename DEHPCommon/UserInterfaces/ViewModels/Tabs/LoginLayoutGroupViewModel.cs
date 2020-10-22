// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginLayoutGroupViewModel.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
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

namespace DEHPCommon.UserInterfaces.ViewModels.Tabs
{
    using System;

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
            this.WhenAnyValue(vm => vm.LoginViewModel.LoginSuccessfull).Subscribe(value =>
            {
                this.ServerIsChecked = value;
            });
            
        }
    }
}
