// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserPreference.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Base interface from which all <see cref="UserPreference"/> shall derive
    /// </summary>
    public interface IUserPreference
    {
        /// <summary>
        /// A list used to store the server connection settings,
        /// </summary>
        List<ServerConnection> SavedServerConections { get; set; }
    }
}
