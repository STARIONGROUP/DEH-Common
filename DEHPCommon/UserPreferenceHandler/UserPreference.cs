// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreference.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler
{
    /// <summary>
    /// Base class from which all <see cref="UserPreference"/> shall derive
    /// </summary>
    public class UserPreference
    {
        /// <summary>
        /// Gets or sets server serverType value
        /// </summary>
        public string ServerType { get; set; }

        /// <summary>
        /// Gets or sets server uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets server username value
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets server password value
        /// </summary>
        public string Password { get; set; }
    }
}
