// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterBaseRowViewModel.cs" company="RHEA System S.A.">
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

    using CDP4Common.CommonData;
    using CDP4Common.Comparers;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.Utilities;

    using ReactiveUI;

    /// <summary>
    /// The Base row-class for <see cref="ParameterBase"/>
    /// </summary>
    /// <typeparam name="T">A <see cref="ParameterBase"/> type</typeparam>
    public abstract class ParameterBaseRowViewModel<T> : ParameterBaseBaseRowViewModel<T>, IValueSetRow, IModelCodeRowViewModel where T : ParameterBase
    {
        /// <summary>
        /// The current <see cref="ParameterGroup"/>
        /// </summary>IsMultiSelect
        private ParameterGroup currentGroup;

        /// <summary>
        /// Backing field for <see cref="Formula"/>
        /// </summary>
        private string formula;

        /// <summary>
        /// The value-set listeners cache
        /// </summary>
        protected List<IDisposable> ValueSetListener = new List<IDisposable>();

        /// <summary>
        /// The state listeners
        /// </summary>
        private readonly List<IDisposable> actualFiniteStateListener;

        /// <summary>
        /// Backing field for <see cref="ModelCode"/>
        /// </summary>
        private string modelCode;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterBaseRowViewModel{T}"/> class. 
        /// </summary>
        /// <param name="parameterBase">
        /// The associated <see cref="ParameterBase"/>
        /// </param>
        /// <param name="session">
        /// The associated <see cref="ISession"/>
        /// </param>
        /// <param name="containerViewModel">
        /// The <see cref="ElementBaseRowViewModel{T}"/> row that contains this row.
        /// </param>
        protected ParameterBaseRowViewModel(T parameterBase, ISession session, IViewModelBase<Thing> containerViewModel) : base(parameterBase, session, containerViewModel)
        {
            this.IsCompoundType = this.Thing.ParameterType is CompoundParameterType;
            this.currentGroup = this.Thing.Group;
            this.ParameterType = this.Thing.ParameterType;
            this.ParameterTypeClassKind = this.Thing.ParameterType.ClassKind;
            this.actualFiniteStateListener = new List<IDisposable>();
            this.Initialize();
        }

        /// <summary>
        /// Used to call virtual member when this gets initialized
        /// </summary>
        private void Initialize()
        {
            this.UpdateProperties();
        }

        /// <summary>
        /// Gets or sets the owner listener
        /// </summary>
        protected KeyValuePair<DomainOfExpertise, IDisposable> OwnerListener { get; set; }

        /// <summary>
        /// Gets the model-code
        /// </summary>
        public string ModelCode
        {
            get => this.modelCode;
            protected set => this.RaiseAndSetIfChanged(ref this.modelCode, value);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ParameterType"/> of this <see cref="Parameter"/> is a <see cref="EnumerationParameterType"/>
        /// </summary>
        public virtual bool IsMultiSelect
        {
            get
            {
                var enumPt = this.ParameterType as EnumerationParameterType;
                return enumPt?.AllowMultiSelect ?? false;
            }
        }

        /// <summary>
        /// Gets the list of possible <see cref="EnumerationValueDefinition"/> for this <see cref="Parameter"/>
        /// </summary>
        public virtual ReactiveList<EnumerationValueDefinition> EnumerationValueDefinition
        {
            get
            {
                var enumValues = new ReactiveList<EnumerationValueDefinition>();

                if (this.ParameterType is EnumerationParameterType enumPt)
                {
                    enumValues.AddRange(enumPt.ValueDefinition);
                }

                return enumValues;
            }
        }

        /// <summary>
        /// Gets or sets the Formula column value
        /// </summary>
        public string Formula
        {
            get => this.formula;
            set => this.RaiseAndSetIfChanged(ref this.formula, value);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ParameterBase"/> is a <see cref="CompoundParameterType"/>
        /// </summary>
        public bool IsCompoundType { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="HasExcludes"/>. Property implemented here to fix binding errors.
        /// </summary>
        public bool? HasExcludes => null;

        /// <summary>
        /// Gets the value indicating whether the row is a top element. Property implemented here to fix binding errors.
        /// </summary>
        public bool IsTopElement => false;

        /// <summary>
        /// Gets the <see cref="ClassKind"/> of the <see cref="ParameterType"/> represented by this <see cref="IValueSetRow"/>
        /// </summary>
        public ClassKind ParameterTypeClassKind { get; protected set; }

        /// <summary>
        /// Sets the values of this row in case where the <see cref="ParameterBase"/> is neither option-dependent nor state-dependent and is a <see cref="ScalarParameterType"/>
        /// </summary>
        public abstract void SetProperties();

        /// <summary>
        /// Initializes the subscription of this row
        /// </summary>
        protected override void InitializeSubscriptions()
        {
            base.InitializeSubscriptions();

            var parameterTypeListener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.Thing.ParameterType)
                   .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .Subscribe(x => this.UpdateProperties());

            this.Disposables.Add(parameterTypeListener);
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
        /// Dispose of the listeners
        /// </summary>
        /// <param name="disposing">
        /// a value indicating whether the class is being disposed of
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.ValueSetListener.ForEach(x => x.Dispose());
            this.actualFiniteStateListener.ForEach(x => x.Dispose());
            this.OwnerListener.Value?.Dispose();
            this.actualFiniteStateListener.Clear();
            this.ValueSetListener.Clear();
        }

        /// <summary>
        /// Update the <see cref="ThingStatus"/> property
        /// </summary>
        protected override void UpdateThingStatus()
        {
            this.ThingStatus = new ThingStatus(this.Thing);
        }

        /// <summary>
        /// Update this ParameterBase row and its child nodes
        /// </summary>
        /// <remarks>
        /// if the represented <see cref="ParameterBase"/> is updated, repopulate the contained rows
        /// </remarks>
        protected override void UpdateProperties()
        {
            base.UpdateProperties();

            this.UpdateThingStatus();
            this.ModelCode = this.Thing.ModelCode();
            this.Name = this.Thing.ParameterType.Name;

            this.ClearValues();

            // clear the listener on the unique value set represented
            foreach (var listener in this.ValueSetListener)
            {
                listener.Dispose();
            }

            this.ValueSetListener.Clear();

            // clear the children and repopulate
            foreach (var row in this.ContainedRows)
            {
                row.Dispose();
            }

            this.ContainedRows.ClearAndDispose();

            if (this.Thing.IsOptionDependent)
            {
                this.SetOptionProperties();
            }
            else if (this.Thing.StateDependence != null)
            {
                this.SetStateProperties(this, null);
            }
            else if (this.IsCompoundType)
            {
                this.SetComponentProperties(this, null, null);
            }
            else
            {
                this.SetProperties();
            }

            // update the group-row under which this row shall be displayed
            if (this.currentGroup != this.Thing.Group)
            {
                this.currentGroup = this.Thing.Group;

                if (this.ContainerViewModel is IElementBaseRowViewModel elementBaseRow)
                {
                    elementBaseRow.UpdateParameterBasePosition(this.Thing);
                }
            }
        }
        
        /// <summary>
        /// Set the value of this row in case of the <see cref="ParameterType"/> is a <see cref="SampledFunctionParameterType"/>
        /// </summary>
        private void SetSampledFunctionValue()
        {
            var valueSet = (ParameterValueSetBase)this.Thing.QueryParameterBaseValueSet(this.Option, null);
            
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

        /// <summary>
        /// Sets the option dependent rows contained in this row.
        /// </summary>
        private void SetOptionProperties()
        {
            var iteration = this.Thing.GetContainerOfType<Iteration>();

            if (iteration == null)
            {
                throw new InvalidOperationException("No Iteration Container was found.");
            }

            foreach (Option availableOption in iteration.Option)
            {
                var row = new ParameterOptionRowViewModel(this.Thing, availableOption, this.Session, this);

                if (this.Thing.StateDependence != null)
                {
                    this.SetStateProperties(row, availableOption);
                }
                else if (this.IsCompoundType)
                {
                    this.SetComponentProperties(row, availableOption, null);
                }
                else
                {
                    row.SetValues();
                }

                this.ContainedRows.Add(row);
            }
        }

        /// <summary>
        /// Create or remove a row representing an <see cref="ActualFiniteState"/>
        /// </summary>
        /// <param name="row">The row container for the rows to create or remove</param>
        /// <param name="actualOption">The actual option</param>
        /// <param name="actualState">The actual state</param>
        private void UpdateActualStateRow(IRowViewModelBase<Thing> row, Option actualOption, ActualFiniteState actualState)
        {
            if (actualState.Kind == ActualFiniteStateKind.FORBIDDEN)
            {
                var rowToRemove =
                    row.ContainedRows.OfType<ParameterStateRowViewModel>()
                        .SingleOrDefault(x => x.ActualState == actualState);
                
                if (rowToRemove != null)
                {
                    row.ContainedRows.RemoveAndDispose(rowToRemove);
                }

                return;
            }

            // mandatory state
            var existingRow = row.ContainedRows.OfType<ParameterStateRowViewModel>().SingleOrDefault(x => x.ActualState == actualState);

            if (existingRow != null)
            {
                return;
            }

            var stateRow = new ParameterStateRowViewModel(this.Thing, actualOption, actualState, this.Session, row);

            if (this.Thing.ParameterType is CompoundParameterType)
            {
                this.SetComponentProperties(stateRow, actualOption, actualState);
            }
            else if (this.Thing.ParameterType is SampledFunctionParameterType)
            {
                stateRow.SetSampledFunctionValue();
            }
            else
            {
                stateRow.SetValues();
            }

            row.ContainedRows.Add(stateRow);
        }
        
        /// <summary>
        /// Initialize the listeners and process the state-dependency of this <see cref="ParameterBase"/>
        /// </summary>
        /// <param name="row">The row container</param>
        /// <param name="actualOption">The actual option</param>
        private void SetStateProperties(IRowViewModelBase<Thing> row, Option actualOption)
        {
            this.actualFiniteStateListener.ForEach(x => x.Dispose());
            this.actualFiniteStateListener.Clear();

            foreach (var actualState in this.Thing.StateDependence.ActualState)
            {
                var listener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(actualState)
                                    .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(x => this.UpdateActualStateRow(row, actualOption, actualState));

                this.actualFiniteStateListener.Add(listener);
            }

            this.StateDependence.ActualState.Sort(new ActualFiniteStateComparer());
            var actualFiniteStates = this.StateDependence.ActualState.Where(x => x.Kind == ActualFiniteStateKind.MANDATORY);

            foreach (var actualFiniteState in actualFiniteStates)
            {
                this.UpdateActualStateRow(row, actualOption, actualFiniteState);
            }
        }

        /// <summary>
        /// Creates the component rows for this <see cref="CompoundParameterType"/> <see cref="ParameterRowViewModel"/>.
        /// </summary>
        /// <param name="row">The <see cref="IRowViewModelBase{T}"/></param>
        /// <param name="actualOption">The <see cref="Option"/></param>
        /// <param name="actualState">The <see cref="ActualFiniteState"/></param>
        private void SetComponentProperties(IRowViewModelBase<Thing> row, Option actualOption, ActualFiniteState actualState)
        {         
            for (var i = 0; i < ((CompoundParameterType)this.Thing.ParameterType).Component.Count; i++)
            {
                var componentRow = new ParameterComponentValueRowViewModel(this.Thing, i, this.Session, actualOption, actualState, row);
                componentRow.SetValues();
                row.ContainedRows.Add(componentRow);
            }
        }

        /// <summary>
        /// Clear the values
        /// </summary>
        private void ClearValues()
        {
            this.Manual = null;
            this.Reference = null;
            this.Value = null;
            this.Formula = null;
            this.Computed = null;
            this.Switch = null;
            this.Scale = null;
            this.Published = null;
        }
    }
}