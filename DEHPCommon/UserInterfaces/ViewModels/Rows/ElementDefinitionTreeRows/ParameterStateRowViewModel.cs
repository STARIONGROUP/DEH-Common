// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterStateRowViewModel.cs" company="RHEA System S.A.">
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
    using System.Reactive.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// The row representing an <see cref="ActualFiniteState"/>
    /// </summary>
    public class ParameterStateRowViewModel : ParameterValueRowViewModel
    {
        /// <summary>
        /// Backing field for <see cref="IsDefault"/>
        /// </summary>
        private bool isDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterStateRowViewModel"/> class
        /// </summary>
        /// <param name="parameterBase">The associated <see cref="ParameterBase"/></param>
        /// <param name="option">The associated <see cref="Option"/></param>
        /// <param name="actualState">The associated <see cref="ActualFiniteState"/></param>
        /// <param name="session">The associated <see cref="ISession"/></param>
        /// <param name="containerViewModel">The container row</param>
        public ParameterStateRowViewModel(ParameterBase parameterBase, Option option, ActualFiniteState actualState, ISession session, IRowViewModelBase<Thing> containerViewModel)
            : base(parameterBase, session, option, actualState, containerViewModel, 0)
        {
            this.Name = this.ActualState.Name;
            this.State = this.ActualState.Name;
            this.IsDefault = this.ActualState.IsDefault;
            this.Option = this.ActualOption;

            foreach (var possibleFiniteState in this.ActualState.PossibleState)
            {
                var stateListener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(possibleFiniteState)
                                   .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(x => { this.Name = this.ActualState.Name; });

                this.Disposables.Add(stateListener);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ActualFiniteState"/> associated with this row is the default value of the <see cref="PossibleFiniteStateList"/>
        /// </summary>
        public bool IsDefault
        {
            get => this.isDefault;
            set => this.RaiseAndSetIfChanged(ref this.isDefault, value);
        }
        
        /// <summary>
        /// Set the value of this row in case of the <see cref="ParameterType"/> is a <see cref="SampledFunctionParameterType"/>
        /// </summary>
        public void SetSampledFunctionValue()
        {
            var valueSet = (ParameterValueSetBase)this.Thing.QueryParameterBaseValueSet(this.Option, this.ActualState);

            // perform checks to see if this is indeed a scalar value
            if (valueSet.Published.Count < 2)
            {
                this.Logger.Warn("The value set of Parameter or override {0} is marked as SampledFunction, yet has less than 2 values.", this.Thing.Iid);
            }

            this.Switch = valueSet.ValueSwitch;

            var samplesFunctionParameterType = this.Thing.ParameterType as SampledFunctionParameterType;

            if (samplesFunctionParameterType == null)
            {
                this.Logger.Warn("ParameterType mismatch, in {0} is marked as SampledFunction, yet cannot be converted.", this.Thing.Iid);
                this.Value = "-";

                return;
            }

            var cols = samplesFunctionParameterType.NumberOfValues;

            this.Value = $"[{valueSet.Published.Count / cols}x{cols}]";
        }
    }
}