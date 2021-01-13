// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextMenuBehavior.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using DevExpress.Mvvm.UI.Interactivity;
    using DevExpress.Xpf.Grid;

    using NLog;

    /// <summary>
    /// This behavior creates the context menu when the user right mouse clicks on a row in a <see cref="IHaveContextMenuViewModel"/>.
    /// </summary>
    public class ContextMenuBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// The current logger
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The on attached event handler
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseRightButtonUp += this.PopulateContextMenu;
        }

        /// <summary>
        /// Remove the subscription on detaching
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.MouseRightButtonUp -= this.PopulateContextMenu;
        }

        /// <summary>
        /// Event handler for the <see cref="MouseRightButtonUp"/> event
        /// </summary>
        /// <remarks>
        /// Occurs when the right mouse button is released while the mouse pointer is over this element.
        /// </remarks>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">the <see cref="MouseButtonEventArgs"/> associated to the event</param>
        private void PopulateContextMenu(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!(sender is GridDataControlBase control))
                {
                    return;
                }

                if (!(control.DataContext is IHaveContextMenuViewModel browser))
                {
                    return;
                }

                browser.PopulateContextMenu();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "A problem occurend when populating the ContextMenu");
            }
        }
    }
}