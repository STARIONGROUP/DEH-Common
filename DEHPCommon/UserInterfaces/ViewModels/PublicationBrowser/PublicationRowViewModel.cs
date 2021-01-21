// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicationRowViewModel.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
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

namespace DEHPCommon.UserInterfaces.ViewModels.PublicationBrowser
{
    using System.Globalization;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using CDP4Dal;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Rows;

    /// <summary>
    /// The extended row class representing an <see cref="Publication"/>
    /// </summary>
    public class PublicationRowViewModel : RowViewModelBase<Publication>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicationRowViewModel"/> class
        /// </summary>
        /// <param name="publication">The <see cref="Publication"/> associated with this row</param>
        /// <param name="session">The session</param>
        /// <param name="containerViewModel">The container <see cref="IViewModelBase{T}"/></param>
        public PublicationRowViewModel(Publication publication, ISession session, IViewModelBase<Thing> containerViewModel)
            : base(publication, session, containerViewModel)
        {
        }

        /// <summary>
        /// Gets the name of the publication node. In this case references the <see cref="Thing.CreatedOn"/> property.
        /// </summary>
        /// <returns>The <see cref="Thing.CreatedOn"/> property in YYYY-MM-DD format.</returns>
        public string Name
        {
            get { return this.Thing.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the string representation of <see cref="DomainOfExpertiese"/>s that are owners of of one or more publishedParameter(s)
        /// </summary>
        public string OwnerShortName
        {
            get { return string.Join(", ", this.Thing.Domain.Select(d => d.ShortName)); }
        }

        /// <summary>
        /// Gets the empty string to put in modelCode column. Needed to avoid slowdowns connected to binding errors.
        /// </summary>
        public string ModelCode
        {
            get { return string.Empty; }
        }
    }
}