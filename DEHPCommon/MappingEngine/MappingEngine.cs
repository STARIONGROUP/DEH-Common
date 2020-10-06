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

    using DEHPCommon.MappingRules.Core;

    using NLog;

    /// <summary>
    /// The <see cref="MappingEngine"/> allows to map dst tool models to ECSS-E-TM-10-25A model
    /// </summary>
    public class MappingEngine
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger(typeof(MappingEngine));

        private readonly Assembly assembly = Assembly.GetExecutingAssembly();

        public IEnumerable<(Type InputType, Type MappingRuleType)> Rules { get; private set; } = new List<(Type InputType, Type MappingRuleType)>();

        public MappingEngine()
        {
            this.PopulateRules();
        }

        /// <summary>
        /// Maps the provided <see cref="object"/> to another type if a rule is found
        /// </summary>
        /// <param name="obj">The external object to map</param>
        /// <returns>The new object</returns>
        public object Map(object obj)
        {
            if (!this.Rules.Any())
            {
                return null;
            }

            foreach (var (inputType, mappingRuleType) in this.Rules)
            {
                if (this.TryCast(inputType, obj, out var newObject ))
                {
                    var ruleInstance = Activator.CreateInstance(mappingRuleType, newObject);
                    ruleInstance.Transform();
                    return ruleInstance.Output;
                }
            }

            logger.Error($"Could not map {obj}, no corresponding mapping rule has been found");
            return null;
        }

        private bool TryCast(Type type, object obj, out dynamic result)
        {
            result = default;

            if (type.IsInstanceOfType(obj))
            {
                result = Convert.ChangeType(obj, type);
                return true;
            }

            return false;
        }

        public void PopulateRules()
        {
            var assignableType = new List<(Type InputType, Type MappingRuleType)>();

            foreach (var type in this.assembly.GetTypes())
            {
                if (type.GetInterface(nameof(IMappingRule)) != null && type.BaseType?.IsAbstract == true && !type.IsAbstract)
                {
                    var typeArguments = type.BaseType?.GetGenericArguments();
                    assignableType.Add((typeArguments[0], type));
                }
            }

            this.Rules = assignableType;
        }
    }
}
