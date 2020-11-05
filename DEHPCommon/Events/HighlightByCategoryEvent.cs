// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightByCategoryEvent.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Events
{
    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;

    /// <summary>
    /// The purpose of the <see cref="HighlightByCategoryEvent"/> is to notify an observer
    /// that it has to highlight if it is categorized by the provided <see cref="CDP4Common.SiteDirectoryData.Category"/>.
    /// </summary>
    public class HighlightByCategoryEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightByCategoryEvent"/> class.
        /// </summary>
        /// <param name="category">
        /// The payload <see cref="Category"/> to highlight <see cref="Thing"/>s by.
        /// </param>
        public HighlightByCategoryEvent(Category category)
        {
            this.Category = category;
        }

        /// <summary>
        /// Gets or sets the <see cref="Category"/> by which <see cref="Thing"/>s should be highlighted.
        /// </summary>
        public Category Category { get; set; }
    }
}
