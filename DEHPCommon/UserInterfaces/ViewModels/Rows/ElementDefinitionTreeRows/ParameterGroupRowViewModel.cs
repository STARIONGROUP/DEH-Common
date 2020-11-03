// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupRowViewModel.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.UserInterfaces.ViewModels.Comparers;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.Utilities;

    using ReactiveUI;

    /// <summary>
    /// The row representing a <see cref="ParameterGroup"/>
    /// </summary>
    public class ParameterGroupRowViewModel : RowViewModelBase<ParameterGroup>
    {
        /// <summary>
        /// Backing field for <see cref="Name"/>
        /// </summary>
        private string name;

        /// <summary>
        /// Backing field for <see cref="ContainingGroup"/>
        /// </summary>
        private ParameterGroup containingGroup;
        
        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.RaiseAndSetIfChanged(ref this.name, value); }
        }

        /// <summary>
        /// Gets or sets the ContainingGroup
        /// </summary>
        public ParameterGroup ContainingGroup
        {
            get { return this.containingGroup; }
            set { this.RaiseAndSetIfChanged(ref this.containingGroup, value); }
        }

        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public static readonly IComparer<IRowViewModelBase<Thing>> ChildRowComparer = new ParameterGroupChildRowComparer();

        /// <summary>
        /// The active <see cref="DomainOfExpertise"/>
        /// </summary>
        protected DomainOfExpertise CurrentDomain { get; }

        /// <summary>
        /// The current <see cref="ParameterGroup"/>
        /// </summary>
        private ParameterGroup currentGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterGroupRowViewModel"/> class
        /// </summary>
        /// <param name="parameterGroup">The associated <see cref="ParameterGroup"/></param>
        /// <param name="currentDomain">The active <see cref="DomainOfExpertise"/></param>
        /// <param name="session">The associated <see cref="ISession"/></param>
        /// <param name="containerViewModel">The <see cref="IViewModelBase{T}"/> row that contains this row</param>
        public ParameterGroupRowViewModel(ParameterGroup parameterGroup, DomainOfExpertise currentDomain, ISession session, IViewModelBase<Thing> containerViewModel) : base(parameterGroup, session, containerViewModel)
        {
            this.CurrentDomain = currentDomain;
            this.currentGroup = this.Thing.ContainingGroup;
            this.UpdateProperties();
        }

        /// <summary>
        /// Gets a value indicating whether the value set editors are active
        /// </summary>
        public bool IsValueSetEditorActive => false;
        
        /// <summary>
        /// Update the properties of this row on update
        /// </summary>
        private void UpdateProperties()
        {
            this.ModifiedOn = this.Thing.ModifiedOn;
            this.Name = this.Thing.Name;
            this.ContainingGroup = this.Thing.ContainingGroup;

            this.UpdateThingStatus();

            // update the group-row under which this row shall be displayed
            if (this.currentGroup != this.Thing.ContainingGroup)
            {
                this.currentGroup = this.Thing.ContainingGroup;

                if (this.ContainerViewModel is IElementBaseRowViewModel elementBaseRow)
                {
                    elementBaseRow.UpdateParameterGroupPosition(this.Thing);
                }
            }
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
    }
}
