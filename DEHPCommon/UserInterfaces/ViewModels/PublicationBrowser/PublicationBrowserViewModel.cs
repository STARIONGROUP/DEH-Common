// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicationBrowserViewModel.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Reactive.Linq;

    using CDP4Common.CommonData;

    using DEHPCommon.HubController.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="PublicationBrowserViewModel"/> is a View Model that is responsible for managing the data and interactions with that data for a view
    /// that shows all the <see cref="Thing"/>s contained by a data-source following the containment tree that is modelled in 10-25 and the CDP4 extensions.
    /// </summary>
    public class PublicationBrowserViewModel : ReactiveObject, IPublicationBrowserViewModel, IDisposable
    {
        /// <summary>
        /// The <see cref="IHubController"/>
        /// </summary>
        private readonly IHubController hubController;

        /// <summary>
        /// Backing field for <see cref="IsBusy"/>
        /// </summary>
        private bool? isBusy;

        /// <summary>
        /// Backing field for <see cref="PublicationsViewModel"/>
        /// </summary>
        private PublicationsViewModel publicationsViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicationBrowserViewModel"/> class.
        /// </summary>
        /// <param name="hubController">The <see cref="IHubController"/></param>
        public PublicationBrowserViewModel(IHubController hubController)
        {
            this.hubController = hubController;
            this.Caption = "Publication Browser";

            this.WhenAnyValue(x => x.hubController.OpenIteration).ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    this.IsBusy = true;

                    if (this.hubController.IsSessionOpen && this.hubController.OpenIteration != null)
                    {
                        this.ToolTip = $"{this.hubController.Session.DataSourceUri}\n{this.hubController.Session.ActivePerson.Name}";
                        this.PublicationsViewModel = new PublicationsViewModel(this.hubController.OpenIteration, this.hubController.Session);
                    }
                    else
                    {
                        this.PublicationsViewModel = null;
                    }

                    this.IsBusy = false;
                });
        }

        /// <summary>
        /// Gets or sets a value indicating whether the browser is busy
        /// </summary>
        public bool? IsBusy
        {
            get => this.isBusy;
            set => this.RaiseAndSetIfChanged(ref this.isBusy, value);
        }

        /// <summary>
        /// Gets the <see cref="PublicationsViewModel"/> which holds the data used by the browser 
        /// </summary>
        public PublicationsViewModel PublicationsViewModel
        {
            get => this.publicationsViewModel;
            set => this.RaiseAndSetIfChanged(ref this.publicationsViewModel, value);
        }

        /// <summary>
        /// Gets the Caption of the control
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Gets the tooltip of the control
        /// </summary>
        public string ToolTip { get; private set; }

        /// <summary>
        /// Gets the list of <see cref="IDisposable"/> objects that are referenced by this class
        /// </summary>
        protected IEnumerable<IDisposable> Disposables { get; } = new List<IDisposable>();

        /// <summary>
        /// Dispose of this <see cref="PublicationBrowserViewModel"/>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of this <see cref="PublicationBrowserViewModel"/>
        /// </summary>
        /// <param name="disposing">Assert whether to dispose of this.<see cref="IDisposable"/></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            foreach (var disposable in this.Disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
