// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterBaseBaseRowViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// Base row view Model for <see cref="ParameterBaseRowViewModel{T}"/> and <see cref="ParameterValueBaseRowViewModel{T}"/>
    /// </summary>
    public class ParameterBaseBaseRowViewModel<T> : RowViewModelBase<T> where T : ParameterBase
    {
        /// <summary>
        /// Backing field for <see cref="IsOptionDependent"/>
        /// </summary>
        private bool isOptionDependent;

        /// <summary>
        /// Backing field for <see cref="ParameterType"/>
        /// </summary>
        private ParameterType parameterType;

        /// <summary>
        /// Backing field for <see cref="ParameterTypeShortName"/>
        /// </summary>
        private string parameterTypeShortName;

        /// <summary>
        /// Backing field for <see cref="ParameterTypeName"/>
        /// </summary>
        private string parameterTypeName;

        /// <summary>
        /// Backing field for <see cref="Scale"/>
        /// </summary>
        private MeasurementScale scale;

        /// <summary>
        /// Backing field for <see cref="ScaleShortName"/>
        /// </summary>
        private string scaleShortName;

        /// <summary>
        /// Backing field for <see cref="ScaleName"/>
        /// </summary>
        private string scaleName;

        /// <summary>
        /// Backing field for <see cref="StateDependence"/>
        /// </summary>
        private ActualFiniteStateList stateDependence;

        /// <summary>
        /// Backing field for <see cref="Group"/>
        /// </summary>
        private ParameterGroup group;

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
        /// Initializes a new instance of the <see cref="ParameterBaseRowViewModel{T}"/> class
        /// </summary>
        /// <param name="parameterBase">The <see cref="ParameterBase"/> associated with this row</param>
        /// <param name="session">The session</param>
        /// <param name="containerViewModel">The <see cref="IViewModelBase{T}"/> that is the container of this <see cref="IRowViewModelBase{T}"/></param>
        protected ParameterBaseBaseRowViewModel(T parameterBase, ISession session, IViewModelBase<Thing> containerViewModel) : base(parameterBase, session, containerViewModel)
        {
        }

        /// <summary>
        /// Gets or sets the IsOptionDependent
        /// </summary>
        public bool IsOptionDependent
        {
            get => this.isOptionDependent;
            set => this.RaiseAndSetIfChanged(ref this.isOptionDependent, value);
        }

        /// <summary>
        /// Gets or sets the ParameterType
        /// </summary>
        public ParameterType ParameterType
        {
            get => this.parameterType;
            set => this.RaiseAndSetIfChanged(ref this.parameterType, value);
        }

        /// <summary>
        /// Gets or set the ShortName of <see cref="ParameterType"/>
        /// </summary>
        public string ParameterTypeShortName
        {
            get => this.parameterTypeShortName;
            set => this.RaiseAndSetIfChanged(ref this.parameterTypeShortName, value);
        }

        /// <summary>
        /// Gets or set the Name of <see cref="ParameterType"/>
        /// </summary>
        public string ParameterTypeName
        {
            get => this.parameterTypeName;
            set => this.RaiseAndSetIfChanged(ref this.parameterTypeName, value);
        }

        /// <summary>
        /// Gets or sets the Scale
        /// </summary>
        public MeasurementScale Scale
        {
            get => this.scale;
            set => this.RaiseAndSetIfChanged(ref this.scale, value);
        }

        /// <summary>
        /// Gets or set the ShortName of <see cref="Scale"/>
        /// </summary>
        public string ScaleShortName
        {
            get => this.scaleShortName;
            set => this.RaiseAndSetIfChanged(ref this.scaleShortName, value);
        }

        /// <summary>
        /// Gets or set the Name of <see cref="Scale"/>
        /// </summary>
        public string ScaleName
        {
            get => this.scaleName;
            set => this.RaiseAndSetIfChanged(ref this.scaleName, value);
        }

        /// <summary>
        /// Gets or sets the StateDependence
        /// </summary>
        public ActualFiniteStateList StateDependence
        {
            get => this.stateDependence;
            set => this.RaiseAndSetIfChanged(ref this.stateDependence, value);
        }

        /// <summary>
        /// Gets or sets the Group
        /// </summary>
        public ParameterGroup Group
        {
            get => this.group;
            set => this.RaiseAndSetIfChanged(ref this.group, value);
        }

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
        /// The <see cref="Option"/> being used
        /// </summary>
        private Option option;

        /// <summary>
        /// The backing field for the <see cref="Value"/> property.
        /// </summary>
        private string value;

        /// <summary>
        /// The backing field for the <see cref="Published"/> property.
        /// </summary>
        private string published;

        /// <summary>
        /// The backing field for the <see cref="State"/> property.
        /// </summary>
        private string state;
        
        /// <summary>
        /// The backing field for the <see cref="Computed"/> property.
        /// </summary>
        private string computed;

        /// <summary>
        /// The backing field for the <see cref="Manual"/> property.
        /// </summary>
        private object manual;

        /// <summary>
        /// The backing field for the <see cref="Reference"/> property.
        /// </summary>
        private object reference;

        /// <summary>
        /// The backing field for the <see cref="Switch"/> property.
        /// </summary>
        private ParameterSwitchKind? switchValue;

        /// <summary>
        /// The backing field for the <see cref="Name"/> property
        /// </summary>
        private string name;

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        /// <summary>
        /// Gets the ShortName
        /// </summary>
        public string ShortName => this.Name;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value
        {
            get => this.value;
            set => this.RaiseAndSetIfChanged(ref this.value, value);
        }

        /// <summary>
        /// Gets or sets the published value.
        /// </summary>
        public string Published
        {
            get => this.published;
            set => this.RaiseAndSetIfChanged(ref this.published, value);
        }

        /// <summary>
        /// Gets or sets the switch.
        /// </summary>
        public ParameterSwitchKind? Switch
        {
            get => this.switchValue;
            set => this.RaiseAndSetIfChanged(ref this.switchValue, value);
        }

        /// <summary>
        /// Gets or sets the computed value.
        /// </summary>
        public string Computed
        {
            get => this.computed;
            set => this.RaiseAndSetIfChanged(ref this.computed, value);
        }

        /// <summary>
        /// Gets or sets the manual value.
        /// </summary>
        public object Manual
        {
            get => this.manual;
            set => this.RaiseAndSetIfChanged(ref this.manual, value);
        }

        /// <summary>
        /// Gets or sets the reference value.
        /// </summary>
        public object Reference
        {
            get => this.reference;
            set => this.RaiseAndSetIfChanged(ref this.reference, value);
        }

        /// <summary>
        /// Gets or sets the reference value.
        /// </summary>
        public Option Option
        {
            get => this.option;
            protected set => this.RaiseAndSetIfChanged(ref this.option, value);
        }

        /// <summary>
        /// Gets or sets the reference value.
        /// </summary>
        public string State
        {
            get => this.state;
            protected set => this.RaiseAndSetIfChanged(ref this.state, value);
        }

        /// <summary>
        /// Updates the properties of this row
        /// </summary>
        protected  virtual void UpdateProperties()
        {
            this.ModifiedOn = this.Thing.ModifiedOn;

            this.IsOptionDependent = this.Thing.IsOptionDependent;

            if (this.Thing.ParameterType != null)
            {
                this.ParameterTypeShortName = this.Thing.ParameterType.ShortName;
                this.ParameterTypeName = this.Thing.ParameterType.Name;
            }

            this.ParameterType = this.Thing.ParameterType;

            if (this.Thing.Scale != null)
            {
                this.ScaleShortName = this.Thing.Scale.ShortName;
                this.ScaleName = this.Thing.Scale.Name;
            }

            this.Scale = this.Thing.Scale;

            this.StateDependence = this.Thing.StateDependence;

            this.Group = this.Thing.Group;

            if (this.Thing.Owner != null)
            {
                this.OwnerShortName = this.Thing.Owner.ShortName;
                this.OwnerName = this.Thing.Owner.Name;
            }
        }
    }
}
