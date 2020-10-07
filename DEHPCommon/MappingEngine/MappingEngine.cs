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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(typeof(MappingEngine));

        public Dictionary<Type, IMappingRule> Rules { get; private set; } = new Dictionary<Type, IMappingRule>();

        public MappingEngine(Assembly ruleAssembly)
        {
            this.PopulateRules(ruleAssembly);
        }

        /// <summary>
        /// Maps the provided <see cref="object"/> to another type if a rule is found
        /// </summary>
        /// <param name="input">The external object to map</param>
        /// <returns>The new object</returns>
        public object Map(object input)
        {
            if (!this.Rules.Any())
            {
                return null;
            }

            var ruleExist = this.Rules.TryGetValue(input.GetType(), out var foundRule);

            if (ruleExist)
            {
                return foundRule.GetType().GetMethod("Transform")?.Invoke(foundRule, new[] { input });
            }
                
            Logger.Error($"Could not map {input}, no corresponding mapping rule has been found");
            return null;
        }

        /// <summary>
        /// Populates the rules that have been found. A rule shall implement <see cref="MappingRule{TInput,TOutput}"/>
        /// </summary>
        /// <param name="ruleAssembly"></param>
        private void PopulateRules(Assembly ruleAssembly)
        {
            var assignableType =  new Dictionary<Type, IMappingRule>();

            foreach (var type in ruleAssembly.GetTypes().Where(x => x.GetInterface(nameof(IMappingRule)) != null && x.BaseType != null && x.BaseType.IsAbstract && !x.IsAbstract))
            {
                var typeArguments = type.BaseType?.GetGenericArguments();
                assignableType.Add(typeArguments[0], (IMappingRule)Activator.CreateInstance(type));
            }

            this.Rules = assignableType;
        }
    }
}
