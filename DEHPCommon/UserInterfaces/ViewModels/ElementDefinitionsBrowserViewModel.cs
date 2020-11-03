// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionsBrowserViewModel.cs" company="RHEA System S.A.">
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
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;
    using CDP4Dal.Operations;
    using CDP4Dal.Permission;

    using DEHPCommon.Enumerators;
    using DEHPCommon.Events;
    using DEHPCommon.Extensions;
    using DEHPCommon.Mvvm;
    using DEHPCommon.UserInterfaces.ViewModels.Comparers;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    using ReactiveUI;

    /// <summary>
    /// Represent the view-model of the browser that displays all the <see cref="ElementDefinition"/>s in one <see cref="Iteration"/>
    /// </summary>
    public class ElementDefinitionsBrowserViewModel : BrowserViewModel<Iteration>
    {
        /// <summary>
        /// a <see cref="ElementDefinitionBrowserChildComparer"/> that is used to assist in sorted inserts
        /// </summary>
        private static readonly ElementDefinitionBrowserChildComparer RowComparer = new ElementDefinitionBrowserChildComparer();

        /// <summary>
        /// Backing field for <see cref="CurrentModel"/>
        /// </summary>
        private string currentModel;

        /// <summary>
        /// Backing field for <see cref="CurrentIteration"/>
        /// </summary>
        private int currentIteration;

        /// <summary>
        /// Backing field for the <see cref="CanCreateParameterGroup"/> property
        /// </summary>
        private bool canCreateParameterGroup;

        /// <summary>
        /// Backing field for the <see cref="CanCreateElementDefinition"/> property
        /// </summary>
        private bool canCreateElementDefinition;

        /// <summary>
        /// Backing field for <see cref="CanCreateSubscription"/>
        /// </summary>
        private bool canCreateSubscription;

        /// <summary>
        /// Backing field for <see cref="CanCreateOverride"/>
        /// </summary>
        private bool canCreateOverride;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDefinitionsBrowserViewModel"/> class
        /// </summary>
        /// <param name="iteration">The associated <see cref="Iteration"/></param>
        /// <param name="session">The session</param>
        public ElementDefinitionsBrowserViewModel(Iteration iteration, ISession session) : base(iteration, session)
        {
            this.Name = nameof(ElementDefinition);
            this.UpdateElementDefinition();

            this.InitializeCommands();
            this.AddSubscriptions();
            this.UpdateProperties();
        }

        /// <summary>
        /// Gets the view model current <see cref="EngineeringModelSetup"/>
        /// </summary>
        public EngineeringModelSetup CurrentEngineeringModelSetup => this.Thing.IterationSetup.GetContainerOfType<EngineeringModelSetup>();

        /// <summary>
        /// Gets the current model caption to be displayed in the browser
        /// </summary>
        public string CurrentModel
        {
            get => this.currentModel;
            private set => this.RaiseAndSetIfChanged(ref this.currentModel, value);
        }

        /// <summary>
        /// Gets the current iteration caption to be displayed in the browser
        /// </summary>
        public int CurrentIteration
        {
            get => this.currentIteration;
            private set => this.RaiseAndSetIfChanged(ref this.currentIteration, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ISession"/> can create a <see cref="ParameterGroup"/>
        /// </summary>
        public bool CanCreateParameterGroup
        {
            get => this.canCreateParameterGroup;
            set => this.RaiseAndSetIfChanged(ref this.canCreateParameterGroup, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ISession"/> can create a <see cref="ElementDefinition"/>
        /// </summary>
        public bool CanCreateElementDefinition
        {
            get => this.canCreateElementDefinition;
            set => this.RaiseAndSetIfChanged(ref this.canCreateElementDefinition, value);
        }

        /// <summary>
        /// Gets a value indicating whether the create subscription command shall be enabled
        /// </summary>
        public bool CanCreateSubscription
        {
            get => this.canCreateSubscription;
            private set => this.RaiseAndSetIfChanged(ref this.canCreateSubscription, value);
        }

        /// <summary>
        /// Gets a value indicating whether the create override command shall be enabled
        /// </summary>
        public bool CanCreateOverride
        {
            get => this.canCreateOverride;
            private set => this.RaiseAndSetIfChanged(ref this.canCreateOverride, value);
        }

        /// <summary>
        /// Gets the <see cref="ReactiveCommand"/> to Copy Model Code to clipboard <see cref="ParameterRowViewModel"/>
        /// </summary>
        public ReactiveCommand<object> CopyModelCodeToClipboardCommand { get; private set; }

        /// <summary>
        /// Gets the <see cref="ReactiveCommand"/> used to show the usages of specified element definition
        /// </summary>
        public ReactiveCommand<object> HighlightElementUsagesCommand { get; private set; }
        
        /// <summary>
        /// Initializes the create <see cref="ReactiveCommand"/> that allow a user to create the different kinds of <see cref="ParameterType"/>s
        /// </summary>
        protected void InitializeCommands()
        {
            this.ComputeNotContextDependentPermission();

            this.HighlightElementUsagesCommand = ReactiveCommand.Create();
            this.HighlightElementUsagesCommand.Subscribe(_ => this.ExecuteHighlightElementUsagesCommand());

            this.CopyModelCodeToClipboardCommand = ReactiveCommand.Create();
            this.CopyModelCodeToClipboardCommand.Subscribe(_ => this.ExecuteCopyModelCodeToClipboardCommand());
        }

        /// <summary>
        /// Compute the permissions to create the different kinds of <see cref="ParameterType"/>s using the <see cref="IPermissionService"/>
        /// </summary>
        public void ComputePermission()
        {
            base.ComputePermission();
            if (this.SelectedThing == null)
            {
                return;
            }

            if (!(this.SelectedThing is ParameterOrOverrideBaseRowViewModel parameterRow))
            {
                return;
            }

            this.Session.OpenIterations.TryGetValue(this.Thing, out var tuple);

            if (parameterRow.Thing is Parameter parameter)
            {
                if (tuple != null)
                {
                    this.CanCreateOverride = this.SelectedThing.ContainerViewModel is ElementUsageRowViewModel
                                             && ((parameter.Owner == tuple.Item1) || parameter.AllowDifferentOwnerOfOverride)
                                             && this.PermissionService.CanWrite(ClassKind.ParameterOverride, this.SelectedThing.ContainerViewModel.Thing);

                    this.CanCreateSubscription = this.SelectedThing.ContainerViewModel is ElementDefinitionRowViewModel
                                                 && (parameter.Owner != tuple.Item1)
                                                 && this.PermissionService.CanWrite(ClassKind.ParameterSubscription, this.SelectedThing.Thing);
                }

                return;
            }

            if (tuple != null)
            {
                this.CanCreateSubscription = this.SelectedThing.ContainerViewModel is ElementUsageRowViewModel
                                             && (((ParameterOverride)parameterRow.Thing).Owner != tuple.Item1)
                                             && this.PermissionService.CanWrite(ClassKind.ParameterSubscription, this.SelectedThing.Thing);
            }
        }

        /// <summary>
        /// Populate the <see cref="ContextMenuItemViewModel"/>s of the current browser
        /// </summary>
        public override void PopulateContextMenu()
        {
            base.PopulateContextMenu();

            if (this.SelectedThing is IModelCodeRowViewModel)
            {
                this.ContextMenu.Add(new ContextMenuItemViewModel("Copy Model Code to Clipboard", "", this.CopyModelCodeToClipboardCommand, MenuItemKind.None, ClassKind.NotThing));
            }

            if (this.SelectedThing is ElementDefinitionRowViewModel elementDefRow)
            {
                this.ContextMenu.Insert(1, new ContextMenuItemViewModel("Highlight Element Usages", "", this.HighlightElementUsagesCommand, MenuItemKind.Highlight, ClassKind.ElementUsage));
                return;
            }

            if (this.SelectedThing is ElementUsageRowViewModel usageRow)
            {
                this.ContextMenu.Add(new ContextMenuItemViewModel("Navigates to Element Definition", "", this.ChangeFocusCommand, MenuItemKind.Navigate, ClassKind.ElementDefinition));

                if (this.SelectedThing.ContainedRows.Count > 0)
                {
                    this.ContextMenu.Add(
                        this.SelectedThing.IsExpanded
                            ? new ContextMenuItemViewModel("Collapse Rows", "", this.CollpaseRowsCommand, MenuItemKind.None, ClassKind.NotThing) 
                            : new ContextMenuItemViewModel("Expand Rows", "", this.ExpandRowsCommand, MenuItemKind.None, ClassKind.NotThing));
                }

                if (this.SelectedThing is IModelCodeRowViewModel)
                {
                    this.ContextMenu.Add(new ContextMenuItemViewModel("Copy Model Code to Clipboard", "", this.CopyModelCodeToClipboardCommand, MenuItemKind.None, ClassKind.NotThing));
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="DomainChangedEvent"/>
        /// </summary>
        /// <param name="domainChangeEvent">The <see cref="DomainChangedEvent"/></param>
        protected override void UpdateDomain(DomainChangedEvent domainChangeEvent)
        {
            base.UpdateDomain(domainChangeEvent);
            this.ContainedRows.ClearAndDispose();
            this.UpdateElementDefinition();
        }

        /// <summary>
        /// The <see cref="ObjectChangedEvent"/> event-handler.
        /// </summary>
        /// <param name="objectChange">The <see cref="ObjectChangedEvent"/></param>
        protected override void ObjectChangeEventHandler(ObjectChangedEvent objectChange)
        {
            base.ObjectChangeEventHandler(objectChange);
            this.UpdateElementDefinition();
            this.UpdateProperties();
        }

        /// <summary>
        /// executes the <see cref="ChangeFocusCommand"/>
        /// </summary>
        protected override void ExecuteChangeFocusCommand()
        {
            var usage = (ElementUsage)this.SelectedThing.Thing;
            var definitionRow = this.ContainedRows.SingleOrDefault(x => x.Thing == usage.ElementDefinition);
            
            if (definitionRow != null)
            {
                this.SelectedThing = definitionRow;
                this.FocusedRow = definitionRow;
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
            foreach (var elementDef in this.ContainedRows)
            {
                elementDef.Dispose();
            }
        }

        /// <summary>
        /// Execute the <see cref="CopyModelCodeToClipboardCommand"/>
        /// </summary>
        private void ExecuteCopyModelCodeToClipboardCommand()
        {
            switch (this.SelectedThing)
            {
                case null:
                    return;
                case IModelCodeRowViewModel thing:
                    Clipboard.SetText(thing.ModelCode);
                    break;
            }
        }

        /// <summary>
        /// Update the rows to display
        /// </summary>
        private void UpdateElementDefinition()
        {
            var currentDef = this.ContainedRows.Select(x => (ElementDefinition)x.Thing).ToList();
            var addedDef = this.Thing.Element.Except(currentDef).ToList();
            var removedDef = currentDef.Except(this.Thing.Element).ToList();

            foreach (var elementDefinition in addedDef)
            {
                this.AddElementDefinitionRow(elementDefinition);
            }

            foreach (var elementDefinition in removedDef)
            {
                this.RemoveElementDefinitionRow(elementDefinition);
            }

            var topElementDefinitionOld = this.ContainedRows.FirstOrDefault(vm => ((ElementDefinitionRowViewModel)vm).IsTopElement);

            if (this.Thing.TopElement == null)
            {
                // clear the top elements
                if (topElementDefinitionOld != null)
                {
                    ((ElementDefinitionRowViewModel)topElementDefinitionOld).IsTopElement = false;
                }

                return;
            }

            if (this.ContainedRows.FirstOrDefault(vm => vm.Thing.Iid == this.Thing.TopElement.Iid) is ElementDefinitionRowViewModel topElementDefinitionNew)
            {
                topElementDefinitionNew.IsTopElement = true;

                // clear old top element.
                if ((topElementDefinitionOld != null) &&
                    (topElementDefinitionOld != topElementDefinitionNew))
                {
                    ((ElementDefinitionRowViewModel)topElementDefinitionOld).IsTopElement = false;
                }
            }
        }

        /// <summary>
        /// Executes the <see cref="ShowElementUsagesCommand"/>.
        /// </summary>
        protected virtual void ExecuteHighlightElementUsagesCommand()
        {
            // clear all highlights
            CDPMessageBus.Current.SendMessage(new CancelHighlightEvent());

            // highlight the selected thing
            CDPMessageBus.Current.SendMessage(new ElementUsageHighlightEvent((ElementDefinition)this.SelectedThing.Thing), this.SelectedThing.Thing);
        }

        /// <summary>
        /// Add the row of the associated <see cref="ElementDefinition"/>
        /// </summary>
        /// <param name="elementDef">The <see cref="ElementDefinition"/> to add</param>
        private void AddElementDefinitionRow(ElementDefinition elementDef)
        {
            this.Session.OpenIterations.TryGetValue(this.Thing, out var tuple);

            if (tuple != null)
            {
                var row = new ElementDefinitionRowViewModel(elementDef, tuple.Item1, this.Session, this);
                this.ContainedRows.SortedInsert(row, RowComparer);
            }
        }

        /// <summary>
        /// Remove the row of the associated <see cref="ElementDefinition"/>
        /// </summary>
        /// <param name="elementDef">The <see cref="ElementDefinition"/> to remove</param>
        private void RemoveElementDefinitionRow(ElementDefinition elementDef)
        {
            var row = this.ContainedRows.SingleOrDefault(x => x.Thing == elementDef);
            
            if (row != null)
            {
                this.ContainedRows.RemoveAndDispose(row);
            }
        }

        /// <summary>
        /// Computes the permissions that are only user dependent
        /// </summary>
        /// <remarks>This shall be called at initialization or when the domain changes</remarks>
        private void ComputeNotContextDependentPermission()
        {
            this.CanCreateElementDefinition = this.PermissionService.CanWrite(ClassKind.ElementDefinition, this.Thing);
            this.CanCreateParameterGroup = this.PermissionService.CanWrite(ClassKind.ParameterGroup, this.Thing);
        }

        /// <summary>
        /// Add the necessary subscriptions for this view model.
        /// </summary>
        private void AddSubscriptions()
        {
            var engineeringModelSetupSubscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.CurrentEngineeringModelSetup)
                    .Where(objectChange => (objectChange.EventKind == EventKind.Updated) && (objectChange.ChangedThing.RevisionNumber > this.RevisionNumber))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => this.UpdateProperties());
            
            this.Disposables.Add(engineeringModelSetupSubscription);

            var domainOfExpertiseSubscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(DomainOfExpertise))
                    .Where(objectChange => (objectChange.EventKind == EventKind.Updated) && (objectChange.ChangedThing.RevisionNumber > this.RevisionNumber) && (objectChange.ChangedThing.Cache == this.Session.Assembler.Cache))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => this.UpdateProperties());
            
            this.Disposables.Add(domainOfExpertiseSubscription);

            var iterationSetupSubscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.Thing.IterationSetup)
                    .Where(objectChange => (objectChange.EventKind == EventKind.Updated) && (objectChange.ChangedThing.RevisionNumber > this.RevisionNumber))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => this.UpdateProperties());
            
            this.Disposables.Add(iterationSetupSubscription);
        }

        /// <summary>
        /// Update the properties of this view-model
        /// </summary>
        private void UpdateProperties()
        {
            this.CurrentModel = this.CurrentEngineeringModelSetup.Name;
            this.CurrentIteration = this.Thing.IterationSetup.IterationNumber;

            var currentDomainOfExpertise = this.QueryCurrentDomainOfExpertise();
            this.DomainOfExpertise = currentDomainOfExpertise == null ? "None" : $"{currentDomainOfExpertise.Name} [{currentDomainOfExpertise.ShortName}]";
        }

        /// <summary>
        /// Queries the current <see cref="DomainOfExpertise"/> from the session for the current <see cref="Iteration"/>
        /// </summary>
        /// <returns>
        /// The <see cref="DomainOfExpertise"/> if selected, null otherwise.
        /// </returns>
        private DomainOfExpertise QueryCurrentDomainOfExpertise()
        {
            var iterationDomainPair = this.Session.OpenIterations.SingleOrDefault(x => x.Key == this.Thing);
            
            if (iterationDomainPair.Equals(default(KeyValuePair<Iteration, Tuple<DomainOfExpertise, Participant>>)))
            {
                return null;
            }

            return iterationDomainPair.Value?.Item1;
        }
    }
}
