// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooltipService.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Services.TooltipService
{
    using System;
    using System.Linq;
    using System.Text;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using NLog;

    /// <summary>
    /// The purpose of the <see cref="TooltipService"/> is to derive the tooltp text of a specific <see cref="Thing"/>
    /// </summary>
    public static class TooltipService
    {
        /// <summary>
        /// The current logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns a string that represents a tooltip text for the specified <see cref="Thing"/>
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static string Tooltip(this Thing thing)
        {
            if (thing.ChangeKind == ChangeKind.Create)
            {
                // when creating things, especially in nested creted dialogs there are multiple opportunities for various exceptions
                // occuring. i.e. ContainmentException when calling .Owner when that property is derived. So skip the tooltips in this case.
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            if (thing is IShortNamedThing shortNamedThing)
            {
                stringBuilder.AppendLine($"Short Name: {shortNamedThing.ShortName}");
            }

            if (thing is INamedThing namedThing)
            {
                stringBuilder.AppendLine($"Name: {namedThing.Name}");
            }

            GetOwner(thing, stringBuilder);
            GetCategories(thing, stringBuilder);
            GetModelCode(thing, stringBuilder);
            GetDefinition(thing, stringBuilder);

            stringBuilder.Append($"Type: {thing.ClassKind}");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the <see cref="DomainOfExpertise"/> of the provided <see cref="Thing"/> and append it to the <see cref="StringBuilder"/> 
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/></param>
        private static void GetOwner(Thing thing, StringBuilder stringBuilder)
        {
            if (!(thing is IOwnedThing ownedThing))
            {
                return;
            }

            string owner;

            if (ownedThing.Owner != null)
            {
                owner = ownedThing.Owner.ShortName;
            }
            else
            {
                owner = "NA";
                Logger.Debug($"Owner of {thing.ClassKind} is null");
            }

            stringBuilder.AppendLine($"Owner: {owner}");
        }

        /// <summary>
        /// Gets the <see cref="Definition"/> of the provided <see cref="Thing"/> and append it to the <see cref="StringBuilder"/> 
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/></param>
        private static void GetDefinition(Thing thing, StringBuilder stringBuilder)
        {
            if (!(thing is DefinedThing definedThing))
            {
                return;
            }

            var definition = definedThing.Definition.FirstOrDefault();

            stringBuilder.AppendLine(definition == null
                ? "Definition : -"
                : $"Definition [{definition.LanguageCode}]: {definition.Content}");
        }
        
        /// <summary>
        /// Gets the <see cref="IModelCode"/> of the provided <see cref="Thing"/> and append it to the <see cref="StringBuilder"/> 
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/></param>
        private static void GetModelCode(Thing thing, StringBuilder stringBuilder)
        {
            if (!(thing is IModelCode modelCodeThing))
            {
                return;
            }

            string modelCode;

            try
            {
                modelCode = modelCodeThing.ModelCode();
            }
            catch (Exception e)
            {
                modelCode = "Invalid Model Code";
                Logger.Error(e);
            }

            stringBuilder.AppendLine($"Model Code: {modelCode}");
        }
        
        /// <summary>
        /// Gets the <see cref="Category"/>s of the provided <see cref="Thing"/> and append it to the <see cref="StringBuilder"/> 
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/></param>
        private static void GetCategories(Thing thing, StringBuilder stringBuilder)
        {
            if (!(thing is ICategorizableThing categorizableThing))
            {
                return;
            }

            if (!categorizableThing.Category.Any())
            {
                stringBuilder.AppendLine($"Category: -");
            }
            else
            {
                ProcessCategories(stringBuilder, categorizableThing, "Category");
            }

            if (!(thing is ElementUsage elementUsage))
            {
                return;
            }

            if (elementUsage.ElementDefinition.Category.Count == 0)
            {
                stringBuilder.AppendLine($"ED Category: -");
            }
            else
            {
                ProcessCategories(stringBuilder, elementUsage.ElementDefinition, "ED Category");
            }
        }

        /// <summary>
        /// Flattens the categories of the <paramref name="categorizableThing"/> into a string and appends it to the <paramref name="stringBuilder"/>
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/></param>
        /// <param name="categorizableThing">The <see cref="ICategorizableThing"/></param>
        /// <param name="categoryLabel">The label that states whether the categories are attached to an <see cref="ElementDefinition"/> of a <see cref="ElementUsage"/></param>
        private static void ProcessCategories(StringBuilder stringBuilder, ICategorizableThing categorizableThing, string categoryLabel)
        {
            var categoryCounter = 1;

            foreach (var category in categorizableThing.Category)
            {
                var superCategoryShortNames = category.SuperCategory.Count != 0 ? " {" + string.Join(", ", category.SuperCategory.Select(x => x.ShortName)) + "}" : string.Empty;
                var categoryEntry = $"{category.ShortName}{superCategoryShortNames}";
                stringBuilder.AppendLine(categoryCounter == 1 ? $"{categoryLabel}: {categoryEntry}" : $"          {categoryEntry}");

                categoryCounter++;
            }
        }
    }
}
