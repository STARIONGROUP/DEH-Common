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

            var sb = new StringBuilder();

            if (thing is IShortNamedThing shortNamedThing)
            {
                sb.AppendLine($"Short Name: {shortNamedThing.ShortName}");
            }

            if (thing is INamedThing namedThing)
            {
                sb.AppendLine($"Name: {namedThing.Name}");
            }

            if (thing is IOwnedThing ownedThing)
            {
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

                sb.AppendLine($"Owner: {owner}");
            }

            if (thing is ICategorizableThing categorizableThing)
            {
                if (!categorizableThing.Category.Any())
                {
                    sb.AppendLine($"Category: -");
                }
                else
                {
                    var categoryCounter = 1;
                    
                    foreach (var category in categorizableThing.Category)
                    {
                        var superCategoryShortNames = category.SuperCategory.Count != 0 ? " {" + string.Join(", ", category.SuperCategory.Select(x => x.ShortName)) + "}" : string.Empty;
                        var categoryEntry = $"{category.ShortName}{superCategoryShortNames}";
                        sb.AppendLine(categoryCounter == 1 ? $"Category: {categoryEntry}" : $"          {categoryEntry}");

                        categoryCounter++;
                    }
                }

                if (thing is ElementUsage elementUsage)
                {
                    if (elementUsage.ElementDefinition.Category.Count == 0)
                    {
                        sb.AppendLine($"ED Category: -");
                    }
                    else
                    {
                        var categoryCounter = 1;
                        
                        foreach (var category in elementUsage.ElementDefinition.Category)
                        {
                            var superCategoryShortNames = category.SuperCategory.Count != 0 ? " {" + string.Join(", ", category.SuperCategory.Select(x => x.ShortName)) + "}" : string.Empty;
                            var categoryEntry = $"{category.ShortName}{superCategoryShortNames}";
                            sb.AppendLine(categoryCounter == 1 ? $"ED Category: {categoryEntry}" : $"           {categoryEntry}");

                            categoryCounter++;
                        }
                    }
                }
            }

            if (thing is IModelCode modelCodeThing)
            {
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

                sb.AppendLine($"Model Code: {modelCode}");
            }

            if (thing is DefinedThing definedThing)
            {
                var definition = definedThing.Definition.FirstOrDefault();
                
                sb.AppendLine(definition == null
                    ? "Definition : -"
                    : $"Definition [{definition.LanguageCode}]: {definition.Content}");
            }

            sb.Append($"Type: {thing.ClassKind}");

            return sb.ToString();
        }
    }
}
