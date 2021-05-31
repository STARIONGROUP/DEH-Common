// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserViewModelBase.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.CommonData;

    using CDP4Dal;
    using CDP4Dal.DAL;
    using CDP4Dal.Events;
    using CDP4Dal.Permission;

    using DEHPCommon.Mvvm;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.Views.ObjectBrowser;

    using NLog;

    using ReactiveUI;

    /// <summary>
    /// The view model that represents the root element of hierarchical data to be displayed in the <see cref="ObjectBrowser"/>
    /// </summary>
    public abstract class BrowserViewModelBase : ReactiveObject, IHaveContainedRows, IBrowsableThing, IDisposable
    {
        /// <summary>
        /// The NLog logger
        /// </summary>
        protected Logger Logger;
        
        /// <summary>
        /// Backing field for <see cref="IsExpanded"/>
        /// </summary>
        private bool isExpanded;

        /// <summary>
        /// a value indicating whether the instance is disposed
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Backing field for <see cref="RevisionNumber"/>
        /// </summary>
        private int revisionNumber;

        /// <summary>
        /// Backing field for <see cref="ModifiedOn"/> property.
        /// </summary>
        private DateTime modifiedOn;

        /// <summary>
        /// Backing Field For isBusy
        /// </summary>
        private bool? isBusy;
        
        /// <summary>
        /// Gets or sets the name of this browsable thing
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the list of rows representing a <see cref="Thing"/>
        /// </summary>
        /// <remarks>This was made into a list of generic row to use the ReactiveList extension</remarks>
        public DisposableReactiveList<IRowViewModelBase<Thing>> ContainedRows { get; } = new DisposableReactiveList<IRowViewModelBase<Thing>>();
        
        /// <summary>
        /// Gets the <see cref="ISession"/>
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        /// Gets the <see cref="IPermissionService"/>
        /// </summary>
        /// <remarks>
        /// This is a convenience property refernces the <see cref="CDP4Dal.Permission.PermissionService"/>
        /// </remarks>
        public IPermissionService PermissionService { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the browser is busy
        /// </summary>
        public bool? IsBusy
        {
            get => this.isBusy;
            set => this.RaiseAndSetIfChanged(ref this.isBusy, value);
        }

        /// <summary>
        /// Gets the revision number of the <see cref="Thing"/> that is represented by the view-model when
        /// it was last updated
        /// </summary>
        public int RevisionNumber
        {
            get => this.revisionNumber;
            set => this.RaiseAndSetIfChanged(ref this.revisionNumber, value);
        }

        /// <summary>
        /// Gets or sets the <see ref="DateTime"/> at which the <see ref="Thing"/> was last modified.
        /// </summary>
        [CDPVersion("1.1.0")]
        public DateTime ModifiedOn
        {
            get => this.modifiedOn;
            set => this.RaiseAndSetIfChanged(ref this.modifiedOn, value);
        }

        /// <summary>
        /// Gets the Uri of the <see cref="IDal"/> from which the <see cref="Thing"/> represented in the view-model comes from
        /// </summary>
        public Uri IDalUri { get; protected set; }

        /// <summary>
        /// Gets the list of <see cref="IDisposable"/> objects that are referenced by this class
        /// </summary>
        protected List<IDisposable> Disposables { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this row should be expanded or collapsed
        /// </summary>
        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.RaiseAndSetIfChanged(ref this.isExpanded, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase{T}"/> class.
        /// </summary>
        /// <param name="session"> The session this view model belongs to. </param>
        protected BrowserViewModelBase(ISession session)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);

            this.PermissionService = session.PermissionService;
            this.Disposables = new List<IDisposable>();
            this.Session = session;
        }
        
        /// <summary>
        /// Expands the current row and all contained rows along the containment hierarchy
        /// </summary>
        public void ExpandAllRows()
        {
            this.IsExpanded = true;

            foreach (var row in this.ContainedRows)
            {
                row.ExpandAllRows();
            }
        }

        /// <summary>
        /// Collapses the current row and all contained rows along the containment hierarchy
        /// </summary>
        public void CollapseAllRows()
        {
            this.IsExpanded = false;

            foreach (var row in this.ContainedRows)
            {
                row.CollapseAllRows();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// a value indicating whether the class is being disposed of
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing) //Free any other managed objects here
            {
                // Clear all property values that maybe have been set
                // when the class was instantiated
                this.RevisionNumber = 0;

                if (this.Disposables != null)
                {
                    foreach (var disposable in this.Disposables)
                    {
                        disposable.Dispose();
                    }
                }
                else
                {
                    this.Logger.Trace("The Disposables collection of the {0} is null", this.GetType().Name);
                }

                foreach (var row in this.ContainedRows)
                {
                    row.Dispose();
                }
            }

            // Indicate that the instance has been disposed.
            this.isDisposed = true;
        }

        /// <summary>
        /// The event-handler that is invoked by the subscription that listens for updates
        /// on the <see cref="Thing"/> that is being represented by the view-model
        /// </summary>
        /// <param name="objectChange">
        /// The payload of the event that is being handled
        /// </param>
        protected virtual void ObjectChangeEventHandler(ObjectChangedEvent objectChange)
        {
            this.RevisionNumber = objectChange.ChangedThing.RevisionNumber;
        }
    }
}
