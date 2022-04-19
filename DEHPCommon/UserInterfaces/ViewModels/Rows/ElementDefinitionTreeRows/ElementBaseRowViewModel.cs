// -------------------------------------------------------------------------------------------------
// <copyright file="ElementBaseRowViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.Events;
    using DEHPCommon.Extensions;
    using DEHPCommon.UserInterfaces.ViewModels.Comparers;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// The Base row class representing an <see cref="ElementBase"/>
    /// </summary>
    /// <typeparam name="T">An <see cref="ElementBase"/> type</typeparam>
    public abstract class ElementBaseRowViewModel<T> : DefinedThingRowViewModel<T>, IElementBaseRowViewModel where T : ElementBase
    {
        /// <summary>
        /// Backing field for <see cref="Owner"/>
        /// </summary>
        private DomainOfExpertise owner;

        /// <summary>
        /// Backing field for <see cref="OwnerShortName"/>
        /// </summary>
        private string ownerShortName;

        /// <summary>
        /// Backing field for <see cref="OwnerName"/>
        /// </summary>
        private string ownerName;
        
        /// <summary>
        /// Gets or sets the Owner
        /// </summary>
        public DomainOfExpertise Owner
        {
            get => this.owner;
            set => this.RaiseAndSetIfChanged(ref this.owner, value);
        }

        /// <summary>
        /// Gets or set the ShortName of <see cref="Owner"/>
        /// </summary>
        public string OwnerShortName
        {
            get => this.ownerShortName;
            set => this.RaiseAndSetIfChanged(ref this.ownerShortName, value);
        }

        /// <summary>
        /// Gets or set the Name of <see cref="Owner"/>
        /// </summary>
        public string OwnerName
        {
            get => this.ownerName;
            set => this.RaiseAndSetIfChanged(ref this.ownerName, value);
        }

        /// <summary>
        /// Updates the properties of this row
        /// </summary>
        private void UpdateProperties()
        {
            this.ModifiedOn = this.Thing.ModifiedOn;

            if (this.Thing.Owner != null)
            {
                this.OwnerShortName = this.Thing.Owner.ShortName;
                this.OwnerName = this.Thing.Owner.Name;
            }

            this.Owner = this.Thing.Owner;
        }

        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public IComparer<IRowViewModelBase<Thing>> ChildRowComparer { get; } = new ElementBaseChildRowComparer();

        /// <summary>
        /// A cache for all <see cref="ParameterBase"/>
        /// </summary>
        protected Dictionary<ParameterBase, IRowViewModelBase<ParameterBase>> ParameterBaseCache;

        /// <summary>
        /// A cache that associates a <see cref="ParameterBase"/> with its <see cref="ParameterGroup"/> in the tree-view
        /// </summary>
        protected Dictionary<ParameterBase, ParameterGroup> ParameterBaseContainerMap;

        /// <summary>
        /// A list of all rows representing all <see cref="ParameterGroup"/> contained by this <see cref="CDP4Common.EngineeringModelData.ElementDefinition"/>
        /// </summary>
        protected Dictionary<Guid, ParameterGroupRowViewModel> ParameterGroupCache;

        /// <summary>
        /// A parameter group - parameter group container mapping
        /// </summary>
        protected Dictionary<Guid, ParameterGroup> ParameterGroupContainment;

        /// <summary>
        /// The cache for the Parameter update's listener
        /// </summary>
        protected Dictionary<ParameterBase, IDisposable> ParameterBaseListener; 
        
        /// <summary>
        /// Backing field for <see cref="ModelCode"/>
        /// </summary>
        private string modelCode;

        /// <summary>
        /// Backing field for <see cref="CurrentDomain"/>
        /// </summary>
        private readonly DomainOfExpertise currentDomain;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementBaseRowViewModel{T}"/> class
        /// </summary>
        /// <param name="elementBase">The associated <see cref="ElementBase"/></param>
        /// <param name="currentDomain">The active <see cref="DomainOfExpertise"/></param>
        /// <param name="session">The associated <see cref="ISession"/></param>
        /// <param name="containerViewModel">The container view-model</param>
        protected ElementBaseRowViewModel(T elementBase, DomainOfExpertise currentDomain, ISession session, IViewModelBase<Thing> containerViewModel) : base(elementBase, session, containerViewModel)
        {
            this.ParameterBaseCache = new Dictionary<ParameterBase, IRowViewModelBase<ParameterBase>>();
            this.ParameterBaseContainerMap = new Dictionary<ParameterBase, ParameterGroup>();
            this.ParameterGroupCache = new Dictionary<Guid, ParameterGroupRowViewModel>();
            this.ParameterGroupContainment = new Dictionary<Guid, ParameterGroup>();
            this.ParameterBaseListener = new Dictionary<ParameterBase, IDisposable>();
            this.currentDomain = currentDomain;            
            this.UpdateOwnerProperties();
            this.WhenAnyValue(vm => vm.Owner).Subscribe(_ => this.UpdateOwnerProperties());
            this.ModelCode = ((IModelCode)this.Thing).ModelCode();
        }

        /// <summary>
        /// Gets or sets the <see cref="HasExcludes"/>. Null if <see cref="ElementUsage"/> is in no options.
        /// </summary>
        public virtual bool? HasExcludes { get; set; } = null;

        /// <summary>
        /// Gets the value indicating whether the row is a top element. Property implemented here to fix binding errors.
        /// </summary>
        public virtual bool IsTopElement { get; set; } = false;

        /// <summary>
        /// Gets the active <see cref="DomainOfExpertise"/>
        /// </summary>
        public virtual DomainOfExpertise CurrentDomain => this.currentDomain;

        /// <summary>
        /// Gets a value indicating whether the value set editors are active
        /// </summary>
        public bool IsValueSetEditorActive => false;

        /// <summary>
        /// Update properties of the Owner
        /// </summary>
        public void UpdateOwnerProperties()
        {
            if (this.Owner != null)
            {
                this.OwnerName = this.Owner.Name;
                this.OwnerShortName = this.Owner.ShortName;
            }
        }

        /// <summary>
        /// Gets the mode-code
        /// </summary>
        public string ModelCode
        {
            get => this.modelCode;
            private set => this.RaiseAndSetIfChanged(ref this.modelCode, value);
        }

        /// <summary>
        /// Update the row containment associated to a <see cref="ParameterBase"/>
        /// </summary>
        /// <param name="parameterBase">The <see cref="ParameterBase"/></param>
        public void UpdateParameterBasePosition(ParameterBase parameterBase)
        {
            try
            {
                var oldContainer = this.ParameterBaseContainerMap[parameterBase];
                var newContainer = parameterBase.Group;
                var associatedRow = this.ParameterBaseCache[parameterBase];

                if ((newContainer != null) && (oldContainer == null))
                {
                    this.ContainedRows.RemoveWithoutDispose(associatedRow);
                    this.ParameterGroupCache[newContainer.Iid].ContainedRows.SortedInsert(associatedRow, ParameterGroupRowViewModel.ChildRowComparer);
                    this.ParameterBaseContainerMap[parameterBase] = newContainer;
                }
                else if ((newContainer == null) && (oldContainer != null))
                {
                    this.ParameterGroupCache[oldContainer.Iid].ContainedRows.RemoveWithoutDispose(associatedRow);
                    this.ContainedRows.SortedInsert(associatedRow, this.ChildRowComparer);
                    this.ParameterBaseContainerMap[parameterBase] = null;
                }
                else if ((newContainer != null) && (newContainer != oldContainer))
                {
                    this.ParameterGroupCache[oldContainer.Iid].ContainedRows.RemoveWithoutDispose(associatedRow);
                    this.ParameterGroupCache[newContainer.Iid].ContainedRows.SortedInsert(associatedRow, ParameterGroupRowViewModel.ChildRowComparer);
                    this.ParameterBaseContainerMap[parameterBase] = newContainer;
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "A problem occured when executing the UpdateParameterBasePosition method.");
            }            
        }

        /// <summary>
        /// Update the row containment associated to a <see cref="ParameterGroup"/>
        /// </summary>
        /// <param name="parameterGroup">The <see cref="ParameterGroup"/></param>
        public void UpdateParameterGroupPosition(ParameterGroup parameterGroup)
        {
            try
            {
                var oldContainer = this.ParameterGroupContainment[parameterGroup.Iid];
                var newContainer = parameterGroup.ContainingGroup;
                var associatedRow = this.ParameterGroupCache[parameterGroup.Iid];

                if (newContainer != null && oldContainer == null)
                {
                    this.ContainedRows.RemoveWithoutDispose(associatedRow);
                    this.ParameterGroupCache[newContainer.Iid].ContainedRows.SortedInsert(associatedRow, ParameterGroupRowViewModel.ChildRowComparer);
                    this.ParameterGroupContainment[parameterGroup.Iid] = newContainer;
                }
                else if (newContainer == null && oldContainer != null)
                {
                    this.ParameterGroupCache[oldContainer.Iid].ContainedRows.RemoveWithoutDispose(associatedRow);
                    this.ContainedRows.SortedInsert(associatedRow, ChildRowComparer);
                    this.ParameterGroupContainment[parameterGroup.Iid] = null;
                }
                else if (newContainer != null && newContainer != oldContainer)
                {
                    this.ParameterGroupCache[oldContainer.Iid].ContainedRows.RemoveWithoutDispose(associatedRow);
                    this.ParameterGroupCache[newContainer.Iid].ContainedRows.SortedInsert(associatedRow, ParameterGroupRowViewModel.ChildRowComparer);
                    this.ParameterGroupContainment[parameterGroup.Iid] = newContainer;
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "A problem occured when executing the UpdateParameterGroupPosition method.");
            }
        }
        
        /// <summary>
        /// Initializes the subscription for this row
        /// </summary>
        protected override void InitializeSubscriptions()
        {
            base.InitializeSubscriptions();

            var selectSubscription = CDPMessageBus.Current.Listen<SelectEvent>()
                .Where(x => x.SelectedThing.ShortName == this.Thing.ShortName && x.SelectedThing.Iid == this.Thing.Iid && (this.Thing.Original != null || this.Thing.Iid == Guid.Empty))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => this.IsSelectedForTransfer = !x.CancelSelection);

            this.Disposables.Add(selectSubscription);

            var ownerListener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.Thing.Owner)
                                   .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(x => { this.OwnerName = this.Thing.Owner.Name; this.OwnerShortName = this.Thing.Owner.ShortName; });
            this.Disposables.Add(ownerListener);
        }

        /// <summary>
        /// Handles the <see cref="ObjectChangedEvent"/>
        /// </summary>
        /// <param name="objectChange">The <see cref="ObjectChangedEvent"/></param>
        protected override void ObjectChangeEventHandler(ObjectChangedEvent objectChange)
        {
            base.ObjectChangeEventHandler(objectChange);
            this.UpdateProperties();
            this.ModelCode = ((IModelCode)this.Thing).ModelCode();
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

            foreach (var row in this.ParameterBaseCache.Values)
            {
                row.Dispose();
            }

            foreach (var groupRow in this.ParameterGroupCache.Values)
            {
                groupRow.Dispose();
            }

            foreach (var listener in this.ParameterBaseListener.Values)
            {
                listener.Dispose();
            }
        }
        
        /// <summary>
        /// Populates the Parameter group rows
        /// </summary>
        /// <param name="elementDefinition">
        /// The element Definition.
        /// </param>
        protected void PopulateParameterGroups(ElementDefinition elementDefinition)
        {
            try
            {
                var definedGroups = elementDefinition.ParameterGroup;

                // remove deleted groups
                var parameterGroups = this.ParameterGroupCache.Values.Select(x => x.Thing).ToList();
                var oldgroup = parameterGroups.Except(definedGroups).ToList();

                foreach (var group in oldgroup)
                {
                    if (group.ContainingGroup == null)
                    {
                        this.ContainedRows.RemoveWithoutDispose(this.ParameterGroupCache[group.Iid]);
                    }
                    else
                    {
                        if (this.ParameterGroupCache.ContainsKey(group.ContainingGroup.Iid))
                        {
                            this.ParameterGroupCache[group.ContainingGroup.Iid].ContainedRows.RemoveWithoutDispose(this.ParameterGroupCache[group.Iid]);
                        }
                    }

                    this.ParameterGroupCache[group.Iid].Dispose();
                    this.ParameterGroupCache.Remove(group.Iid);
                    this.ParameterGroupContainment.Remove(group.Iid);
                }

                var updatedGroups = parameterGroups.Intersect(definedGroups).ToList();

                // create new group rows
                var newgroup = definedGroups.Except(parameterGroups).ToList();
                
                foreach (var group in newgroup)
                {
                    var row = new ParameterGroupRowViewModel(group, this.currentDomain, this.Session, this);
                    this.ParameterGroupCache.Add(group.Iid, row);
                    this.ParameterGroupContainment.Add(group.Iid, group.ContainingGroup);
                }

                // add the new group in the right position in the tree
                foreach (var group in newgroup)
                {
                    if (group.ContainingGroup == null)
                    {
                        this.ContainedRows.SortedInsert(this.ParameterGroupCache[group.Iid], this.ChildRowComparer);
                    }
                    else
                    {
                        var container = this.ParameterGroupCache[group.ContainingGroup.Iid];
                        container.ContainedRows.SortedInsert(this.ParameterGroupCache[group.Iid], ParameterGroupRowViewModel.ChildRowComparer);
                    }
                }

                // Check if ContainingGroup for existing group might have been updated
                foreach (var group in updatedGroups)
                {
                    this.UpdateParameterGroupPosition(group);
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "A problem occured when executing the PopulateParameterGroups method.");
            }
        }

        /// <summary>
        /// Populate the <see cref="ParameterBase"/> rows for this <see cref="ElementBaseRowViewModel{T}"/> row
        /// </summary>
        protected abstract void PopulateParameters();

        /// <summary>
        /// Removes the list of <see cref="ParameterBase"/>
        /// </summary>
        /// <param name="deletedParameterBase">The <see cref="ParameterBase"/>s to remove</param>
        protected void RemoveParameterBase(IEnumerable<ParameterBase> deletedParameterBase)
        {
            try
            {
                foreach (var parameter in deletedParameterBase)
                {
                    IRowViewModelBase<ParameterBase> row;
                    if (!this.ParameterBaseCache.TryGetValue(parameter, out row))
                    {
                        continue;
                    }

                    // remove the row from its container node
                    var group = this.ParameterBaseContainerMap[parameter];
                    if (group == null)
                    {
                        this.ContainedRows.RemoveWithoutDispose(row);
                    }
                    else
                    {
                        this.ParameterGroupCache[group.Iid].ContainedRows.RemoveWithoutDispose(row);
                    }

                    row.Dispose();
                    this.ParameterBaseCache.Remove(parameter);
                    this.ParameterBaseContainerMap.Remove(parameter);

                    this.RemoveParameterBaseListener(parameter);
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "A problem occured when executing the RemoveParameterBase method.");
            }
        }

        /// <summary>
        /// Adds the list of <see cref="ParameterBase"/>
        /// </summary>
        /// <param name="addedParameterBase">The <see cref="ParameterBase"/>s to add</param>
        protected void AddParameterBase(IEnumerable<ParameterBase> addedParameterBase)
        {
            foreach (var parameterBase in addedParameterBase)
            {
                var row = this.AddParameter(parameterBase);

                if (row == null)
                {
                    throw new NotSupportedException("The ParameterBase is neither a Parameter or a Subscription.");
                }

                this.ParameterBaseCache.Add(parameterBase, row);

                var group = parameterBase.Group;
                this.ParameterBaseContainerMap.Add(parameterBase, group);

                if (group == null)
                {
                    this.ContainedRows.SortedInsert(row, ChildRowComparer);
                }
                else
                {
                    if (this.ParameterGroupCache.TryGetValue(group.Iid, out var parameterGroupRowViewModel))
                    {
                        parameterGroupRowViewModel.ContainedRows.SortedInsert(row, ParameterGroupRowViewModel.ChildRowComparer);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the <paramref name="parameterBase"/> depending whether its a <see cref="Parameter"/>, <see cref="ParameterOverride"/> or a <see cref="ParameterSubscription"/>
        /// </summary>
        /// <param name="parameterBase">The <see cref="ParameterBase"/></param>
        /// <returns>A <see cref="IRowViewModelBase{T}"/></returns>
        private IRowViewModelBase<ParameterBase> AddParameter(ParameterBase parameterBase)
        {
            var row = default(IRowViewModelBase<ParameterBase>);

            switch (parameterBase)
            {
                case Parameter parameter:
                    row = new ParameterRowViewModel(parameter, this.Session, this);
                    this.AddParameterOrOverrideListener(parameter);
                    break;
                case ParameterOverride parameterOverride:
                    row = new ParameterOverrideRowViewModel(parameterOverride, this.Session, this);
                    this.AddParameterOrOverrideListener(parameterOverride);
                    break;
                case ParameterSubscription parameterSubscription:
                {
                    row = parameterSubscription.Container switch
                    {
                        Parameter _ => new ParameterSubscriptionRowViewModel(parameterSubscription, this.Session, this),
                        ParameterOverride _ => new ParameterSubscriptionRowViewModel(parameterSubscription, this.Session, this),
                        _ => null
                    };

                    this.AddParameterSubscriptionListener(parameterSubscription);
                    break;
                }
            }

            return row;
        }

        /// <summary>
        /// Updates the list of <see cref="ParameterBase"/>
        /// </summary>
        /// <param name="updatedParameterBase">The <see cref="ParameterBase"/>s to update</param>
        protected void UpdateParameterBase(IEnumerable<ParameterBase> updatedParameterBase)
        {
            foreach (var parameterBase in updatedParameterBase)
            {
                this.UpdateParameterBasePosition(parameterBase);
            }
        }

        /// <summary>
        /// Add the listener associated to the <see cref="ParameterOrOverrideBase"/>
        /// </summary>
        /// <param name="parameterOrOverride">The <see cref="ParameterOrOverrideBase"/></param>
        private void AddParameterOrOverrideListener(ParameterOrOverrideBase parameterOrOverride)
        {
            if (this.ParameterBaseListener.ContainsKey(parameterOrOverride))
            {
                return;
            }

            var listener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(parameterOrOverride)
                .Where(objectChange => objectChange.EventKind == EventKind.Updated && objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.PopulateParameters());
            this.ParameterBaseListener.Add(parameterOrOverride, listener);
        }

        /// <summary>
        /// Add the listener associated to the <see cref="ParameterSubscription"/>
        /// </summary>
        /// <param name="parameterSubscription">The <see cref="ParameterSubscription"/></param>
        private void AddParameterSubscriptionListener(ParameterSubscription parameterSubscription)
        {
            if (this.ParameterBaseListener.ContainsKey(parameterSubscription))
            {
                return;
            }

            var listener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(parameterSubscription)
                .Where(objectChange => objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.PopulateParameters());
            this.ParameterBaseListener.Add(parameterSubscription, listener);
        }

        /// <summary>
        /// Remove the listener associated to the <see cref="ParameterBase"/>
        /// </summary>
        /// <param name="parameterBase">The <see cref="ParameterBase"/></param>
        private void RemoveParameterBaseListener(ParameterBase parameterBase)
        {
            if (parameterBase is Parameter parameter)
            {
                this.RemoveParameterOrOverrideListener(parameter);
                return;
            }

            if (parameterBase is ParameterOverride parameterOverride)
            {
                this.RemoveParameterOrOverrideListener(parameterOverride);
            }

            if (parameterBase is ParameterSubscription parameterSubscription)
            {
                this.RemoveParameterSubscriptionListener(parameterSubscription);
            }
        }

        /// <summary>
        /// Remove the listener associated to the <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameter">The <see cref="Parameter"/></param>
        private void RemoveParameterOrOverrideListener(Parameter parameter)
        {
            if (!this.ParameterBaseListener.TryGetValue(parameter, out var disposable))
            {
                return;
            }

            if (!(this.Thing is ElementDefinition elementDef))
            {
                if (!(this.Thing is ElementUsage usage))
                {
                    return;
                }

                elementDef = usage.ElementDefinition;
            }

            if (!elementDef.Parameter.Contains(parameter))
            {
                disposable.Dispose();
                this.ParameterBaseListener.Remove(parameter);   
            }
        }

        /// <summary>
        /// Remove the listener associated to the <see cref="ParameterOverride"/>
        /// </summary>
        /// <param name="parameterOverride">The <see cref="ParameterOverride"/></param>
        private void RemoveParameterOrOverrideListener(ParameterOverride parameterOverride)
        {
            if (!this.ParameterBaseListener.TryGetValue(parameterOverride, out var disposable) || !(this.Thing is ElementUsage usage))
            {
                return;
            }

            if (!usage.ParameterOverride.Contains(parameterOverride))
            {
                disposable.Dispose();
                this.ParameterBaseListener.Remove(parameterOverride);
            }
        }

        /// <summary>
        /// Remove the listener associated to the <see cref="ParameterSubscription"/>
        /// </summary>
        /// <param name="parameterSubscription">The <see cref="ParameterSubscription"/></param>
        private void RemoveParameterSubscriptionListener(ParameterSubscription parameterSubscription)
        {
            if (!this.ParameterBaseListener.TryGetValue(parameterSubscription, out var disposable))
            {
                return;
            }

            disposable.Dispose();
            this.ParameterBaseListener.Remove(parameterSubscription);
        }

        /// <summary>
        /// Updates the overriding row properties
        /// </summary>
        public abstract void UpdateChildren();
    }
}