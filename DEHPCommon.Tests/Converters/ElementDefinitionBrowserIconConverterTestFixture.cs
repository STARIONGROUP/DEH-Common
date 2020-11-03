// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionBrowserIconConverterTestFixture.cs" company="RHEA System S.A.">
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
    using System.Reflection;
    using System.Threading;
    using System.Windows.Media.Imaging;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.Converters;
    using DEHPCommon.Services.IconCacheService;
    using DEHPCommon.Utilities;

    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// suite of tests for the <see cref="ThingToIconUriConverter"/>
    /// </summary>
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ElementDefinitionBrowserIconConverterTestFixture
    {
        /// <summary>
        /// the <see cref="ThingToIconUriConverter"/> under test
        /// </summary>
        private ElementDefinitionBrowserIconConverter converter;

        private ThingToIconUriConverter genericConverter;

        private IconCacheService iconCacheService;

        [SetUp]
        public void SetUp()
        {
            this.iconCacheService = new IconCacheService();

            this.converter = new ElementDefinitionBrowserIconConverter();
            this.genericConverter = new ThingToIconUriConverter();
            AppContainer.BuildContainer();
        }

        [Test]
        public void VerifyThatConvertProvidesTheExpectedIconForElementDefinition()
        {
            var generic = (BitmapImage)this.genericConverter.Convert(new object[] { new ElementDefinition() }, null, null, null);
            var icon = (BitmapImage)this.converter.Convert(new object[] { new ElementDefinition() }, null, null, null);

            Assert.AreEqual(generic.UriSource.ToString(), icon.UriSource.ToString());
        }

        [Test]
        public void VerifyThatConvertProvidesTheExpectedIconForParameterBase()
        {
            var generic = (BitmapImage)this.genericConverter.Convert(new object[] { new Parameter(), }, null, null, null);
            var icon = (BitmapImage)this.converter.Convert(new object[] { new Parameter() }, null, null, null);

            Assert.AreEqual(generic.UriSource.ToString(), icon.UriSource.ToString());
        }

        [Test]
        public void VerifyThatConvertProvidesTheExpectedIconForParameterBaseWithOptionNoState()
        {
            var generic = (BitmapImage)this.genericConverter.Convert(new object[] { new Option(), }, null, null, null);

            var parameter = new Parameter { ParameterType = new BooleanParameterType() };
            var icon = this.converter.Convert(new object[] { parameter }, null, ClassKind.Option, null);
            // overlay
            Assert.IsTrue(icon is BitmapSource);
        }

        [Test]
        public void VerifyThatConvertProvidesTheExpectedIconForParameterBaseWithOptionWithState()
        {
            var generic = (BitmapImage)this.genericConverter.Convert(new object[] { new Option() }, null, null, null);

            var parameter = new Parameter { ParameterType = new BooleanParameterType(), StateDependence = new ActualFiniteStateList() };
            var icon = (BitmapImage)this.converter.Convert(new object[] { new ThingStatus(parameter) }, null, ClassKind.Option, null);

            // overlay
            Assert.AreEqual(generic.UriSource.ToString(), icon.UriSource.ToString());
        }

        [Test]
        public void VerifyThatConvertBackThrowsException()
        {
            Assert.Throws<NotSupportedException>(() => this.converter.ConvertBack(null, null, null, null));
        }
    }
}
