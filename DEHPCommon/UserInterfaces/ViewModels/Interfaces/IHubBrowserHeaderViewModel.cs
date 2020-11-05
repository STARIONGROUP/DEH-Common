// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHubBrowserHeaderViewModel.cs" company="RHEA System S.A.">
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

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    /// <summary>
    /// Interface definition for <see cref="HubBrowserHeaderViewModel"/>
    /// </summary>
    public interface IHubBrowserHeaderViewModel
    {
        /// <summary>
        /// Gets or sets the <see cref="EngineeringModel"/> Name
        /// </summary>
        string Model { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> of the connected data source
        /// </summary>
        string DataSource { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CDP4Common.EngineeringModelData.Iteration"/> number
        /// </summary>
        string Iteration { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Person"/> name
        /// </summary>
        string Person { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CDP4Common.EngineeringModelData.Option"/> name
        /// </summary>
        string Option { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DomainOfExpertise"/> name
        /// </summary>
        string Domain { get; set; }
    }
}
