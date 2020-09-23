// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreferenceService.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler.UserPreferenceService
{
    using System;
    using System.IO;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using NLog;

    /// <summary>
    /// Definition of the <see cref="UserPreferenceService"/> used to load specific settings
    /// </summary>
    public class UserPreferenceService : IUserPreferenceService
    {
        /// <summary>
        /// The logger for the current class
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The path of the User Preference directory storage
        /// </summary>
        public static string UserPreferenceDataFolder = Environment.CurrentDirectory;

        /// <summary>
        /// Application configuration folder path.
        /// </summary>
        public static string DirectoryFolder = "UserPreferenceService";

        /// <summary>
        /// The setting file extension
        /// </summary>
        public const string SETTING_FILE_EXTENSION = ".settings.json";

        /// <summary>
        /// Gets or sets  user preference setting,
        /// </summary>
        private UserPreference userPreferenceSettings { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="UserPreferenceService"/>
        /// </summary>
        public UserPreferenceService()
        {
            this.userPreferenceSettings = new UserPreference();
        }

        /// <summary>
        /// Reads the <see cref="T"/> user preference in settings
        /// </summary>
        /// <typeparam name="T">A type of <see cref="UserPreference"/></typeparam>
        /// <returns>
        /// An instance of <see cref="UserPreference"/>
        /// </returns>
        public T Read<T>() where T : UserPreference
        {
            var assemblyName = this.QueryAssemblyTitle(typeof(T));

            if (this.userPreferenceSettings.TryGetValue(assemblyName, out var result))
            {
                return result as T;
            }

            this.CheckConfigurationDirectory();

            var path = Path.Combine(this.ApplicationDirectory, assemblyName);

            logger.Debug("Read user preference for {0} from {1}", assemblyName, path);

            try
            {
                using (var file = File.OpenText($"{path}{SETTING_FILE_EXTENSION}"))
                {
                    var serializer = new JsonSerializer();
                    result = (T)serializer.Deserialize(file, typeof(T));

                    // once the settings have been read from disk, add them to the cache for fast access
                    this.userPreferenceSettings Add(assemblyName, result);

                    return (T)result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "The user preference could not be read");

                throw new Exception("The user preference could not be read", ex);
            }
        }

        /// <summary>
        /// Writes the <see cref="UserPreference"/> to disk
        /// </summary>
        /// <param name="userPreference">
        /// The <see cref="UserPreference"/> that will be persisted
        /// </param>
        public void Write<T>(T userPreference) where T : UserPreference
        {
            if (userPreference == null)
            {
                throw new ArgumentNullException(nameof(userPreference), "This may not be null");
            }

            var assemblyName = this.QueryAssemblyTitle(userPreference.GetType());

            this.CheckConfigurationDirectory();

            var path = Path.Combine(this.ApplicationDirectory, $"{assemblyName}{SETTING_FILE_EXTENSION}");

            logger.Debug("Write user preference to for {0} to {1}", assemblyName, path);

            using (var streamWriter = File.CreateText(path))
            {
                var serializer = new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                };

                serializer.Serialize(streamWriter, userPreference);
            }

            if (this.userPreferenceSettings.ContainsKey(assemblyName))
            {
                this.userPreferenceSettings[assemblyName] = userPreference;
            }
            else
            {
                this.userPreferenceSettings.Add(assemblyName, userPreference);
            }
        }

        /// <summary>
        /// Queries the name of the assembly that contains the <see cref="Type"/>
        /// </summary>
        /// <param name="type">
        /// The <see cref="Type"/> that is contained in the assembly for which the name is queried.
        /// </param>
        /// <returns>
        /// A string that contains the name of the assembly
        /// </returns>
        private string QueryAssemblyTitle(Type type)
        {
            return ((AssemblyTitleAttribute)type.Assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute)))
                .Title;
        }

        /// <summary>
        /// Configuration file Directory
        /// </summary>
        public string ApplicationDirectory
        {
            get { return Path.Combine(UserPreferenceDataFolder, DirectoryFolder); }
        }

        /// <summary>
        /// Checks for the existence of the <see cref="ApplicationDirectory"/>
        /// </summary>
        public void CheckConfigurationDirectory()
        {
            if (!Directory.Exists(this.ApplicationDirectory))
            {
                logger.Debug("The user preference folder {0} does not yet exist", this.ApplicationDirectory);
                Directory.CreateDirectory(this.ApplicationDirectory);
                logger.Debug("The user preference folder {0} has been created", this.ApplicationDirectory);
            }
        }
    }
}
