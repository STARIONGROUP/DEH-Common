// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterComponentValueRowViewModel.cs" company="RHEA System S.A.">
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
    /// The Row representing a value that corresponds to a <see cref="ParameterTypeComponent"/> of a <see cref="ParameterBase"/>
    /// </summary>
    public class ParameterComponentValueRowViewModel : ParameterValueRowViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterComponentValueRowViewModel"/> class
        /// </summary>
        /// <param name="parameterBase">
        /// The associated <see cref="ParameterBase"/>
        /// </param>
        /// <param name="valueIndex">
        /// The index of this component in the <see cref="CompoundParameterType"/>
        /// </param>
        /// <param name="session">
        /// The associated <see cref="ISession"/>
        /// </param>
        /// <param name="actualOption">
        /// The <see cref="Option"/> of this row if any
        /// </param>
        /// <param name="actualState">
        /// The <see cref="ActualFiniteState"/> of this row if any
        /// </param>
        /// <param name="containerRow">
        /// the row container
        /// </param>
        /// <param name="isReadOnly">
        /// A value indicating whether the row is read-only
        /// </param>
        public ParameterComponentValueRowViewModel(ParameterBase parameterBase, int valueIndex, ISession session, Option actualOption, ActualFiniteState actualState, IViewModelBase<Thing> containerRow, bool isReadOnly)
            : base(parameterBase, session, actualOption, actualState, containerRow, valueIndex, isReadOnly)
        {
            if (!(this.Thing.ParameterType is CompoundParameterType compoundParameterType))
            {
                throw new InvalidOperationException("This row shall only be used for CompoundParameterType.");
            }

            if (valueIndex >= compoundParameterType.Component.Count)
            {
                throw new IndexOutOfRangeException($"The compoundParameterType {compoundParameterType.Name} has only {compoundParameterType.Component.Count} components");
            }

            if (containerRow == null)
            {
                throw new ArgumentNullException(nameof(containerRow), "The container row is mandatory");
            }

            // reset the classkind of this row to match the component classkind
            var component = compoundParameterType.Component[valueIndex];
            this.Name = component.ShortName;

            var subscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(component)
                        .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(x => this.Name = component.ShortName);

            this.Disposables.Add(subscription);

            this.WhenAnyValue(x => x.Switch).Skip(1).Subscribe(x =>
            {
                if (this.ContainerViewModel is ParameterValueRowViewModel valueBaseRow)
                {
                    foreach (var rowViewModelBase in valueBaseRow.ContainedRows)
                    {
                        var row = (ParameterComponentValueRowViewModel) rowViewModelBase;
                        row.Switch = x;
                    }

                    return;
                }

                if (this.ContainerViewModel is ParameterOrOverrideBaseRowViewModel parameterBaseRow)
                {
                    foreach (var rowViewModelBase in parameterBaseRow.ContainedRows)
                    {
                        var row = (ParameterComponentValueRowViewModel) rowViewModelBase;
                        row.Switch = x;
                    }

                    return;
                }

                if (this.ContainerViewModel is ParameterSubscriptionRowViewModel subscriptionRow)
                {
                    foreach (var rowViewModelBase in subscriptionRow.ContainedRows)
                    {
                        var row = (ParameterComponentValueRowViewModel) rowViewModelBase;
                        row.Switch = x;
                    }
                }
            });
        }

        /// <summary>
        /// The row type for this <see cref="ParameterComponentValueRowViewModel"/>
        /// </summary>
        public override string RowType => "Parameter Type Component";

        /// <summary>
        /// Gets a value indicating whether the values are read only
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether the reference values are read only
        /// </summary>
        public bool IsReferenceReadOnly => false;

        /// <summary>
        /// Setting values for this <see cref="ParameterComponentValueRowViewModel"/>
        /// </summary>
        public override void SetValues()
        {
            base.SetValues();

            if (!(this.Thing.ParameterType is CompoundParameterType compoundParameterType))
            {
                throw new InvalidOperationException("This row shall only be used for CompoundParameterType.");
            }

            this.Scale = compoundParameterType.Component[this.ValueIndex].Scale;
            this.ScaleShortName = this.Scale == null ? "-" : this.Scale.ShortName;
        }
    }
}
