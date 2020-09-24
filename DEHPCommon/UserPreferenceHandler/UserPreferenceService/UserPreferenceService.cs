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
    using System.Linq;
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
        /// The path of the user preference directory storage
        /// </summary>
        public static string ApplicationExecutePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location);

        /// <summary>
        /// The name of folder where user preference is storage.
        /// </summary>
        public static string UserPreferenceDirectoryName = "UserPreferenceService";

        /// <summary>
        /// The setting file extension
        /// </summary>
        public const string SETTING_FILE_EXTENSION = ".settings.json";

        /// <summary>
        /// Gets or sets  user preference settings,
        /// </summary>
        public UserPreference UserPreferenceSettings { get; set; }

        /// <summary>
        /// Configuration user preference file path
        /// </summary>
        public string UserPreferenceDirectoryPath
        {
            get { return Path.Combine(ApplicationExecutePath, UserPreferenceDirectoryName); }
        }
        /// <summary>
        /// Initializes a new instance of <see cref="UserPreferenceService"/>
        /// </summary>
        public UserPreferenceService()
        {
            this.UserPreferenceSettings = new UserPreference();
        }

        /// <summary>
        /// Reads the <see cref="UserPreference"/> settings
        /// </summary>
        public void Read()
        {
            var assemblyName = this.QueryAssemblyTitle(typeof(UserPreference));

            this.CheckConfigurationDirectory();

            var path = Path.Combine(this.UserPreferenceDirectoryPath, assemblyName);

            logger.Debug("Read user preference for {0} from {1}", assemblyName, path);

            try
            {
                var fileExist = File.Exists($"{path}{SETTING_FILE_EXTENSION}");

                if (!fileExist)
                {
                    this.Write(this.UserPreferenceSettings);
                }

                var file = File.ReadAllText($"{path}{SETTING_FILE_EXTENSION}");
                this.UserPreferenceSettings = JsonConvert.DeserializeObject<UserPreference>(file);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "The user preference could not be read");

                throw new IOException("The user preference could not be read", ex);
            }
        }

        /// <summary>
        /// Writes the <see cref="UserPreference"/> to disk
        /// </summary>
        /// <param name="userPreference">
        /// The <see cref="UserPreference"/> that will be persisted
        /// </param>
        public void Write(UserPreference userPreference)
        {
            if (userPreference == null)
            {
                throw new ArgumentNullException(nameof(userPreference), "This may not be null");
            }

            var assemblyName = this.QueryAssemblyTitle(userPreference.GetType());

            this.CheckConfigurationDirectory();

            var path = Path.Combine(this.UserPreferenceDirectoryPath, $"{assemblyName}{SETTING_FILE_EXTENSION}");

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

            this.UserPreferenceSettings = userPreference;
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
        /// Checks for the existence of the <see cref="UserPreferenceDirectoryPath"/>
        /// </summary>
        public void CheckConfigurationDirectory()
        {
            if (!Directory.Exists(this.UserPreferenceDirectoryPath))
            {
                logger.Debug("The user preference folder {0} does not yet exist", this.UserPreferenceDirectoryPath);
                Directory.CreateDirectory(this.UserPreferenceDirectoryPath);
                logger.Debug("The user preference folder {0} has been created", this.UserPreferenceDirectoryPath);
            }
        }
    }
}
