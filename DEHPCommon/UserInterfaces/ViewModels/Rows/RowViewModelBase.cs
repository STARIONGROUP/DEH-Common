// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RowViewModelBase.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.Rows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.Converters;
    using DEHPCommon.Enumerators;
    using DEHPCommon.Events;
    using DEHPCommon.Mvvm;
    using DEHPCommon.Services.TooltipService;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.Utilities;

    using ReactiveUI;

    /// <summary>
    /// The Base view-model class for rows
    /// </summary>
    /// <typeparam name="T">The <see cref="Thing"/> represented by the row</typeparam>
    public abstract class RowViewModelBase<T> : ViewModelBase<T>, IRowViewModelBase<T> where T : Thing
    {
        /// <summary>
        /// Backing field for <see cref="Index"/>
        /// </summary>
        private int index;

        /// <summary>
        /// Backing property for the <see cref="IsHighlighted"/>
        /// </summary>
        private bool isHighlighted;

        /// <summary>
        /// Backing property for the <see cref="IsSelectedForTransfer"/>
        /// </summary>
        private bool isSelectedForTransfer;

        /// <summary>
        /// Backing field for <see cref="RowStatus"/>
        /// </summary>
        private RowStatusKind rowStatus;

        /// <summary>
        /// Backing field for <see cref="IsExpanded"/>
        /// </summary>
        private bool isExpanded;

        /// <summary>
        /// Backing field for <see cref="Tooltip"/>
        /// </summary>
        private string tooltip;

        /// <summary>
        /// Backing field for <see cref="ThingStatus"/>
        /// </summary>
        private ThingStatus thingStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="RowViewModelBase{T}"/> class.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> represented by the row</param>
        /// <param name="session">The session</param>
        /// <param name="containerViewModel">The parent Row</param>
        protected RowViewModelBase(T thing, ISession session, IViewModelBase<Thing> containerViewModel = null) : base(thing, session)
        {
            this.ContainedRows = new DisposableReactiveList<IRowViewModelBase<Thing>>();
            this.ContainerViewModel = containerViewModel;
            this.HighlightCancelDisposables = new List<IDisposable>();

            this.TopContainerViewModel = this.ContainerViewModel;

            if (this.ContainerViewModel is IRowViewModelBase<Thing> rowContainer)
            {
                this.TopContainerViewModel = rowContainer.TopContainerViewModel;
            }
            
            if (this.Thing is NotThing)
            {
                return;
            }

            this.Initialize();
        }
        
        /// <summary>
        /// Used to call virtual member when this gets initialized
        /// </summary>
        private void Initialize()
        {
            this.InitializeSubscriptions();
            this.UpdateTooltip();
        }

        /// <summary>
        /// Gets or sets a value representing the <see cref="RowStatusKind"/>
        /// </summary>
        /// <remarks>
        /// The default is RowStatusKind.Active
        /// </remarks>
        public RowStatusKind RowStatus
        {
            get => this.rowStatus;
            set => this.RaiseAndSetIfChanged(ref this.rowStatus, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="ThingStatus"/>
        /// </summary>
        public ThingStatus ThingStatus
        {
            get => this.thingStatus;
            protected set => this.RaiseAndSetIfChanged(ref this.thingStatus, value);
        }

        /// <summary>
        /// Gets or sets the Contained <see cref="IRowViewModelBase{T}"/>
        /// </summary>
        public DisposableReactiveList<IRowViewModelBase<Thing>> ContainedRows { get; protected set; }

        /// <summary>
        /// Gets or sets the parent <see cref="IViewModelBase{T}"/>
        /// </summary>
        public IViewModelBase<Thing> ContainerViewModel { get; protected set; }

        /// <summary>
        /// Gets the top container <see cref="IViewModelBase{T}"/>
        /// </summary>
        public IViewModelBase<Thing> TopContainerViewModel { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the row is highlighted
        /// </summary>
        public bool IsHighlighted
        {
            get => this.isHighlighted;
            set => this.RaiseAndSetIfChanged(ref this.isHighlighted, value);
        }

        /// <summary>
        /// Gets a value indicating whether the row is highlighted
        /// </summary>
        public bool IsSelectedForTransfer
        {
            get => this.isSelectedForTransfer;
            set => this.RaiseAndSetIfChanged(ref this.isSelectedForTransfer, value);
        }

        /// <summary>
        /// Gets a value indicating whether the row is favorited
        /// </summary>
        public bool IsFavorite
        {
            get => this.isHighlighted;
            set => this.RaiseAndSetIfChanged(ref this.isHighlighted, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this row should be expanded or collapsed
        /// </summary>
        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.RaiseAndSetIfChanged(ref this.isExpanded, value);
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
        /// Gets or sets the index of the row
        /// </summary>
        /// <remarks>
        /// this property is used in the case of <see cref="OrderedItemList{T}"/>
        /// </remarks>
        public int Index
        {
            get => this.index;
            set => this.RaiseAndSetIfChanged(ref this.index, value);
        }

        /// <summary>
        /// Gets or sets the tooltip
        /// </summary>
        public string Tooltip
        {
            get => this.tooltip;
            protected set => this.RaiseAndSetIfChanged(ref this.tooltip, value);
        }

        /// <summary>
        /// Gets the list of <see cref="IDisposable"/> objects that are referenced by this class used for cancelation of highlight.
        /// </summary>
        protected List<IDisposable> HighlightCancelDisposables { get; private set; }

        /// <summary>
        /// The ClassKind of the current thing
        /// </summary>
        public virtual string RowType =>
            (this.Thing == null)
                ? "-"
                : new CamelCaseToSpaceConverter().Convert(this.Thing.ClassKind, null, null, null)?.ToString();

        /// <summary>
        /// Clears the row highlighting for itself and its children.
        /// </summary>
        public void ClearRowHighlighting()
        {
            if (this.IsHighlighted)
            {
                this.IsHighlighted = false;
            }
        }

        /// <summary>
        /// Initializes the subscriptions
        /// </summary>
        protected virtual void InitializeSubscriptions()
        {
            var highlightSubscription = CDPMessageBus.Current.Listen<HighlightEvent>(this.Thing)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.HighlightEventHandler());

            this.Disposables.Add(highlightSubscription);

            // category highlighting
            if (this.Thing is ICategorizableThing thingAsCategorizableThing)
            {
                this.Disposables.AddRange(
                    thingAsCategorizableThing.Category.Select(
                        category => CDPMessageBus.Current.Listen<HighlightByCategoryEvent>(category)
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(_ => this.HighlightEventHandler())));
            }

            this.Disposables.Add(CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(Relationship))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.UpdateThingStatus()));
        }

        /// <summary>
        /// Update the <see cref="ThingStatus"/> property
        /// </summary>
        protected virtual void UpdateThingStatus()
        {
        }
        
        /// <summary>
        /// The event-handler that is invoked by the subscription that listens for highlight of row
        /// on the <see cref="Thing"/> that is being represented by the view-model
        /// </summary>
        protected virtual void HighlightEventHandler()
        {
            this.IsHighlighted = true;

            // add a subscription to handle cancel of highlight
            var cancelHighlightSubscription = CDPMessageBus.Current.Listen<CancelHighlightEvent>()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.CancelHighlightEventHandler());

            this.HighlightCancelDisposables.Add(cancelHighlightSubscription);
        }

        /// <summary>
        /// The event-handler that is invoked by the subscription that listens for cancel of highlight of row
        /// on the <see cref="Thing"/> that is being represented by the view-model
        /// </summary>
        protected virtual void CancelHighlightEventHandler()
        {
            this.IsHighlighted = false;

            foreach (var cancelationSubscription in this.HighlightCancelDisposables)
            {
                cancelationSubscription.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// a value indicating whether the class is being disposed of
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach (var disposable in this.HighlightCancelDisposables)
            {
                disposable.Dispose();
            }

            foreach (var row in this.ContainedRows)
            {
                row.Dispose();
            }
        }

        /// <summary>
        /// The event-handler that is invoked by the subscription that listens for updates
        /// on the <see cref="Thing"/> that is being represented by the view-model
        /// </summary>
        /// <param name="objectChange"> The payload of the event that is being handled </param>
        protected override void ObjectChangeEventHandler(ObjectChangedEvent objectChange)
        {
            base.ObjectChangeEventHandler(objectChange);
            this.UpdateTooltip();
        }

        /// <summary>
        /// Update this <see cref="Tooltip"/>
        /// </summary>
        protected virtual void UpdateTooltip()
        {
            this.Tooltip = this.Thing.Tooltip();
        }
    }
}
