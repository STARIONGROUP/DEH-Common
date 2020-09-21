// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreferenceService.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler.UserPreferenceService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using NLog;

    public class UserPreferenceService : IUserPreferenceService
    {
        /// <summary>
        /// The logger for the current class
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Path to special windows "AppData" folder 
        /// </summary>
        public static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Application configuration folder path.
        /// </summary>
        public static string ConfigurationDirectoryFolder = "";

        /// <summary>
        /// The setting file extension
        /// </summary>
        public const string SETTING_FILE_EXTENSION = ".settings.json";

        /// <summary>
        /// A dictionary used to store the user preference setting,
        /// </summary>
        private readonly Dictionary<string, UserPreference> userPreferenceSettings;

        /// <summary>
        /// Initializes a new instance of <see cref="UserPreferenceService"/>
        /// </summary>
        public UserPreferenceService()
        {
            this.userPreferenceSettings = new Dictionary<string, UserPreference>();
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

            var path = Path.Combine(this.ApplicationConfigurationDirectory, assemblyName);

            logger.Debug("Read user preference for {0} from {1}", assemblyName, path);

            try
            {
                using (var file = File.OpenText($"{path}{SETTING_FILE_EXTENSION}"))
                {
                    var serializer = new JsonSerializer();
                    result = (T)serializer.Deserialize(file, typeof(T));

                    // once the settings have been read from disk, add them to the cache for fast access
                    this.userPreferenceSettings.Add(assemblyName, result);

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
                throw new ArgumentNullException(nameof(userPreference), "The pluginSettings may not be null");
            }

            var assemblyName = this.QueryAssemblyTitle(userPreference.GetType());

            this.CheckConfigurationDirectory();

            var path = Path.Combine(this.ApplicationConfigurationDirectory, $"{assemblyName}{SETTING_FILE_EXTENSION}");

            logger.Debug("write user preference to for {0} to {1}", assemblyName, path);

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
        public string ApplicationConfigurationDirectory
        {
            get { return Path.Combine(AppDataFolder, ConfigurationDirectoryFolder); }
        }

        /// <summary>
        /// Checks for the existence of the <see cref="UserPreferenceService.ApplicationConfigurationDirectory"/>
        /// </summary>
        public void CheckConfigurationDirectory()
        {
            if (!Directory.Exists(this.ApplicationConfigurationDirectory))
            {
                logger.Debug("The user preference folder {0} does not yet exist", this.ApplicationConfigurationDirectory);
                Directory.CreateDirectory(this.ApplicationConfigurationDirectory);
                logger.Debug("The user preference folder {0} has been created", this.ApplicationConfigurationDirectory);
            }
        }
    }
}
