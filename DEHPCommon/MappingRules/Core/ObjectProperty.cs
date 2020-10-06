// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectProperty.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MappingRules.Core
{
    using System;

    /// <summary>
    /// Reprents a Mappable property usable by the <see cref="MappingEngine"/> and <see cref="ObjectType"/>
    /// </summary>
    public class ObjectProperty : IMappingRule
    {
        /// <summary>
        /// Gets or Sets the property Name of this reprented property
        /// </summary>
        public string DstPropertyName { get; }

        /// <summary>
        /// Gets or Sets the internal <see cref="Type"/> Should be IMappingRule
        /// if this reprented property is not of a value or primitive type
        /// </summary>
        public Type InternalType { get; } = typeof(object);

        /// <summary>
        /// Gets the External property name
        /// </summary>
        public string ExternalProperty { get; }

        /// <summary>
        /// Gets the External type <see cref="Type"/>
        /// </summary>
        public Type ExternalType { get; }

        /// <summary>
        /// Gets or sets this represented property nested property if applicable
        /// </summary>
        public IMappingRule[] NestedRules { get; } = { };

        /// <summary>
        /// Initializes a new <see cref="ObjectProperty"/>. Use this one for simple type
        /// </summary>
        /// <param name="dstPropertyName"></param>
        /// <param name="internalType"></param>
        /// <param name="externalProperty"></param>
        public ObjectProperty(string dstPropertyName, Type internalType, string externalProperty)
        {
            this.DstPropertyName = dstPropertyName;
            this.InternalType = internalType;
            this.ExternalProperty = externalProperty;
        }

        /// <summary>
        /// Initializes a new <see cref="ObjectProperty"/>. Use this one for Reference type
        /// </summary>
        /// <param name="dstPropertyName"></param>
        /// <param name="externalType"></param>
        /// <param name="nestedRules"></param>
        public ObjectProperty(string dstPropertyName, Type externalType, IMappingRule[] nestedRules)
        {
            this.DstPropertyName = dstPropertyName;
            this.ExternalType = externalType;
            this.NestedRules = nestedRules;
        }
    }
}
