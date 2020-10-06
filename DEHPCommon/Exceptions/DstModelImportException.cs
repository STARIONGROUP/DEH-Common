// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DstModelImportException.cs"company="RHEA System S.A.">
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
    public class DstModelImportException : Exception
    {
        /// <inheritdoc cref="Exception(string)"/>
        public DstModelImportException(string message) : base(message)
        {
        }

        /// <inheritdoc cref="Exception(string, Exception)"/>
        public DstModelImportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
