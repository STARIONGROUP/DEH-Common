// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeConverter.cs" company="RHEA System S.A.">
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
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    /// <summary>
    /// Converts an object to its <see cref="Type"/>.
    /// </summary>
    public class DataTypeConverter : IValueConverter
    {
        /// <summary>
        /// Returns the type of the supplied object.
        /// </summary>
        /// <param name="value">The incoming object.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="culture">The supplied culture</param>
        /// <returns><see cref="Visibility.Visible"/> if the supplied type is of type <see cref="ElementUsageRowViewModel"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType();
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="value"> The incoming collection. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="parameter"> The parameter passed on to this conversion. </param>
        /// <param name="culture"> The culture information. </param>
        /// <returns> Throws <see cref="NotImplementedException"/> always. </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
