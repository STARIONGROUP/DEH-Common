// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserPreferenceService.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler.UserPreferenceService
{
    /// <summary>
    /// Definition of the <see cref="IUserPreferenceService"/> used to load specific settings
    /// </summary>
    public interface IUserPreferenceService
    {
        /// <summary>
        /// Gets or sets  user preference settings,
        /// </summary>
        UserPreference UserPreferenceSettings { get; set; }

        /// <summary>
        /// Reads the <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">A type of <see cref="UserPreference"/></typeparam>
        /// <returns>
        /// An instance of <see cref="UserPreference"/>
        /// </returns>
        T Read<T>() where T : UserPreference;

        /// <summary>
        /// Writes the <see cref="UserPreference"/> to disk
        /// </summary>
        /// <param name="userPreference">
        /// The <see cref="UserPreference"/> that will be persisted
        /// </param>
        void Write<T>(T userPreference) where T : UserPreference;
    }
}
