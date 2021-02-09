// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueRowViewModel.cs" company="RHEA System S.A.">
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
    using System.Linq;
    using System.Reactive.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Helpers;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.Utilities;

    using ReactiveUI;

    /// <summary>
    /// The base row view-model that displays the value-set of a <see cref="ParameterBase"/> 
    /// when its type is not a <see cref="ScalarParameterType"/> and it is not option and state dependent
    /// </summary>
    public abstract class ParameterValueRowViewModel : ParameterValueBaseRowViewModel<ParameterBase>
    {
        /// <summary>
        /// The Index of the <see cref="ValueArray{T}"/> in which the value is contained
        /// </summary>
        protected readonly int ValueIndex;

        /// <summary>
        /// The <see cref="Option"/> associated with this row if the <see cref="ParameterBase"/> is option-dependent
        /// </summary>
        public readonly Option ActualOption;

        /// <summary>
        /// The <see cref="ActualFiniteState"/> associated with this row if the <see cref="ParameterBase"/> is state-dependent
        /// </summary>
        public readonly ActualFiniteState ActualState;

        /// <summary>
        /// A value that indicates whether the value set's listener is initialized
        /// </summary>
        private bool isValueSetListenerInitialized;

        /// <summary>
        /// Backing field for <see cref="IsPublishable"/>
        /// </summary>
        private bool isPublishable;
        
        /// <summary>
        /// Backing field for <see cref="ModelCode"/>
        /// </summary>
        private string modelCode;

        /// <summary>
        /// Backing field for <see cref="Formula"/>
        /// </summary>
        private string formula;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterValueRowViewModel"/> class
        /// </summary>
        /// <param name="parameterBase">
        /// The associated <see cref="ParameterBase"/>
        /// </param>
        /// <param name="session">
        /// The <see cref="ISession"/>
        /// </param>
        /// <param name="actualOption">
        /// The actual <see cref="Option"/> represented if any
        /// </param>
        /// <param name="actualState">
        /// The actual <see cref="ActualFiniteState"/> represented if any
        /// </param>
        /// <param name="containerRow">
        /// The row container
        /// </param>
        /// <param name="valueIndex">
        /// The index of the component if applicable
        /// </param>
        protected ParameterValueRowViewModel(ParameterBase parameterBase, ISession session, Option actualOption, ActualFiniteState actualState, IViewModelBase<Thing> containerRow, int valueIndex = 0)
            : base(parameterBase, session, containerRow)
        {
            this.ActualOption = actualOption;
            this.ActualState = actualState;

            this.ValueIndex = valueIndex;
            this.ParameterTypeClassKind = this.Thing.ParameterType.ClassKind;
            this.Initialize();
            this.SetOwnerValue();
        }

        /// <summary>
        /// Used to call virtual member when this gets initialized
        /// </summary>
        private void Initialize()
        {
            this.UpdateThingStatus();
        }

        /// <summary>
        /// Gets the model-code
        /// </summary>
        public string ModelCode
        {
            get => this.modelCode;
            protected set => this.RaiseAndSetIfChanged(ref this.modelCode, value);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ParameterBase"/> is publishable
        /// </summary>
        public bool IsPublishable
        {
            get => this.isPublishable;
            private set => this.RaiseAndSetIfChanged(ref this.isPublishable, value);
        }

        /// <summary>
        /// Gets the <see cref="ClassKind"/> of the <see cref="ParameterType"/> represented by this <see cref="IValueSetRow"/>
        /// </summary>
        public ClassKind ParameterTypeClassKind { get; protected set; }

        /// <summary>
        /// Gets or sets the Formula column value
        /// </summary>
        public string Formula
        {
            get => this.formula;
            set => this.RaiseAndSetIfChanged(ref this.formula, value);
        }

        /// <summary>
        /// Set the Values of this row
        /// </summary>
        public virtual void SetValues()
        {
            if (this.Thing is ParameterSubscription)
            {
                this.SetParameterSubscriptionValues();
            }
            else
            {
                this.SetParameterOrOverrideValues();
                this.UpdateIsPublishableStatus();
            }

            this.ScaleShortName = this.Thing.Scale == null ? "-" : this.Thing.Scale.ShortName;
            this.UpdateThingStatus();
        }

        /// <summary>
        /// Update the <see cref="ThingStatus"/> property
        /// </summary>
        protected override void UpdateThingStatus()
        {
            this.ThingStatus = new ThingStatus(this.Thing);
        }

        /// <summary>
        /// Sets the values of this row in the case where the represented thing is a <see cref="ParameterSubscription"/>
        /// </summary>
        private void SetParameterSubscriptionValues()
        {
            if (!(this.Thing is ParameterSubscription parameterSubscription))
            {
                return;
            }

            if (this.ContainedRows.Any())
            {
                return;
            }

            var valueSet = this.GetParameterSubscriptionValueSet();

            if (valueSet == null)
            {
                this.Logger.Error("No Value set was found for the option: {0}, state: {1}", (this.ActualOption == null)? "null" : this.ActualOption.Name, (this.ActualState == null)? "null" : this.ActualState.Name);
                return;
            }

            this.AddValueSetListener(valueSet);

            this.Switch = valueSet.ValueSwitch;
            this.Computed = valueSet.Computed.Count > this.ValueIndex ? valueSet.Computed[this.ValueIndex] : "-";
            this.Manual = valueSet.Manual.Count > this.ValueIndex ? valueSet.Manual[this.ValueIndex].ToValueSetObject(this.ParameterType) : ValueSetConverter.DefaultObject(this.ParameterType);
            this.Reference = valueSet.Reference.Count > this.ValueIndex ? valueSet.Reference[this.ValueIndex].ToValueSetObject(this.ParameterType) : ValueSetConverter.DefaultObject(this.ParameterType);
            this.Value = valueSet.ActualValue.Count > this.ValueIndex ? valueSet.ActualValue[this.ValueIndex] : "-";
            this.ModelCode = valueSet.ModelCode(this.ValueIndex);
        }

        /// <summary>
        /// Sets the values of this row in the case where the represented thing is a <see cref="ParameterOrOverrideBase"/>
        /// </summary>
        private void SetParameterOrOverrideValues()
        {
            if (this.ContainedRows.Any())
            {
                return;
            }

            ParameterValueSetBase valueSet;

            if (this.Thing is Parameter)
            {
                valueSet = this.GetParameterValueSet();
            }
            else
            {
                valueSet = this.GetParameterOverrideValueSet();
            }

            if (valueSet == null)
            {
                this.Logger.Error("No Value set was found for the option: {0}, state: {1}", (this.ActualOption == null) ? "null" : this.ActualOption.Name, (this.ActualState == null) ? "null" : this.ActualState.Name);
                return;
            }

            this.AddValueSetListener(valueSet);

            if (this.Thing.ParameterType is SampledFunctionParameterType samplesFunctionParameterType)
            {
                var cols = samplesFunctionParameterType.NumberOfValues;

                this.Computed = $"[{valueSet.Computed.Count / cols}x{cols}]";
                this.Manual = $"[{valueSet.Manual.Count / cols}x{cols}]";
                this.Reference = $"[{valueSet.Reference.Count / cols}x{cols}]";
                this.Value = $"[{valueSet.ActualValue.Count / cols}x{cols}]";
                this.Formula = $"[{valueSet.Formula.Count / cols}x{cols}]";
                this.Published = $"[{valueSet.Published.Count / cols}x{cols}]";
            }
            else
            {
                this.Computed = valueSet.Computed.Count > this.ValueIndex ? valueSet.Computed[this.ValueIndex] : "-";
                this.Manual = valueSet.Manual.Count > this.ValueIndex ? valueSet.Manual[this.ValueIndex].ToValueSetObject(this.ParameterType) : ValueSetConverter.DefaultObject(this.ParameterType);
                this.Reference = valueSet.Reference.Count > this.ValueIndex ? valueSet.Reference[this.ValueIndex].ToValueSetObject(this.ParameterType) : ValueSetConverter.DefaultObject(this.ParameterType);
                this.Value = valueSet.ActualValue.Count > this.ValueIndex ? valueSet.ActualValue[this.ValueIndex] : "-";
                this.Formula = valueSet.Formula.Count > this.ValueIndex ? valueSet.Formula[this.ValueIndex] : "-";
                this.State = valueSet.ActualState != null ? valueSet.ActualState.Name : "-";
                this.Option = valueSet.ActualOption;
                this.Switch = valueSet.ValueSwitch;
                this.Published = valueSet.Published.Count > this.ValueIndex ? valueSet.Published[this.ValueIndex] : "-";
                this.ModelCode = valueSet.ModelCode(this.ValueIndex);
            }
        }

        /// <summary>
        /// Gets the <see cref="ParameterOverrideValueSet"/> of this <see cref="ParameterOverride"/> if applicable
        /// </summary>
        /// <returns>The <see cref="ParameterOverrideValueSet"/></returns>
        private ParameterOverrideValueSet GetParameterOverrideValueSet()
        {
            var parameterOverride = (ParameterOverride)this.Thing;
            ParameterOverrideValueSet valueSet = null;

            if (this.ActualOption == null && this.ActualState == null)
            {
                return parameterOverride.ValueSet.FirstOrDefault();
            }

            if (this.ActualOption != null && this.ActualState == null)
            {
                valueSet = parameterOverride.ValueSet.FirstOrDefault(v => v.ActualOption == this.ActualOption);
            }

            if (this.ActualOption == null && this.ActualState != null)
            {
                valueSet = parameterOverride.ValueSet.FirstOrDefault(v => v.ActualState == this.ActualState);
            }

            if (this.ActualOption != null && this.ActualState != null)
            {
                valueSet = parameterOverride.ValueSet.FirstOrDefault(v => v.ActualOption == this.ActualOption && v.ActualState == this.ActualState);
            }

            return valueSet;
        }

        /// <summary>
        /// Gets the <see cref="ParameterValueSet"/> of this <see cref="Parameter"/> if applicable
        /// </summary>
        /// <returns>The <see cref="ParameterValueSet"/></returns>
        private ParameterValueSet GetParameterValueSet()
        {
            var parameter = (Parameter)this.Thing;

            if (this.ActualOption == null && this.ActualState == null)
            {
                return parameter.ValueSet.FirstOrDefault();
            }

            if (this.ActualOption != null && this.ActualState == null)
            {
                return parameter.ValueSet.FirstOrDefault(v => v.ActualOption == this.ActualOption);
            }

            if (this.ActualOption == null)
            {
                return parameter.ValueSet.FirstOrDefault(v => v.ActualState == this.ActualState);
            }

            return parameter.ValueSet.FirstOrDefault(v => v.ActualOption == this.ActualOption && v.ActualState == this.ActualState);
        }

        /// <summary>
        /// Gets the <see cref="ParameterSubscriptionValueSet"/> of this <see cref="ParameterSubscription"/>
        /// </summary>
        /// <returns>The <see cref="ParameterSubscriptionValueSet"/></returns>
        private ParameterSubscriptionValueSet GetParameterSubscriptionValueSet()
        {
            var parameterSubscription = (ParameterSubscription)this.Thing;

            ParameterSubscriptionValueSet valueSet;
            if (this.ActualOption == null && this.ActualState == null)
            {
                valueSet = parameterSubscription.ValueSet.FirstOrDefault();
            }
            else if (this.ActualOption != null && this.ActualState == null)
            {
                valueSet = parameterSubscription.ValueSet.FirstOrDefault(v => v.ActualOption == this.ActualOption);
            }
            else if (this.ActualOption == null)
            {
                valueSet = parameterSubscription.ValueSet.FirstOrDefault(v => v.ActualState == this.ActualState);
            }
            else
            {
                valueSet = parameterSubscription.ValueSet.FirstOrDefault(v => v.ActualOption == this.ActualOption && v.ActualState == this.ActualState);
            }

            return valueSet;
        }

        /// <summary>
        /// Add the value set update listener for this row
        /// </summary>
        /// <param name="thing">The <see cref="ParameterValueSetBase"/> which updates need to be handled in this row</param>
        private void AddValueSetListener(ParameterValueSetBase thing)
        {
            if (this.isValueSetListenerInitialized)
            {
                return;
            }

            var listener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(thing)
                            .Where(objectChange => objectChange.EventKind == EventKind.Updated && objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(_ => this.SetValues());

            this.Disposables.Add(listener);

            this.isValueSetListenerInitialized = true;
        }

        /// <summary>
        /// Add the value set update listener for this row
        /// </summary>
        /// <param name="thing">The <see cref="ParameterSubscriptionValueSet"/> which updates need to be handled in this row</param>
        private void AddValueSetListener(ParameterSubscriptionValueSet thing)
        {
            if (this.isValueSetListenerInitialized)
            {
                return;
            }

            var listener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(thing)
                            .Where(objectChange => objectChange.EventKind == EventKind.Updated && objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(_ => this.SetValues());
            
            this.Disposables.Add(listener);

            var subscribedListener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(thing.SubscribedValueSet)
                            .Where(objectChange => objectChange.EventKind == EventKind.Updated && objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(_ => this.SetValues());
            
            this.Disposables.Add(subscribedListener);
            this.isValueSetListenerInitialized = true;
        }

        /// <summary>
        /// Update the <see cref="IsPublishable"/> value
        /// </summary>
        private void UpdateIsPublishableStatus()
        {
            if (this.ContainedRows.Any())
            {
                this.IsPublishable = false;
                return;
            }

            this.IsPublishable = this.Published != this.Value;
        }

        /// <summary>
        /// Set the value of the owner for this row
        /// </summary>
        private void SetOwnerValue()
        {
            var subscription = this.Thing as ParameterSubscription;
            IDisposable listener = null;

            if (subscription != null)
            {
                var parameterOrOverride = (ParameterOrOverrideBase) subscription.Container;
                
                if (parameterOrOverride.Owner != null)
                {
                    this.OwnerName = "[" + parameterOrOverride.Owner.Name + "]";
                    this.OwnerShortName = "[" + parameterOrOverride.Owner.ShortName + "]";
                }

                listener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(parameterOrOverride.Owner)
                    .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(x =>
                    {
                        this.OwnerName = ((DomainOfExpertise) x.ChangedThing).Name;
                        this.OwnerShortName = ((DomainOfExpertise) x.ChangedThing).ShortName;
                    });
            }
            else
            {
                if (this.Owner != null)
                {
                    this.OwnerName = this.Owner.Name;
                    this.OwnerShortName = this.Owner.ShortName;

                    listener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.Thing.Owner)
                        .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(x =>
                        {
                            this.OwnerName = ((DomainOfExpertise) x.ChangedThing).Name;
                            this.OwnerShortName = ((DomainOfExpertise) x.ChangedThing).ShortName;
                        });
                }
            }

            if (listener != null)
            {
                this.Disposables.Add(listener);
            }
        }
    }
}