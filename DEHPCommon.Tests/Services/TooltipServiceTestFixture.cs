// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooltipServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.Services
{
    using System;
    using System.Text;

    using NUnit.Framework;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.Services.TooltipService;

    /// <summary>
    /// Suite of tests for the <see cref="TooltipService"/> class
    /// </summary>
    [TestFixture]
    public class TooltipServiceTestFixture
    {
        private Category equippmentCategory;
        private Category productCategory;
        private DomainOfExpertise domainOfExpertise;

        [SetUp]
        public void SetUp()
        {
            this.equippmentCategory = new Category(Guid.NewGuid(), null, null) { ShortName = "EQT" };
            this.productCategory = new Category(Guid.NewGuid(), null, null) { ShortName = "PROD" };
            this.domainOfExpertise = new DomainOfExpertise(Guid.NewGuid(), null, null) { ShortName = "SYS" };
        }

        [Test]
        public void Verif_that_ElementDefinition_tooltip_returns_expected_result()
        {
            var elementDefinition = new ElementDefinition(Guid.NewGuid(), null, null)
            {
                ShortName = "Bat",
                Name = "Battery",
                Owner = this.domainOfExpertise
            };

            elementDefinition.Category.Add(this.productCategory);
            elementDefinition.Category.Add(this.equippmentCategory);

            var definition = new CDP4Common.CommonData.Definition(Guid.NewGuid(), null, null)
            {
                LanguageCode = "en-GB",
                Content = "this is a definition"
            };

            elementDefinition.Definition.Add(definition);

            var tooltip = TooltipService.Tooltip(elementDefinition);
            var expectedToolTip = new StringBuilder();
            expectedToolTip.AppendLine("Short Name: Bat");
            expectedToolTip.AppendLine("Name: Battery");
            expectedToolTip.AppendLine("Owner: SYS");
            expectedToolTip.AppendLine("Category: PROD");
            expectedToolTip.AppendLine("          EQT");
            expectedToolTip.AppendLine("Model Code: Bat");
            expectedToolTip.AppendLine("Definition [en-GB]: this is a definition");
            expectedToolTip.Append("Type: ElementDefinition");

            Assert.That(tooltip, Is.EqualTo(expectedToolTip.ToString()));
        }

        [Test]
        public void Verif_that_ElementUsage_tooltip_returns_expected_result()
        {
            this.equippmentCategory.SuperCategory.Add(this.productCategory);

            var elementDefinition = new ElementDefinition(Guid.NewGuid(), null, null)
            {
                ShortName = "Bat",
                Name = "Battery",
                Owner = this.domainOfExpertise
            };

            elementDefinition.Category.Add(this.equippmentCategory);

            var elementUsage = new ElementUsage(Guid.NewGuid(), null, null)
            {
                ShortName = "bat",
                Name = "battery",
                Owner = this.domainOfExpertise,
                ElementDefinition = elementDefinition
            };

            var definition = new CDP4Common.CommonData.Definition(Guid.NewGuid(), null, null)
            {
                LanguageCode = "en-GB",
                Content = "this is a definition"
            };

            elementUsage.Definition.Add(definition);

            var tooltip = elementUsage.Tooltip();
            var expectedToolTip = new StringBuilder();
            expectedToolTip.AppendLine("Short Name: bat");
            expectedToolTip.AppendLine("Name: battery");
            expectedToolTip.AppendLine("Owner: SYS");
            expectedToolTip.AppendLine("Category: -");
            expectedToolTip.AppendLine("ED Category: EQT {PROD}");
            expectedToolTip.AppendLine("Model Code: Invalid Model Code");
            expectedToolTip.AppendLine("Definition [en-GB]: this is a definition");
            expectedToolTip.Append("Type: ElementUsage");

            Assert.That(tooltip, Is.EqualTo(expectedToolTip.ToString()));
        }
    }
}
