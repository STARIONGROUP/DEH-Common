// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingUtility.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MappingEngine.Utilities
{
    using System;

    /// <summary>
    /// Provides Utility methods to help the <see cref="MappingEngine"/>
    /// </summary>
    public static class MappingUtility
    {
        /// <summary>
        /// Computes whether the provided type is a <see cref="ValueType"/>, a <see cref="string"/><see cref="Type"/>,
        /// a <see cref="decimal"/><see cref="Type"/> or even a <see cref="Enum"/><see cref="Type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to verify</param>
        /// <returns>An assert</returns>
        public static bool IsSimpleType(Type type)
        {
            while (true)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                    continue;
                }

                return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);
            }
        }
    }
}
