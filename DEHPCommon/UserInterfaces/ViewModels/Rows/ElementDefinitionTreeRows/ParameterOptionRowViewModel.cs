// -------------------------------------------------------------------------------------------------
// <copyright file="ParameterOptionRowViewModel.cs" company="RHEA System S.A.">
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

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// The row representing an <see cref="Option"/> of a <see cref="ParameterBase"/>
    /// </summary>
    public class ParameterOptionRowViewModel : ParameterValueRowViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterOptionRowViewModel"/> class
        /// </summary>
        /// <param name="parameterBase">The associated <see cref="ParameterBase"/></param>
        /// <param name="option">The associated <see cref="Option"/></param>
        /// <param name="session">The associated <see cref="ISession"/></param>
        /// <param name="containerViewModel">The container row</param>
        public ParameterOptionRowViewModel(ParameterBase parameterBase, Option option, ISession session, IRowViewModelBase<Thing> containerViewModel)
            : base(parameterBase, session, option, null, containerViewModel, 0)
        {
            this.Name = this.ActualOption.Name;
            this.Option = this.ActualOption;

            var optionListener = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.Option)
                                   .Where(objectChange => objectChange.EventKind == EventKind.Updated)
                                   .ObserveOn(RxApp.MainThreadScheduler)
                                   .Subscribe(x => { this.Name = this.ActualOption.Name; });

            this.Disposables.Add(optionListener);
        }
    }
}