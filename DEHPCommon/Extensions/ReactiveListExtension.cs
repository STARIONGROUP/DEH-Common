// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReactiveListExtension.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Extensions
{
    using System;
    using System.Collections.Generic;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// An Extension for the ReactiveList to order the contained rows
    /// </summary>
    public static class ReactiveListExtension
    {
        /// <summary>
        /// Insert a <see cref="IRowViewModelBase{T}"/> into the list given a <see cref="IComparer{T}"/>
        /// </summary>
        /// <param name="list">The <see cref="ReactiveList{T}"/></param>
        /// <param name="row">The <see cref="IRowViewModelBase{Thing}"/> to add</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> used to perform the sorting</param>
        public static void SortedInsert<T>(this ReactiveList<T> list, T row, IComparer<T> comparer)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row), $"The {nameof(row)} may not be null");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer), $"The {nameof(comparer)} may not be null");
            }

            // item is found using the comparer : returns the index of the item found
            // item not found : returns a negative number that is the bitwise complement 
            // of the index of the next element that is larger or count if none
            var index = list.BinarySearch(row, comparer);

            if (index < 0)
            {
                list.Insert(~index, row);
            }
            else
            {
                list.Insert(index, row);
            }
        }
    }
}
