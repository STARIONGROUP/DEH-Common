// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusBarMessageSeverityToIconConverter.cs" company="RHEA System S.A.">
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
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using DEHPCommon.Enumerators;

    /// <summary>
    /// Converts <see cref="StatusBarMessageSeverity"/> to <see cref="BitmapImage"/> other value than 1,2,3 will make the converter return null
    /// </summary>
    public class StatusBarMessageSeverityToIconConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="StatusBarMessageSeverity"/> to an <see cref="BitmapImage"/>
        /// </summary>
        /// <param name="value">The incoming type.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The converter paramete.</param>
        /// <param name="culture">The supplied culture</param>
        /// <returns>An <see cref="BitmapImage"/> or null</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var image = value switch
            {
                StatusBarMessageSeverity.None => string.Empty,
                StatusBarMessageSeverity.Info => "Info_16x16.png",
                StatusBarMessageSeverity.Warning => "Warning_16x16.png",
                StatusBarMessageSeverity.Error => "ExclamationRed_16x16.png",
                _ => string.Empty
            };

            return string.IsNullOrWhiteSpace(image) ? null : new BitmapImage(new Uri($"pack://application:,,,/DEHPCommon;component/Resources/Images/{image}"));
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="value">The incoming collection.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter passed on to this conversion.</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>Throws <see cref="NotImplementedException"/> always.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
