// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMappingRule.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MappingRules.Core
{
    using System;

    /// <summary>
    /// Defines a mapping rule for <see cref="ObjectProperty"/>
    /// </summary>
    public interface IMappingRule
    {
        /// <summary>
        /// Gets or Sets the property Name of this reprented property
        /// </summary>
        string DstPropertyName { get; }

        /// <summary>
        /// Gets or Sets the internal <see cref="Type"/> Should be IMappingRule
        /// if this reprented property is not of a value or primitive type
        /// </summary>
        Type InternalType { get; }

        /// <summary>
        /// Gets the External property name
        /// </summary>
        string ExternalProperty { get; }

        /// <summary>
        /// Gets the External type <see cref="Type"/>
        /// </summary>
        Type ExternalType { get; }

        /// <summary>
        /// Gets or sets this represented property nested property if applicable
        /// </summary>
        IMappingRule[] NestedRules { get; }
    }
}
