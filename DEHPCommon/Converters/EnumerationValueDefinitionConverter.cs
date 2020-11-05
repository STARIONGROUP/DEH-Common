// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationValueDefinitionConverter.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Converters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using CDP4Common.SiteDirectoryData;

    /// <summary>
    /// Convert <see cref="EnumerationValueDefinition"/>(s) to a text to display
    /// </summary>
    public class EnumerationValueDefinitionConverter : IValueConverter
    {
        /// <summary>
        /// Convert the <see cref="EnumerationValueDefinition"/>(s) to a <see cref="List{String}"/> where
        /// each string is the name of the <see cref="EnumerationValueDefinition"/>
        /// </summary>
        /// <param name="value">
        /// The <see cref="EnumerationValueDefinition"/> to convert
        /// </param>
        /// <param name="targetType">
        /// The parameter is not used.
        /// </param>
        /// <param name="parameter">
        /// The parameter is not used.
        /// </param>
        /// <param name="culture">
        /// The parameter is not used.
        /// </param>
        /// <returns>
        /// A <see cref="List{String}"/>
        /// </returns>
        /// <remarks>
        /// The default value "-" is added
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable valueArray))
            {
                return "-";
            }

            var enumArray = valueArray.Cast<EnumerationValueDefinition>().Select(x => x.Name).ToList();
            return (enumArray.Count == 0) ? "-" : string.Join(" | ", enumArray);
        }

        /// <summary>
        /// This operation is not supported.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>
        /// throws not supported exception
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
