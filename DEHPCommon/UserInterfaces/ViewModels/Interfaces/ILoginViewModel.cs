// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoginViewModel.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Reactive;

    using DEHPCommon.UserInterfaces.ViewModels.Rows;
    using DEHPCommon.UserPreferenceHandler.Enums;

    using ReactiveUI;

    /// <summary>
    /// Interface definition for <see cref="LoginViewModel"/>
    /// </summary>
    public interface ILoginViewModel : ICloseWindowViewModel
    {
        /// <summary>
        /// Gets or sets datasource server type
        /// </summary>
        Dictionary<ServerType, string> DataSourceList { get; }

        /// <summary>
        /// Gets or sets server serverType value
        /// </summary>
        KeyValuePair<ServerType, string> SelectedServerType { get; set; }

        /// <summary>
        /// Gets or sets server username value
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets server password value
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets server uri
        /// </summary>
        string Uri { get; set; }

        /// <summary>
        /// Gets or sets login succesfully flag
        /// </summary>
        bool LoginSuccessfull { get; }

        /// <summary>
        /// Gets or sets login failed flag
        /// </summary>
        bool LoginFailed { get; }

        /// <summary>
        /// Gets or sets engineering models list
        /// </summary>
        ReactiveList<EngineeringModelRowViewModel> EngineeringModels { get; }

        /// <summary>
        /// Gets the server login command
        /// </summary>
        ReactiveCommand<Unit> LoginCommand { get; }
    }
}
