// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseChildRowComparer.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    /// <summary>
    /// Base <see cref="IComparer{T}"/> for <see cref="ParameterGroupChildRowComparer"/> and <see cref="ElementBaseChildRowComparer"/>
    /// </summary>
    public abstract class BaseChildRowComparer
    {
        /// <summary>
        /// The Permissible Kind of child <see cref="IRowViewModelBase{T}"/>
        /// </summary>
        private static readonly List<Type> PermissibleRowTypes = new List<Type>
        {
            typeof(ParameterOrOverrideBaseRowViewModel),
            typeof(ParameterSubscriptionRowViewModel),
            typeof(ParameterGroupRowViewModel),
            typeof(ElementUsageRowViewModel)
        };

        /// <summary>
        /// Compares two <see cref="IRowViewModelBase{Thing}"/>
        /// </summary>
        /// <param name="x">The first <see cref="IRowViewModelBase{Thing}"/> to compare</param>
        /// <param name="y">The second <see cref="IRowViewModelBase{Thing}"/> to compare</param>
        /// <returns>
        /// Less than zero : x is "lower" than y 
        /// Zero: x "equals" y. 
        /// Greater than zero: x is "greater" than y.
        /// </returns>
        public int? CommonCompare(IRowViewModelBase<Thing> x, IRowViewModelBase<Thing> y)
        {
            if (!PermissibleRowTypes.Any(type => type.IsInstanceOfType(x)) || !PermissibleRowTypes.Any(type => type.IsInstanceOfType(y)))
            {
                throw new NotSupportedException("The list contains other types of row than the specified ones.");
            }

            if (x.GetType() == y.GetType())
            {
                return this.CompareSameType(x.Thing, y);
            }

            if ((x is ParameterOrOverrideBaseRowViewModel || x is ParameterSubscriptionRowViewModel) && 
                (y is ParameterOrOverrideBaseRowViewModel || y is ParameterSubscriptionRowViewModel))
            {
                return this.CompareSameType(x.Thing, y);
            }

            if (x is ParameterOrOverrideBaseRowViewModel || x is ParameterSubscriptionRowViewModel)
            {
                return -1;
            }

            if (x is ElementUsageRowViewModel)
            {
                return 1;
            }

            return null;
        }

        /// <summary>
        /// Compares two <see cref="IRowViewModelBase{Thing}"/> of the same type
        /// </summary>
        /// <param name="xThing">The First <see cref="Thing"/></param>
        /// <param name="y">The second <see cref="IRowViewModelBase{Thing}"/></param>
        /// <returns>
        /// Less than zero : x is "lower" than y 
        /// Zero: x "equals" y. 
        /// Greater than zero: x is "greater" than y.
        /// </returns>
        protected abstract int CompareSameType(Thing xThing, IViewModelBase<Thing> y);
    }
}
