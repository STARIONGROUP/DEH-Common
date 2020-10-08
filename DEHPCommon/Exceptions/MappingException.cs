// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingException.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Exceptions
{
    using System;

    /// <summary>
    /// Can be thrown by the mapping engine when the mapping fails
    /// </summary>
    public class MappingException : Exception
    {
        /// <inheritdoc cref="Exception(string, Exception)"/>
        public MappingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
