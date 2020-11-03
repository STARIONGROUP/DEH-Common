// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationValueDefinitionConverterTestFixture.cs" company="RHEA System S.A.">
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

    using CDP4Common;
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.Converters;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="EnumerationValueDefinitionConverter"/> class
    /// </summary>
    [TestFixture]
    public class EnumerationValueDefinitionConverterTestFixture
    {
        private EnumerationValueDefinitionConverter converter;

        [SetUp]
        public void SetUp()
        {
            this.converter = new EnumerationValueDefinitionConverter();
        }

        [Test]
        public void VerifyThatConvertBackThrowsNotSupporedException()
        {
            Assert.Throws<NotSupportedException>(() => this.converter.ConvertBack(null, null, null, null));
        }

        [Test]
        public void VerifyThatAnyOtherClassThanEnumerationValueDefinitionGetsConvertedToHyphen()
        {
            var notThing = new NotThing("nothing");

            Assert.AreEqual("-", this.converter.Convert(notThing, null, null, null));
        }

        [Test]
        public void VerifyThatAListOfEnumerationValueDefinitionsIsConverted()
        {
            var enumeration = new EnumerationParameterType { Name = "Technology Readiness Level", ShortName = "TRL" };

            var level1 = new EnumerationValueDefinition { Name = "1", ShortName = "1" };
            var level2 = new EnumerationValueDefinition { Name = "2", ShortName = "2" };

            enumeration.ValueDefinition.Add(level1);
            enumeration.ValueDefinition.Add(level2);

            var result = this.converter.Convert(enumeration.ValueDefinition, null, null, null);

            Assert.AreEqual("1 | 2", result);
        }

        [Test]
        public void VerfifyThatEmptyListOfValueDefinitionsReturnsHyphen()
        {
            var emptyList = new List<EnumerationValueDefinition>();
            var result = this.converter.Convert(emptyList, null, null, null);

            Assert.AreEqual("-", result);
        }
    }
}
