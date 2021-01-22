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
    using System.Linq;
    using System.Reactive.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.Enumerators;
    using DEHPCommon.Extensions;
    using DEHPCommon.Mvvm;
    using DEHPCommon.UserInterfaces.ViewModels.Comparers;
    using DEHPCommon.UserInterfaces.ViewModels;

    using ReactiveUI;

    /// <summary>
    /// The view-model for the <see cref="PublicationBrowser"/> view
    /// </summary>
    public class PublicationsViewModel : BrowserViewModel<Iteration>
    {
        /// <summary>
        /// The <see cref="PublicationChildRowComparer"/> used to compare the sort-order of rows
        /// </summary>
        private static readonly PublicationChildRowComparer rowComparer = new PublicationChildRowComparer();

        /// <summary>
        /// Backing field for <see cref="CurrentModel"/>
        /// </summary>
        private string currentModel;

        /// <summary>
        /// Backing field for <see cref="CurrentIteration"/>
        /// </summary>
        private int currentIteration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicationsViewModel"/> class
        /// </summary>
        /// <param name="iteration">The associated <see cref="Iteration"/></param>
        /// <param name="session">The session</param>
        public PublicationsViewModel(Iteration iteration, ISession session) : base(iteration, session)
        {
            this.Publications = new DisposableReactiveList<PublicationRowViewModel>();
            this.Domains = new DisposableReactiveList<PublicationDomainOfExpertiseRowViewModel>();

            this.AddSubscriptions();
            this.UpdatePublications();
            this.UpdateDomains();
            this.UpdateProperties();
        }

        /// <summary>
        /// Gets the view model current <see cref="EngineeringModelSetup"/>
        /// </summary>
        public EngineeringModelSetup CurrentEngineeringModelSetup
        {
            get { return this.Thing.IterationSetup.GetContainerOfType<EngineeringModelSetup>(); }
        }

        /// <summary>
        /// Gets the current model caption to be displayed in the browser
        /// </summary>
        public string CurrentModel
        {
            get { return this.currentModel; }
            private set { this.RaiseAndSetIfChanged(ref this.currentModel, value); }
        }

        /// <summary>
        /// Gets the current iteration caption to be displayed in the browser
        /// </summary>
        public int CurrentIteration
        {
            get { return this.currentIteration; }
            private set { this.RaiseAndSetIfChanged(ref this.currentIteration, value); }
        }

        /// <summary>
        /// Gets the rows representing <see cref="Publication"/>s
        /// </summary>
        public DisposableReactiveList<PublicationRowViewModel> Publications { get; private set; }

        /// <summary>
        /// Gets the rows representing <see cref="DomainOfExpertise"/>s
        /// </summary>
        public DisposableReactiveList<PublicationDomainOfExpertiseRowViewModel> Domains { get; private set; }

        /// <summary>
        /// Gets all parameter rows.
        /// </summary>
        public IEnumerable<PublicationParameterOrOverrideRowViewModel> Parameters
        {
            get
            {
                return this.Domains.SelectMany(d => d.ContainedRows).OfType<PublicationParameterOrOverrideRowViewModel>();
            }
        }

        /// <summary>
        /// Add the necessary subscriptions for this view model.
        /// </summary>
        private void AddSubscriptions()
        {
            var updatePublishableParameterListener =
                CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(ParameterValueSetBase))
                    .Where(objectChange => objectChange.EventKind == EventKind.Updated
                                        && objectChange.ChangedThing.CacheKey.Iteration == this.Thing.Iid
                                        && objectChange.ChangedThing.Cache == this.Session.Assembler.Cache)
                    .Select(x => x.ChangedThing.Container as ParameterOrOverrideBase)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(this.UpdatePublishableParameter);
            this.Disposables.Add(updatePublishableParameterListener);

            var addParameterListener =
                CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(ParameterOrOverrideBase))
                    .Where(objectChange => objectChange.EventKind == EventKind.Added
                                           && objectChange.ChangedThing.CacheKey.Iteration == this.Thing.Iid
                                           && objectChange.ChangedThing.Cache == this.Session.Assembler.Cache)
                    .Select(x => x.ChangedThing as ParameterOrOverrideBase)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(this.UpdatePublishableParameter);
            this.Disposables.Add(addParameterListener);

            var updateParameterListener =
                 CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(ParameterOrOverrideBase))
                 .Where(objectChange => objectChange.EventKind == EventKind.Updated
                                    && objectChange.ChangedThing.CacheKey.Iteration == this.Thing.Iid
                                    && objectChange.ChangedThing.Cache == this.Session.Assembler.Cache)
                 .Select(x => x.ChangedThing as ParameterOrOverrideBase)
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .Subscribe(this.RelocateParameterRowViewModel);
            this.Disposables.Add(updateParameterListener);

            var removeParameterListener =
                 CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(ParameterOrOverrideBase))
                 .Where(objectChange => objectChange.EventKind == EventKind.Removed
                                    && objectChange.ChangedThing.CacheKey.Iteration == this.Thing.Iid
                                    && objectChange.ChangedThing.Cache == this.Session.Assembler.Cache)
                 .Select(x => x.ChangedThing as ParameterOrOverrideBase)
                 .ObserveOn(RxApp.MainThreadScheduler)
                 .Subscribe(this.RemovePublishableParameterRowViewModel);
            this.Disposables.Add(removeParameterListener);

            var addPublicationListener =
                CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(Publication))
                    .Where(objectChange => objectChange.EventKind == EventKind.Added
                                        && objectChange.ChangedThing.CacheKey.Iteration == this.Thing.Iid
                                        && objectChange.ChangedThing.Cache == this.Session.Assembler.Cache)
                    .Select(x => x.ChangedThing as Publication)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(this.AddPublicationRowViewModel);
            this.Disposables.Add(addPublicationListener);

            var updatedEngineeringModelListener =
                CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.CurrentEngineeringModelSetup)
                    .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(x => this.UpdateDomains());
            this.Disposables.Add(updatedEngineeringModelListener);

            var updateDomainOfExpretiseListener =
                CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(DomainOfExpertise))
                    .Where(objectChange => objectChange.EventKind == EventKind.Updated && objectChange.ChangedThing.Cache == this.Session.Assembler.Cache)
                    .Select(x => x.ChangedThing as DomainOfExpertise)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(this.UpdateDomainOfExpretiseRowViewModel);
            this.Disposables.Add(updateDomainOfExpretiseListener);

            var engineeringModelSetupSubscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.CurrentEngineeringModelSetup)
                    .Where(objectChange => objectChange.EventKind == EventKind.Updated
                                        && objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => this.UpdateProperties());
            this.Disposables.Add(engineeringModelSetupSubscription);

            var domainOfExpertiseSubscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(typeof(DomainOfExpertise))
                    .Where(objectChange => objectChange.EventKind == EventKind.Updated
                                        && objectChange.ChangedThing.RevisionNumber > this.RevisionNumber
                                        && objectChange.ChangedThing.Cache == this.Session.Assembler.Cache)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => this.UpdateProperties());
            this.Disposables.Add(domainOfExpertiseSubscription);

            var iterationSetupSubscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.Thing.IterationSetup)
                    .Where(objectChange => objectChange.EventKind == EventKind.Updated
                                        && objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => this.UpdateProperties());
            this.Disposables.Add(iterationSetupSubscription);
        }

        /// <summary>
        /// Update the rows representing the publishable <see cref="ParameterOrOverrideBase"/>
        /// </summary>
        /// <param name="parameter">The updated <see cref="ParameterOrOverrideBase"/></param>
        private void UpdatePublishableParameter(ParameterOrOverrideBase parameter)
        {
            if (parameter.CanBePublished)
            {
                this.AddPublishableParameterRowViewModel(parameter);
            }
            else
            {
                this.RemovePublishableParameterRowViewModel(parameter);
            }
        }

        /// <summary>
        /// Adds the publishable <see cref="PublicationParameterOrOverrideRowViewModel"/> row view model to the tree.
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterOrOverrideBase"/> that this row will belong to.</param>
        private void AddPublishableParameterRowViewModel(ParameterOrOverrideBase parameter)
        {
            var domainRow = this.Domains.SingleOrDefault(vm => vm.Thing == parameter.Owner);
            if (domainRow == null)
            {
                return;
            }

            // if the row already exists then simply update it
            var parameterRow = domainRow.ContainedRows.FirstOrDefault(pr => pr.Thing == parameter);
            if (parameterRow != null)
            {
                ((PublicationParameterOrOverrideRowViewModel)parameterRow).SetProperties();
                return;
            }

            // create the row only if it is publishable
            parameterRow = new PublicationParameterOrOverrideRowViewModel(parameter, this.Session, domainRow);

            // if the domain row has a checkbox, select this one as well.
            ((PublicationParameterOrOverrideRowViewModel)parameterRow).ToBePublished = domainRow.ToBePublished;

            domainRow.ContainedRows.SortedInsert(parameterRow, rowComparer);
        }

        /// <summary>
        /// Removes a publishable parameter row view-model if the <see cref="ParameterOrOverrideBase"/> is no longer publishable.
        /// </summary>
        /// <param name="parameter">The parameter that is no longer publishable.</param>
        private void RemovePublishableParameterRowViewModel(ParameterOrOverrideBase parameter)
        {
            var domainRow = this.Domains.SingleOrDefault(vm => vm.Thing == parameter.Owner);
            if (domainRow == null)
            {
                return;
            }

            var parameterRow = domainRow.ContainedRows.FirstOrDefault(pr => pr.Thing == parameter);
            if (parameterRow != null)
            {
                domainRow.ContainedRows.RemoveAndDispose(parameterRow);
            }
        }

        /// <summary>
        /// Updates the <see cref="Domains"/> of the parameter in case the <see cref="ParameterOrOverrideBase.Owner"/> that contains Parameter changes.
        /// </summary>
        /// <param name="parameterOrOverrideBase">The <see cref="ParameterOrOverrideBase"/> to relocate or remove</param>
        private void RelocateParameterRowViewModel(ParameterOrOverrideBase parameterOrOverrideBase)
        {
            // In case the owner has changed check if there are other domains that contain that parameter.
            var oldOwners = this.Domains.Where(d => d.ContainedRows.Any(p => p.Thing == parameterOrOverrideBase) && d.Thing != parameterOrOverrideBase.Owner);
            foreach (var owner in oldOwners)
            {
                var row = owner.ContainedRows.FirstOrDefault(pr => pr.Thing == parameterOrOverrideBase);
                if (row != null)
                {
                    owner.ContainedRows.RemoveAndDispose(row);
                }
            }

            if (!parameterOrOverrideBase.CanBePublished)
            {
                return;
            }

            var domainRow = this.Domains.SingleOrDefault(vm => vm.Thing == parameterOrOverrideBase.Owner);
            if (domainRow == null)
            {
                return;
            }

            var parameterRow = domainRow.ContainedRows.FirstOrDefault(pr => pr.Thing == parameterOrOverrideBase);
            if (parameterRow == null)
            {
                parameterRow = new PublicationParameterOrOverrideRowViewModel(parameterOrOverrideBase, this.Session, domainRow);

                // if the domain row has a checkbox, select this one as well.
                ((PublicationParameterOrOverrideRowViewModel)parameterRow).ToBePublished = domainRow.ToBePublished;

                domainRow.ContainedRows.SortedInsert(parameterRow, rowComparer);
            }
        }

        /// <summary>
        /// Adds a <see cref="PublicationRowViewModel"/>
        /// </summary>
        /// <param name="publication">The associated <see cref="Publication"/></param>
        private void AddPublicationRowViewModel(Publication publication)
        {
            if (this.Publications.Any(x => x.Thing == publication))
            {
                return;
            }

            var row = new PublicationRowViewModel(publication, this.Session, this);
            row.Index = this.Thing.Publication.IndexOf(publication);

            var listOfParams = new List<PublicationParameterOrOverrideRowViewModel>();

            foreach (var parameterOrOverrideBase in publication.PublishedParameter)
            {
                var parameterRow = new PublicationParameterOrOverrideRowViewModel(parameterOrOverrideBase, this.Session, row);

                // turn off the checkbox for this row
                parameterRow.IsCheckable = false;
                listOfParams.Add(parameterRow);
            }

            this.Publications.Add(row);
            row.ContainedRows.AddRange(listOfParams.OrderBy(r => r.Thing.ParameterType.Name));
        }

        /// <summary>
        /// The <see cref="ObjectChangedEvent"/> handler
        /// </summary>
        /// <param name="objectChange">The <see cref="ObjectChangedEvent"/></param>
        protected override void ObjectChangeEventHandler(ObjectChangedEvent objectChange)
        {
            base.ObjectChangeEventHandler(objectChange);
            this.UpdatePublications();
            this.UpdateDomains();
            this.UpdateProperties();
        }

        /// <summary>
        /// Update the properties of this view-model
        /// </summary>
        private void UpdateProperties()
        {
            this.CurrentModel = this.CurrentEngineeringModelSetup.Name;
            this.CurrentIteration = this.Thing.IterationSetup.IterationNumber;

            var currentDomainOfExpertise = this.Session.QuerySelectedDomainOfExpertise(this.Thing);
            this.DomainOfExpertise = currentDomainOfExpertise == null ? "None" : $"{currentDomainOfExpertise.Name} [{currentDomainOfExpertise.ShortName}]";
        }

        /// <summary>
        /// Update the <see cref="Publications"/> List
        /// </summary>
        private void UpdatePublications()
        {
            var newPublications = this.Thing.Publication.Except(this.Publications.Select(x => x.Thing)).ToList();
            var oldPublications = this.Publications.Select(x => x.Thing).Except(this.Thing.Publication).ToList();

            foreach (var publication in oldPublications)
            {
                var row = this.Publications.SingleOrDefault(x => x.Thing == publication);
                if (row != null)
                {
                    this.Publications.RemoveAndDispose(row);
                }
            }

            foreach (var publication in newPublications)
            {
                this.AddPublicationRowViewModel(publication);
            }

            this.Publications.Sort((o1, o2) => o1.Index.CompareTo(o2.Index));
        }

        /// <summary>
        /// Update the <see cref="Domains"/> List
        /// </summary>
        private void UpdateDomains()
        {
            var newDomains = this.CurrentEngineeringModelSetup.ActiveDomain.Except(this.Domains.Select(x => x.Thing)).ToList();
            var oldDomains = this.Domains.Select(x => x.Thing).Except(this.CurrentEngineeringModelSetup.ActiveDomain).ToList();

            foreach (var domain in oldDomains)
            {
                var row = this.Domains.SingleOrDefault(x => x.Thing == domain);
                if (row != null)
                {
                    this.Domains.RemoveAndDispose(row);
                }
            }

            foreach (var domain in newDomains)
            {
                this.AddDomainOfExpretiseRowViewModel(domain);
            }

            this.SortDomains();
        }


        /// <summary>
        /// Sort the <see cref="Domains"/> List
        /// </summary>
        private void SortDomains()
        {
            this.Domains.Sort((o1, o2) =>
            {
                var nameO1 = o1.Name ?? string.Empty;
                var nameO2 = o2.Name ?? string.Empty;

                if (string.IsNullOrEmpty(nameO1))
                {
                    return -1;
                }

                return string.IsNullOrEmpty(nameO2) ? 1 : string.CompareOrdinal(nameO1, nameO2);
            });
        }

        /// <summary>
        /// Add a domain to the <see cref="Domains"/> List
        /// </summary>
        /// <param name="domain">Domain to be added</param>
        private void AddDomainOfExpretiseRowViewModel(DomainOfExpertise domain)
        {
            var row = new PublicationDomainOfExpertiseRowViewModel(domain, this.Session, this);

            var listOfParams = new List<PublicationParameterOrOverrideRowViewModel>();

            foreach (var parameter in domain.OwnedParametersThatCanBePublished(this.Thing))
            {
                var parameterRow = new PublicationParameterOrOverrideRowViewModel(parameter, this.Session, row);
                listOfParams.Add(parameterRow);
            }

            this.Domains.Add(row);
            row.ContainedRows.AddRange(listOfParams.OrderBy(r => r.Name));
        }

        /// <summary>
        /// Remove a domain to the <see cref="Domains"/> List
        /// </summary>
        /// <param name="domain">Domain to be removed</param>
        private void RemoveDomainOfExpretiseRowViewModel(DomainOfExpertise domain)
        {
            var domainRow = this.Domains.SingleOrDefault(vm => vm.Thing == domain);

            if (domainRow != null)
            {
                this.Domains.RemoveAndDispose(domainRow);
            }
        }

        /// <summary>
        /// Adds or removes a domain to the <see cref="Domains"/> list depending if it is or not deprecated
        /// </summary>
        /// <param name="domain">Domain to be updated</param>
        private void UpdateDomainOfExpretiseRowViewModel(DomainOfExpertise domain)
        {
            if (domain.IsDeprecated)
            {
                this.RemoveDomainOfExpretiseRowViewModel(domain);
            }
            else if (this.Domains.SingleOrDefault(vm => vm.Thing == domain) == null)
            {
                this.AddDomainOfExpretiseRowViewModel(domain);
                this.SortDomains();
            }
        }

        /// <summary>
        /// Populates the <see cref="PublicationsViewModel.ContextMenu"/>
        /// </summary>
        public override void PopulateContextMenu()
        {
            base.PopulateContextMenu();
            this.ContextMenu.Clear();

            if (this.SelectedThing != null && this.SelectedThing.ContainedRows.Count > 0)
            {
                this.ContextMenu.Add(
                    this.SelectedThing.IsExpanded ?
                    new ContextMenuItemViewModel("Collapse Rows", "", this.CollpaseRowsCommand, MenuItemKind.None, ClassKind.NotThing) :
                    new ContextMenuItemViewModel("Expand Rows", "", this.ExpandRowsCommand, MenuItemKind.None, ClassKind.NotThing));
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

            foreach (var publication in this.Publications)
            {
                publication.Dispose();
            }

            foreach (var domain in this.Domains)
            {
                domain.Dispose();
            }
        }
    }
}