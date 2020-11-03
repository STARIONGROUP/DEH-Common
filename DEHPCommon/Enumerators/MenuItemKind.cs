// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuItemKind.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Enumerators
{
    /// <summary>
    /// An enumeration data type used to identify the kind of command associated to a menu-item
    /// </summary>
    public enum MenuItemKind
    {
        /// <summary>
        /// Assertion that the associated menu item is not any of the other known enumeration value definitions.
        /// </summary>
        /// <remarks>
        /// None is the default enumeration value.
        /// </remarks>
        None = 0,

        /// <summary>
        /// Assertion that the associated menu item is used to create.
        /// </summary>
        Create = 1,

        /// <summary>
        /// Assertion that the associated menu item is used to edit.
        /// </summary>
        Edit = 2,

        /// <summary>
        /// Assertion that the associated menu item is used to inspect.
        /// </summary>
        Inspect = 3,

        /// <summary>
        /// Assertion that the associated menu item is used to Delete.
        /// </summary>
        Delete = 4,

        /// <summary>
        /// Assertion that the associated menu item is used to deprecate.
        /// </summary>
        Deprecate = 5,

        /// <summary>
        /// Assertion that the associated menu item is used to refresh the data.
        /// </summary>
        Refresh = 6,

        /// <summary>
        /// Assertion that the associated menu item is used to export the data.
        /// </summary>
        Export = 7,

        /// <summary>
        /// Assertion that the associated menu item is used to invoke the help.
        /// </summary>
        Help = 8,

        /// <summary>
        /// Assertion that the associated menu item is used to highlight.
        /// </summary>
        Highlight = 9,

        /// <summary>
        /// Assertion that the associated menu item is used to copy.
        /// </summary>
        Copy = 10,

        /// <summary>
        /// Assertion that the associated menu item is used to save.
        /// </summary>
        Save = 11,

        /// <summary>
        /// Assertion that the associated menu item is used to navigate to a Thing
        /// </summary>
        Navigate = 12,

        /// <summary>
        /// Assertion that the associated menu item is used to save a Thing to favorites
        /// </summary>
        Favorite = 13
    }
}
