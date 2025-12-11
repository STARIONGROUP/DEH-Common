// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceListExtensions.cs" company="Starion Group S.A.">
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

namespace DEHPCommon.Mvvm
{
    using DynamicData;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for <see cref="SourceList{T}"/> to manage the disposal of items when they are removed.
    /// These methods enforce a choice between disposing of items or not when they are removed from the list.
    /// </summary>
    public static class SourceListExtensions
    {
        /// <summary>
        /// Clears all items from the <paramref name="list"/> and disposes of each item of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to clear.</param>
        public static void ClearAndDispose<T>(this SourceList<T> list) where T : IDisposable
        {
            list.Clear(true);
        }

        /// <summary>
        /// Clears all items from the <paramref name="list"/> without disposing of them.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to clear.</param>
        public static void ClearWithoutDispose<T>(this SourceList<T> list) where T : IDisposable
        {
            list.Clear(false);
        }

        /// <summary>
        /// Removes all items from the <paramref name="list"/> and optionally disposes of them.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to clear.</param>
        /// <param name="dispose">Indicates whether to dispose of the removed items.</param>
        public static void Clear<T>(this SourceList<T> list, bool dispose) where T : IDisposable
        {
            if (dispose)
            {
                foreach (var item in list.Items.ToArray())
                {
                    TryDispose(item);
                }
            }

            list.RemoveRange(0, list.Count);
        }

        /// <summary>
        /// Removes all specified items from the <paramref name="list"/> and disposes of them.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="items">The items to remove.</param>
        public static void RemoveAllAndDispose<T>(this SourceList<T> list, IEnumerable<T> items) where T : IDisposable
        {
            list.RemoveAll(items, true);
        }

        /// <summary>
        /// Removes all specified items from the <paramref name="list"/> without disposing of them.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="items">The items to remove.</param>
        public static void RemoveAllWithoutDispose<T>(this SourceList<T> list, IEnumerable<T> items) where T : IDisposable
        {
            list.RemoveAll(items, false);
        }

        /// <summary>
        /// Removes specified items from the <paramref name="list"/> and optionally disposes of them.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="items">The items to remove.</param>
        /// <param name="dispose">Indicates whether to dispose of the removed items.</param>
        private static void RemoveAll<T>(this SourceList<T> list, IEnumerable<T> items, bool dispose) where T : IDisposable
        {
            var disposables = items as T[] ?? items.ToArray();

            if (dispose)
            {
                foreach (var item in disposables)
                {
                    TryDispose(item);
                }
            }

            foreach (var item in disposables)
            {
                list.Remove(item);
            }
        }

        /// <summary>
        /// Removes the item and disposes of it.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="item">The zero-based index of the item to remove.</param>
        public static void RemoveAndDispose<T>(this SourceList<T> list, T item) where T : IDisposable
        {
            list.Remove(item, true);
        }

        /// <summary>
        /// Removes the item without disposing of it.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="item">The zero-based index of the item to remove.</param>
        public static void RemoveWithoutDispose<T>(this SourceList<T> list, T item) where T : IDisposable
        {
            list.Remove(item, false);
        }

        /// <summary>
        /// Removes the item and optionally disposes of it.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="item">The zero-based index of the item to remove.</param>
        /// <param name="dispose">Indicates whether to dispose of the removed item.</param>
        private static void Remove<T>(this SourceList<T> list, T item, bool dispose) where T : IDisposable
        {
            if (dispose)
            {
                TryDispose(item);
            }

            list.Remove(item);
        }

        /// <summary>
        /// Removes the item at the specified index from the <paramref name="list"/> and disposes of it.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public static void RemoveAtAndDispose<T>(this SourceList<T> list, int index) where T : IDisposable
        {
            list.RemoveAt(index, true);
        }

        /// <summary>
        /// Removes the item at the specified index from the <paramref name="list"/> without disposing of it.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public static void RemoveAtWithoutDispose<T>(this SourceList<T> list, int index) where T : IDisposable
        {
            list.RemoveAt(index, false);
        }

        /// <summary>
        /// Removes the item at the specified index from the <paramref name="list"/> and optionally disposes of it.
        /// </summary>
        /// <typeparam name="T">The type of items in the list, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="list">The <see cref="SourceList{T}"/> to modify.</param>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <param name="dispose">Indicates whether to dispose of the removed item.</param>
        private static void RemoveAt<T>(this SourceList<T> list, int index, bool dispose) where T : IDisposable
        {
            if (dispose)
            {
                var item = list.Items.ElementAt(index);
                TryDispose(item);
            }

            list.RemoveAt(index);
        }

        /// <summary>
        /// Disposes of the specified item if it implements <see cref="IDisposable"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item, which must implement <see cref="IDisposable"/>.</typeparam>
        /// <param name="item">The item to dispose.</param>
        private static void TryDispose<T>(T item) where T : IDisposable
        {
            item?.Dispose();
        }
    }
}
