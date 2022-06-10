// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreferenceService.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// 
//    This file is part of DEHP Common Library
// 
//    The DEHPCommon is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The DEHPCommon is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler.UserPreferenceService
{
    using System;
    using System.IO;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using NLog;

    /// <summary>
    /// Definition of the <see cref="UserPreferenceService"/> used to load specific settings
    /// </summary>
    public class UserPreferenceService<T> : IUserPreferenceService<T> where T : new()
    {
        /// <summary>
        /// The logger for the current class
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The path of the user preference directory storage
        /// </summary>
        public readonly string[] UserPreferenceDirectories =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".rheagroup",
            "DEHAdapterSettingFile",
            "UserPreferences"
        };

        /// <summary>
        /// The setting file extension
        /// </summary>
        public const string SETTING_FILE_EXTENSION = ".settings.json";

        /// <summary>
        /// The name of the file
        /// </summary>
        public const string FILE_NAME = "UserPreference";

        /// <summary>
        /// Gets or sets  user preference settings,
        /// </summary>
        public T UserPreferenceSettings { get; set; }

        /// <summary>
        /// Configuration user preference file path
        /// </summary>
        public string UserPreferenceDirectoryPath => Path.Combine(this.UserPreferenceDirectories);

        /// <summary>
        /// Reads the <see cref="T"/> user preference in settings
        /// </summary>
        public void Read()
        {
            this.CheckConfigurationDirectory();

            var path = Path.Combine(this.UserPreferenceDirectoryPath, FILE_NAME);

            this.logger.Info($"Read user preference for {FILE_NAME} from {path}");

            try
            {
                var fileExist = File.Exists($"{path}{SETTING_FILE_EXTENSION}");

                if (!fileExist)
                {
                    this.Save();
                }

                var file = File.ReadAllText($"{path}{SETTING_FILE_EXTENSION}");
                this.UserPreferenceSettings = JsonConvert.DeserializeObject<T>(file);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "The user preference could not be read");

                throw new IOException("The user preference could not be read", ex);
            }
        }

        /// <summary>
        /// Save the <see cref="UserPreference"/> to disk
        /// </summary>
            public void Save()
        {
            if (this.UserPreferenceSettings == null)
            {
                this.UserPreferenceSettings = new T();
            }

            this.CheckConfigurationDirectory();

            var path = Path.Combine(this.UserPreferenceDirectoryPath, $"{FILE_NAME}{SETTING_FILE_EXTENSION}");

            this.logger.Info($"Write user preference to for {FILE_NAME} to {path}");

            using (var streamWriter = File.CreateText(path))
            {
                var serializer = new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = {new Newtonsoft.Json.Converters.StringEnumConverter()},
                    Formatting = Formatting.Indented
                };

                serializer.Serialize(streamWriter, this.UserPreferenceSettings);
            }
        }

        /// <summary>
        /// Checks for the existence of the <see cref="UserPreferenceDirectoryPath"/>
        /// </summary>
        public void CheckConfigurationDirectory()
        {
            if (!Directory.Exists(this.UserPreferenceDirectoryPath))
            {
                this.logger.Info($"The user preference folder {this.UserPreferenceDirectoryPath} does not yet exist");
                Directory.CreateDirectory(this.UserPreferenceDirectoryPath);
                this.logger.Info($"The user preference folder {this.UserPreferenceDirectoryPath} has been created");
            }
        }
    }
}
