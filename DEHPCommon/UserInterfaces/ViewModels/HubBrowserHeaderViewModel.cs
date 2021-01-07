// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubBrowserHeaderViewModel.cs" company="RHEA System S.A.">
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

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.Views;

    using ReactiveUI;

    /// <summary>
    /// View model for <see cref="HubBrowserHeader"/>
    /// </summary>
    public class HubBrowserHeaderViewModel : ReactiveObject, IHubBrowserHeaderViewModel
    {
        /// <summary>
        /// The <see cref="IHubController"/>
        /// </summary>
        private readonly IHubController hubController;

        /// <summary>
        /// Backing field for the <see cref="Model"/>
        /// </summary>
        private string model;

        /// <summary>
        /// Gets or sets the <see cref="EngineeringModel"/> Name
        /// </summary>
        public string Model
        {
            get => this.model;
            set => this.RaiseAndSetIfChanged(ref this.model, value);
        }

        /// <summary>
        /// Backing field for the <see cref="DataSource"/>
        /// </summary>
        private string dataSource;

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> of the connected data source
        /// </summary>
        public string DataSource
        {
            get => this.dataSource;
            set => this.RaiseAndSetIfChanged(ref this.dataSource, value);
        }
        
        /// <summary>
        /// Backing field for the <see cref="Iteration"/>
        /// </summary>
        private string iteration;

        /// <summary>
        /// Gets or sets the <see cref="CDP4Common.EngineeringModelData.Iteration"/> number
        /// </summary>
        public string Iteration
        {
            get => this.iteration;
            set => this.RaiseAndSetIfChanged(ref this.iteration, value);
        }

        /// <summary>
        /// Backing field for the <see cref="Person"/>
        /// </summary>
        private string person;

        /// <summary>
        /// Gets or sets the <see cref="Person"/> name
        /// </summary>
        public string Person
        {
            get => this.person;
            set => this.RaiseAndSetIfChanged(ref this.person, value);
        }

        /// <summary>
        /// Backing field for the <see cref="Option"/>
        /// </summary>
        private string option;

        /// <summary>
        /// Gets or sets the <see cref="CDP4Common.EngineeringModelData.Option"/> name
        /// </summary>
        public string Option
        {
            get => this.option;
            set => this.RaiseAndSetIfChanged(ref this.option, value);
        }

        /// <summary>
        /// Backing field for the <see cref="Domain"/>
        /// </summary>
        private string domain;

        /// <summary>
        /// Gets or sets the <see cref="DomainOfExpertise"/> name
        /// </summary>
        public string Domain
        {
            get => this.domain;
            set => this.RaiseAndSetIfChanged(ref this.domain, value);
        }

        /// <summary>
        /// Initializes a new <see cref="HubBrowserHeaderViewModel"/>
        /// </summary>
        /// <param name="hubController">The <see cref="IHubController"/></param>
        public HubBrowserHeaderViewModel(IHubController hubController)
        {
            this.hubController = hubController;

            this.WhenAnyValue(x => x.hubController.OpenIteration).ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(this.UpdateProperties);
        }

        /// <summary>
        /// Updates this view model properties
        /// </summary>
        /// <param name="openIteration"></param>
        private void UpdateProperties(Iteration openIteration)
        {
            if (openIteration is null)
            {
                this.Model = string.Empty;
                this.DataSource = string.Empty;
                this.Iteration = string.Empty;
                this.Person = string.Empty;
                this.Domain = string.Empty;
                this.Option = string.Empty;
            }
            else
            {
                this.Model = this.hubController.OpenIteration.GetContainerOfType<EngineeringModel>()?.EngineeringModelSetup?.Name;
                this.DataSource = this.hubController.Session.DataSourceUri;
                this.Iteration = this.hubController.OpenIteration.IterationSetup.IterationNumber.ToString();
                this.Person = this.hubController.Session.ActivePerson.Name;
                this.Domain = this.hubController.CurrentDomainOfExpertise.Name;
                this.Option = this.hubController.OpenIteration.DefaultOption.Name;
            }
        }
    }
}
