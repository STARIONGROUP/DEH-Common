// -------------------------------------------------------------------------------------------------
// <copyright file="PublicationDomainOfExpertiseRowViewModel.cs" company="RHEA System S.A.">
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
    using System;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Rows;

    using ReactiveUI;

    /// <summary>
    /// The view-model for the <see cref="DomainOfExpertiseRowViewModel"/> view
    /// </summary>
    public class PublicationDomainOfExpertiseRowViewModel : RowViewModelBase<DomainOfExpertise>, IPublishableRow
    {
        /// <summary>
        /// Backing field for <see cref="ToBePublished"/>
        /// </summary>
        private bool toBePublished;

        /// <summary>
        /// Backing field for <see cref="Name"/>
        /// </summary>
        private string name;

        /// <summary>
        /// Backing field for <see cref="ShortName"/>
        /// </summary>
        private string shortName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainOfExpertiseRowViewModel"/> class
        /// </summary>
        /// <param name="domainOfExpertise">The <see cref="DomainOfExpertise"/> associated with this row</param>
        /// <param name="session">The session</param>
        /// <param name="containerViewModel">The <see cref="IViewModelBase{T}"/> that is the container of this <see cref="IRowViewModelBase{T}"/></param>
        public PublicationDomainOfExpertiseRowViewModel(DomainOfExpertise domainOfExpertise, ISession session, IViewModelBase<Thing> containerViewModel)
            : base(domainOfExpertise, session, containerViewModel)
        {
            this.UpdateProperties();
            this.WhenAnyValue(vm => vm.ToBePublished).Subscribe(_ => this.ToBePublishedChanged());
        }

        /// <summary>
        /// Gets or sets a value indicating whether the row is to be published
        /// </summary>
        public bool ToBePublished
        {
            get { return this.toBePublished; }
            set { this.RaiseAndSetIfChanged(ref this.toBePublished, value); }
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.RaiseAndSetIfChanged(ref this.name, value); }
        }

        /// <summary>
        /// Gets or sets the ShortName
        /// </summary>
        public string ShortName
        {
            get { return this.shortName; }
            set { this.RaiseAndSetIfChanged(ref this.shortName, value); }
        }

        /// <summary>
        /// The event-handler that is invoked by the subscription that listens for updates
        /// on the <see cref="Thing"/> that is being represented by the view-model
        /// </summary>
        /// <param name="objectChange">
        /// The payload of the event that is being handled
        /// </param>
        protected override void ObjectChangeEventHandler(ObjectChangedEvent objectChange)
        {
            base.ObjectChangeEventHandler(objectChange);
            this.UpdateProperties();
        }

        /// <summary>
        /// Exute the change to publication selection.
        /// </summary>
        private void ToBePublishedChanged()
        {
            foreach (var row in this.ContainedRows.OfType<IPublishableRow>())
            {
                row.ToBePublished = this.ToBePublished;
            }
        }

        /// <summary>
        /// Updates the properties of this row
        /// </summary>
        private void UpdateProperties()
        {
            this.ModifiedOn = this.Thing.ModifiedOn;
            this.Name = this.Thing.Name;
            this.ShortName = this.Thing.ShortName;
        }
    }
}
