// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreference.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler
{
    using System.Collections.Generic;

    /// <summary>
    /// Base class from which all <see cref="UserPreference"/> shall derive
    /// </summary>
    public class UserPreference
    {
        /// <summary>
        /// A list used to store the server connection settings,
        /// </summary>
        public List<ServerConnection> SavedServerConections { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="UserPreference"/>
        /// </summary>
        public UserPreference()
        {
            this.SavedServerConections = new List<ServerConnection>();
        }
    }
}
