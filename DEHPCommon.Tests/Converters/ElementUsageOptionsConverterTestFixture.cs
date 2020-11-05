// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementUsageOptionsConverterTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using DEHPCommon.Converters;

    using NUnit.Framework;

    using ReactiveUI;

    [TestFixture]
    public class ElementUsageOptionsConverterTestFixture
    {
        private ElementUsageOptionsConverter converter;

        [SetUp]
        public void Setup()
        {
            this.converter = new ElementUsageOptionsConverter();
        }

        [Test]
        public void VerifyConvert()
        {
            Assert.AreEqual(new List<object>(), this.converter.Convert(null, null, null, CultureInfo.CurrentCulture));
            Assert.AreEqual(new List<object>() { 'a', 'b', 'c'}, this.converter.Convert(new List<char>() { 'a', 'b', 'c' }, null, null, CultureInfo.CurrentCulture));
            Assert.Throws<InvalidCastException>(() => this.converter.Convert(1, null, null, CultureInfo.CurrentCulture));
        }

        [Test]
        public void VerifyConvertBack()
        {
            Assert.Throws<InvalidCastException>(() => this.converter.ConvertBack(-1, null, null, CultureInfo.CurrentCulture));
            Assert.AreEqual(new ReactiveList<Option>(), this.converter.ConvertBack(null, null, null, CultureInfo.CurrentCulture));
            Assert.Throws<InvalidCastException>(() => this.converter.ConvertBack(new List<char>() { 'a', 'b', 'c' }, null, null, CultureInfo.CurrentCulture));

            var options = new List<Option>()
            {
                new Option() { Name = "Option0" }, new Option() { Name = "Option1" }, new Option() { Name = "Option1" }
            };

            Assert.AreEqual(new ReactiveList<Option>(options), this.converter.ConvertBack(options, null, null, CultureInfo.CurrentCulture));
        }
    }
}
