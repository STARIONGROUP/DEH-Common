// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerConnection.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler
{
    using DEHPCommon.UserPreferenceHandler.Enums;

    /// <summary>
    /// Definition of the <see cref="ServerConnection"/> used to handle specific settings
    /// </summary>
    public class ServerConnection
    {
        /// <summary>
        /// Gets or sets server serverType value
        /// </summary>
        public ServerType ServerType { get; set; }

        /// <summary>
        /// Gets or sets server uri
        /// </summary>
        public string Uri { get; set; }
    }
}
