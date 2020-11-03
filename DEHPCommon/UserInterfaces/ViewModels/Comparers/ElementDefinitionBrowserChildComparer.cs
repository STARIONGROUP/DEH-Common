// ------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionBrowserChildComparer.cs" company="RHEA System S.A.">
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

    using CDP4Common.CommonData;
    using CDP4Common.Comparers;
    using CDP4Common.EngineeringModelData;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    /// <summary>
    /// The <see cref="IComparer{T}"/> for the child-rows of the <see cref="ElementDefinitionBrowserViewModel"/>
    /// </summary>
    public class ElementDefinitionBrowserChildComparer : IComparer<IRowViewModelBase<Thing>>
    {
        /// <summary>
        /// The comparer
        /// </summary>
        private static readonly ElementDefinitionComparer comparer = new ElementDefinitionComparer();

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
        public int Compare(IRowViewModelBase<Thing> x, IRowViewModelBase<Thing> y)
        {
            if (!(x.Thing is ElementDefinition) || !(y.Thing is ElementDefinition))
            {
                throw new InvalidOperationException("one or both of the parameters is not an Element Definition row.");
            }

            return comparer.Compare((ElementDefinition) x.Thing, (ElementDefinition) y.Thing);
        }
    }
}