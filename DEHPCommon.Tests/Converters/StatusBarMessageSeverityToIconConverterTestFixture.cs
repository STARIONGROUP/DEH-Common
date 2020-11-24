// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusBarMessageSeverityToIconConverterTestFixture.cs" company="RHEA System S.A.">
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

using NUnit.Framework;

namespace DEHPCommon.Tests.Converters
{
    using System;
    using System.Windows.Media.Imaging;

    using DEHPCommon.Converters;
    using DEHPCommon.Enumerators;

    [TestFixture]
    public class StatusBarMessageSeverityToIconConverterTestFixture
    {
        private StatusBarMessageSeverityToIconConverter converter;

        [SetUp]
        public void SetUp()
        {
            this.converter = new StatusBarMessageSeverityToIconConverter();

            if (!UriParser.IsKnownScheme("pack"))
            {
                _ = new System.Windows.Application();
            }
        }

        [Test]
        public void VerifyThatConvertReturnsExpectedResult()
        {
            Assert.IsNull(this.converter.Convert(null, null, null, null));
            Assert.IsNull(this.converter.Convert(0.5, null, null, null));

            Assert.IsNull(this.converter.Convert(StatusBarMessageSeverity.None, null, null, null));
            Assert.AreEqual($"pack://application:,,,/DEHPCommon;component/Resources/Images/Info_16x16.png", (this.converter.Convert(StatusBarMessageSeverity.Info, null, null, null) as BitmapImage)?.UriSource.ToString());
            Assert.AreEqual($"pack://application:,,,/DEHPCommon;component/Resources/Images/ExclamationRed_16x16.png", (this.converter.Convert(StatusBarMessageSeverity.Error, null, null, null) as BitmapImage)?.UriSource.ToString());
            Assert.AreEqual($"pack://application:,,,/DEHPCommon;component/Resources/Images/Warning_16x16.png", (this.converter.Convert(StatusBarMessageSeverity.Warning, null, 0, null) as BitmapImage)?.UriSource.ToString());
        }

        [Test]
        public void VerifyThatConvertBackIsNotSupported()
        {
            Assert.Throws<NotSupportedException>(() => this.converter.ConvertBack(null, null, null, null));
        }
    }
}
