// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingRule.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MappingRules.Core
{
    using System;

    /// <summary>
    /// Reprents a Mappable property usable by the <see cref="MappingEngine"/>
    /// </summary>
    /// <typeparam name="TInput">The input <see cref="Type"/></typeparam>
    /// <typeparam name="TOutput">The output <see cref="Type"/></typeparam>
    public abstract class MappingRule<TInput, TOutput> : IMappingRule<TInput, TOutput>
    { 
        /// <summary>
        /// Transforms <see cref="TInput"/> to a <see cref="TOutput"/>
        /// </summary>
        public abstract TOutput Transform(TInput input);
    }
}
