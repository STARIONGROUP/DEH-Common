// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceListExtension.cs" company="Starion Group S.A.">
//    Copyright (c) 2020-2024 Starion Group S.A.
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
    using System.Linq;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using DynamicData;

    using ReactiveUI;
    using DynamicData;

    /// <summary>
    /// An Extension for the SourceList to order the contained rows
    /// </summary>
    public static class SourceListExtension
    {
        /// <summary>
        /// Inserts a <typeparamref name="T"/> item into the <see cref="SourceList{T}"/> in a sorted order, 
        /// using the provided <see cref="IComparer{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the <see cref="SourceList{T}"/>.</typeparam>
        /// <param name="list">The list to insert into.</param>
        /// <param name="item">The item to be inserted into the list.</param>
        /// <param name="comparer">The comparer used to determine the order of the items.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the item or the comparer is null.</exception>
        public static void SortedInsert<T>(this SourceList<T> list, T item, IComparer<T> comparer)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), $"The {nameof(item)} may not be null.");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer), $"The {nameof(comparer)} may not be null.");
            }

            var insertIndex = list.BinarySearch(item, comparer);

            if (insertIndex < 0)
            {
                list.Insert(~insertIndex, item);
            }
            else
            {
                list.Insert(insertIndex, item);
            }
        }

        /// <summary>
        /// Performs a binary search on the <see cref="SourceList{T}"/> to find the index of the item or 
        /// the index where it should be inserted if not found.
        /// </summary>
        /// <typeparam name="T">The type of the items in the <see cref="SourceList{T}"/>.</typeparam>
        /// <param name="list">The list in which the search is performed.</param>
        /// <param name="item">The item being searched for.</param>
        /// <param name="comparer">The comparer used to compare items in the list.</param>
        /// <returns>
        /// The index of the found item, or if not found, the bitwise complement of the index where the item should be inserted.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if the item or comparer is null.</exception>
        public static int BinarySearch<T>(this SourceList<T> list, T item, IComparer<T> comparer)
        {
            int startIndex = 0;
            int endIndex = list.Count - 1;

            var items = list.Items.ToArray();

            while (startIndex <= endIndex)
            {
                int middleIndex = startIndex + (endIndex - startIndex) / 2;
                var comparison = comparer.Compare(items[middleIndex], item);

                if (comparison == 0)
                {
                    return middleIndex;
                }
                else if (comparison < 0)
                {
                    startIndex = middleIndex + 1;
                }
                else
                {
                    endIndex = middleIndex - 1;
                }
            }

            return ~startIndex;
        }
    }
}
