// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingEngine.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MappingEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.MappingRules;
    using DEHPCommon.MappingRules.Core;

    /// <summary>
    /// The <see cref="MappingEngine"/> allows to map dst tool models to ECSS-E-TM-10-25A model
    /// </summary>
    public class MappingEngine
    {
        /// <summary>
        /// Maps an existing non ECSS-E-TM-10-25A Model to a new ECSS-E-TM-10-25A model
        /// </summary>
        /// <param name="model">The external model</param>
        /// <param name="mappingRules">The rules that allows the <see cref="MappingEngine"/> to map</param>
        /// <returns></returns>
        public EngineeringModel MapForward(object obj, params IMappingRule[] mappingRules)
        {
            var model = new ObjectType(obj, mappingRules);

            var EngineeringModel = new EngineeringModel();

            foreach (var nameTypeInstance in model.Properties)
            {
                var propertyRule = (model.MappingRules as IMappingRule[])?.FirstOrDefault(x => x.DstPropertyName == nameTypeInstance.Key);
                var propertyInstance = nameTypeInstance.Value.Instance;
                EngineeringModel.GetType().GetProperty(propertyRule.ExternalProperty).SetValue(EngineeringModel, propertyInstance);
            }

            return EngineeringModel;
        }

        private bool TryGetPropertyValue<TPropertyType>(object undefinedObject, string name, out TPropertyType value)
        {
            value = default;

            var type = undefinedObject.GetType();

            value = (TPropertyType)(type.GetProperty(name)?.GetValue(undefinedObject, null) ??
                        type.GetProperties().FirstOrDefault(x => x.Name == name));

            return value != default(TPropertyType);
        }
    }
}
