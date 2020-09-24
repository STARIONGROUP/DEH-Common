// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserPreferenceService.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler.UserPreferenceService
{
    using DEHPCommon.UserPreferenceHandler.Interfaces;

    /// <summary>
    /// Definition of the <see cref="IUserPreferenceService"/> used to load specific settings
    /// </summary>
    public interface IUserPreferenceService<T> where T : IUserPreference
    {
        /// <summary>
        /// Gets or sets  user preference settings,
        /// </summary>
        T UserPreferenceSettings { get; set; }

        /// <summary>
        /// Reads the <see cref="T"/> user preference in settings
        /// </summary>
        /// <typeparam name="T">A type of <see cref="UserPreference"/></typeparam>
        void Read();

        /// <summary>
        /// Writes the <see cref="UserPreference"/> to disk
        /// </summary>
        /// <param name="userPreference">
        /// The <see cref="UserPreference"/> that will be persisted
        /// </param>
        void Write(T userPreference);
    }
}
