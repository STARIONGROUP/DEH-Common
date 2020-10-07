// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMappingRule.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MappingRules.Core
{
    /// <summary>
    /// Defines a mapping rule for <see cref="MappingRule{T,T}"/>
    /// </summary>
    /// <typeparam name="TInput">The input <see cref="Type"/></typeparam>
    /// <typeparam name="TOutput">The output <see cref="Type"/></typeparam>
    public interface IMappingRule<TInput, TOutput> : IMappingRule
    {
        /// <summary>
        /// Transforms a object of type <see cref="TInput"/> to another one of type <see cref="TOutput"/>
        /// </summary>
        TOutput Transform(TInput input);
    }

    public interface IMappingRule
    {
    }
}
