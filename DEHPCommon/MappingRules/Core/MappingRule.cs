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
    public abstract class MappingRule<TInput, TOutput> : IMappingRule
    { 
        /// <summary>
        /// Gets the output type
        /// </summary>
        public TOutput Output { get; protected set; }

        /// <summary>
        /// Gets or Sets the input
        /// </summary>
        public TInput Input { get; set; }
        
        /// <summary>
        /// Initializes a new <see cref="MappingRule{T,T}"/>
        /// </summary>
        protected MappingRule()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="MappingRule{T,T}"/>
        /// </summary>
        /// <param name="input">The input <see cref="TInput"/></param>
        protected MappingRule(TInput input)
        {
            this.Input = input;
        }

        /// <summary>
        /// Transforms <see cref="TInput"/> to a <see cref="TOutput"/>
        /// </summary>
        public abstract void Transform();
    }
}
