// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridUpdateService.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
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

namespace DEHPCommon.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    using DevExpress.Xpf.Grid;

    using NLog;

    /// <summary>
    /// The service used to lock the update of a grid view when update in the view-model are occuring
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class GridUpdateService
    {
        /// <summary>
        /// The current logger
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The <see cref="DependencyProperty"/> used to lock the update in a grid
        /// </summary>
        public static readonly DependencyProperty UpdateStartedProperty = DependencyProperty.RegisterAttached("UpdateStarted", typeof(bool?), typeof(GridUpdateService), new FrameworkPropertyMetadata(false, UpdateStartedPropertyChanged));

        /// <summary>
        /// Sets the <see cref="UpdateStartedProperty"/> property
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> where the value is set</param>
        /// <param name="value">The <see cref="bool"/> value</param>
        public static void SetUpdateStarted(UIElement element, bool? value)
        {
            try
            {
                element.SetValue(UpdateStartedProperty, value);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"A problem occurend when SetUpdateStarted was called");
            }
        }

        /// <summary>
        /// Gets the <see cref="UpdateStartedProperty"/> property
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> where the value is used</param>
        /// <returns>A value indicating whether an update is occuring or not</returns>
        public static bool? GetUpdateStarted(UIElement element)
        {
            return (bool?)element.GetValue(UpdateStartedProperty);
        }

        /// <summary>
        /// The <see cref="DependencyPropertyChangedEventArgs"/> event-handler for the <see cref="UpdateStartedProperty"/> property
        /// </summary>
        /// <param name="source">The grid which visual and internal update should be prevented</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/></param>
        private static void UpdateStartedPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (!(source is GridDataControlBase grid))
            {
                throw new InvalidOperationException("The Service can only be used on GridDataControlBase view elements such as GridControl or TreeListControl.");
            }

            try
            {
                switch (e.NewValue)
                {
                    case true:
                        grid.BeginDataUpdate();
                        break;
                    case false when grid is TreeListControl treeListControl:
                        treeListControl.View.CancelRowEdit();
                        treeListControl.EndDataUpdate();
                        break;
                    case false when grid is GridControl gridControl && gridControl.DataController.IsUpdateLocked:
                        // cancel out of any active edit.
                        gridControl.View.CancelRowEdit();
                        grid.EndDataUpdate();
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"A problem occurend when UpdateStartedPropertyChanged was called");
            }
        }
    }
}
