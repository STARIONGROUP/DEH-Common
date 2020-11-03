// -------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionRowViewModel.cs" company="RHEA System S.A.">
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
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.Extensions;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.Utilities;

    using ReactiveUI;

    /// <summary>
    /// The row representing an <see cref="ElementDefinition"/>
    /// </summary>
    public class ElementDefinitionRowViewModel : ElementBaseRowViewModel<ElementDefinition>
    {
        /// <summary>
        /// The backing field for <see cref="IsTopElement"/>
        /// </summary>
        private bool isTopElement;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDefinitionRowViewModel"/> class
        /// </summary>
        /// <param name="elementDefinition">The associated <see cref="ElementDefinition"/></param>
        /// <param name="currentDomain">The active <see cref="DomainOfExpertise"/></param>
        /// <param name="session">The associated <see cref="ISession"/></param>
        /// <param name="containerViewModel">The container view-model</param>
        public ElementDefinitionRowViewModel(ElementDefinition elementDefinition, DomainOfExpertise currentDomain, ISession session, IViewModelBase<Thing> containerViewModel)
            : base(elementDefinition, currentDomain, session, containerViewModel)
        {
            this.UpdateProperties();
        }

        /// <summary>
        /// Gets or sets the value indicating whether this is a top element or not.
        /// </summary>
        public override bool IsTopElement
        {
            get => this.isTopElement;
            set => this.RaiseAndSetIfChanged(ref this.isTopElement, value);
        }
        
        /// <summary>
        /// Update the <see cref="ThingStatus"/> property
        /// </summary>
        protected override void UpdateThingStatus()
        {
            this.ThingStatus = new ThingStatus(this.Thing);
        }

        /// <summary>
        /// The object changed event handler
        /// </summary>
        /// <param name="objectChange">The <see cref="ObjectChangedEvent"/></param>
        protected override void ObjectChangeEventHandler(ObjectChangedEvent objectChange)
        {
            base.ObjectChangeEventHandler(objectChange);
            this.UpdateProperties();
        }
        
        /// <summary>
        /// Update this row and its children
        /// </summary>
        private void UpdateProperties()
        {
            this.Owner = this.Thing.Owner;
            this.UpdateThingStatus();
            this.PopulateParameterGroups();
            this.PopulateParameters();
            this.PopulateElemenUsages();
        }

        /// <summary>
        /// Populate the <see cref="ParameterGroupRowViewModel"/>
        /// </summary>
        private void PopulateParameterGroups()
        {
            this.PopulateParameterGroups(this.Thing);
        }

        /// <summary>
        /// Populate the <see cref="ParameterSubscriptionRowViewModel"/>
        /// </summary>
        private void UpdateParameterSubscription()
        {
            // add or remove Subscription
            var definedParameterWithSubscription = this.Thing.Parameter.Where(x => x.ParameterSubscription.Any(s => s.Owner == this.CurrentDomain)).ToList();
            var currentSubscription = this.ParameterBaseCache.Keys.OfType<ParameterSubscription>().ToList();

            var definedSubscription = definedParameterWithSubscription.Select(x => x.ParameterSubscription.Single(s => s.Owner == this.CurrentDomain)).ToList();

            // DELETED Parameter Subscription
            var deletedSubscription = currentSubscription.Except(definedSubscription).ToList();
            this.RemoveParameterBase(deletedSubscription);
            
            // ADDED Parameter Subscription
            var addedSubscription = definedSubscription.Except(currentSubscription).ToList();
            this.AddParameterBase(addedSubscription);
        }

        /// <summary>
        /// Populates the <see cref="ParameterOrOverrideBaseRowViewModel"/>
        /// </summary>
        protected override void PopulateParameters()
        {
            this.UpdateParameterSubscription();

            var definedParameterWithoutSubscription = this.Thing.Parameter.Where(x => x.ParameterSubscription.All(s => s.Owner != this.CurrentDomain)).ToList();
            var currentParameter = this.ParameterBaseCache.Keys.OfType<Parameter>().ToList();

            // DELETED Parameter
            var deletedParameters = currentParameter.Except(definedParameterWithoutSubscription).ToList();
            this.RemoveParameterBase(deletedParameters);

            // add new parameters
            var addedParameters = definedParameterWithoutSubscription.Except(currentParameter).ToList();
            this.AddParameterBase(addedParameters);
        }

        /// <summary>
        /// Populates the <see cref="ElementUsageRowViewModel"/>
        /// </summary>
        private void PopulateElemenUsages()
        {
            var currentUsages = this.ContainedRows.OfType<ElementUsageRowViewModel>().Select(x => x.Thing).ToList();

            var deletedUsages = currentUsages.Except(this.Thing.ContainedElement).ToList();
            
            foreach (var deletedUsage in deletedUsages)
            {
                var row = this.ContainedRows.OfType<ElementUsageRowViewModel>().SingleOrDefault(x => x.Thing == deletedUsage);
                
                if (row == null)
                {
                    continue;
                }

                this.ContainedRows.RemoveAndDispose(row);
            }

            var addedUsages = this.Thing.ContainedElement.Except(currentUsages).ToList();
            
            foreach (var elementUsage in addedUsages)
            {
                var row = new ElementUsageRowViewModel(elementUsage, this.CurrentDomain, this.Session, this);
                this.ContainedRows.SortedInsert(row, ChildRowComparer);
            }
        }
        
        /// <summary>
        /// Update the children rows of the current row
        /// </summary>
        public void UpdateChildren()
        {
            this.UpdateProperties();
        }
    }
}