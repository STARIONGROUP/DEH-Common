// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreferenceServiceTestFixture.cs"company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.UserPreferenceHandler
{
    using System.IO;

    using DEHPCommon.UserPreferenceHandler;
    using DEHPCommon.UserPreferenceHandler.Enums;
    using DEHPCommon.UserPreferenceHandler.UserPreferenceService;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="UserPreferenceService"/> class.
    /// </summary>
    [TestFixture]
    public class UserPreferenceServiceTestFixture
    {
        private UserPreferenceService<UserPreference> userPreferenceService;
        private string expectedUserPreferencePath;
        private UserPreference userPreference;
        private ServerConnection serverConnection1;
        private ServerConnection serverConnection2;
        private ServerConnection serverConnection3;

        [SetUp]
        public void SetUp()
        {
            this.userPreferenceService = new UserPreferenceService<UserPreference>();

            this.expectedUserPreferencePath =
                Path.Combine(this.userPreferenceService.UserPreferenceDirectories);

            var fileName = $"{UserPreferenceService<UserPreference>.FILE_NAME}{UserPreferenceService<UserPreference>.SETTING_FILE_EXTENSION}";

            this.expectedUserPreferencePath = Path.Combine(this.expectedUserPreferencePath, $"{fileName}");

            this.serverConnection1 = new ServerConnection()
            {
                ServerType = ServerType.Cdp4WebServices,
                Uri = "http://localhost:5000",
            };

            this.serverConnection2 = new ServerConnection()
            {
                ServerType = ServerType.OcdtWspServer,
                Uri = "http://localhost:4000",
            };

            this.userPreference = new UserPreference();
            this.userPreference.SavedServerConections.Add(this.serverConnection1);
            this.userPreference.SavedServerConections.Add(this.serverConnection2);

            this.serverConnection3 = new ServerConnection()
            {
                ServerType = ServerType.Cdp4WebServices,
                Uri = "http://localhost:3000",
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(this.expectedUserPreferencePath))
            {
                File.Delete(this.expectedUserPreferencePath);
            }
        }

        [Test]
        public void Verify_that_the_UserPreference_settings_can_be_written_to_disk()
        {
            Assert.DoesNotThrow(() => this.userPreferenceService.Save());

            this.userPreferenceService.Read();

            Assert.AreEqual(0, this.userPreferenceService.UserPreferenceSettings.SavedServerConections.Count);

            this.userPreferenceService.UserPreferenceSettings.SavedServerConections.Add(this.serverConnection1);
            this.userPreferenceService.UserPreferenceSettings.SavedServerConections.Add(this.serverConnection2);

            Assert.DoesNotThrow(() => this.userPreferenceService.Save());
            Assert.AreEqual(2, this.userPreferenceService.UserPreferenceSettings.SavedServerConections.Count);
        }

        [Test]
        public void Verify_that_the_UserPreference_settings_can_be_read_from_disk()
        {
            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "UserPreferenceHandler", "expectedUserPreference.json"), this.expectedUserPreferencePath);
            Assert.IsTrue(File.Exists(this.expectedUserPreferencePath));

            this.userPreferenceService.Read();
            Assert.AreEqual(this.userPreference.SavedServerConections[0].ServerType, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[0].ServerType);
            Assert.AreEqual(this.userPreference.SavedServerConections[0].Uri, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[0].Uri);
            Assert.AreEqual(this.userPreference.SavedServerConections[1].ServerType, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[1].ServerType);
            Assert.AreEqual(this.userPreference.SavedServerConections[1].Uri, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[1].Uri);
        }

        [Test]
        public void Verify_that_the_UserPreference_settings_file_will_be_created_if_not_exist_on_the_first_bootup()
        {
            this.userPreferenceService.Read();
            Assert.AreEqual(this.userPreferenceService.UserPreferenceSettings.SavedServerConections.Count, 0);
            Assert.AreEqual(this.userPreferenceService.UserPreferenceSettings.SavedServerConections.Count, 0);
        }

        [Test]
        public void Verify_that_settings_can_be_read_and_written_to_disk()
        {
            this.userPreferenceService.CheckConfigurationDirectory();

            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "UserPreferenceHandler", "expectedUserPreference.json"), this.expectedUserPreferencePath);
            Assert.IsTrue(File.Exists(this.expectedUserPreferencePath));

            this.userPreferenceService.Read();
            Assert.AreEqual(this.userPreference.SavedServerConections[0].ServerType, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[0].ServerType);
            Assert.AreEqual(this.userPreference.SavedServerConections[0].Uri, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[0].Uri);
            Assert.AreEqual(this.userPreference.SavedServerConections[1].ServerType, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[1].ServerType);
            Assert.AreEqual(this.userPreference.SavedServerConections[1].Uri, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[1].Uri);

            this.userPreferenceService.UserPreferenceSettings.SavedServerConections.Add(this.serverConnection3);
            this.userPreferenceService.Save();

            this.userPreferenceService.Read();
            Assert.AreEqual(this.serverConnection3.ServerType, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[2].ServerType);
            Assert.AreEqual(this.serverConnection3.Uri, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[2].Uri);
        }

        [Test]
        public void VerifyCheckConfigurationDirectory()
        {
            var configurationDirectory = Path.Combine(this.userPreferenceService.UserPreferenceDirectories);

            if (Directory.Exists(configurationDirectory))
            {
                Directory.Delete(configurationDirectory, true);
            }

            Assert.IsFalse(Directory.Exists(configurationDirectory));
            this.userPreferenceService.CheckConfigurationDirectory();
            Assert.IsTrue(Directory.Exists(configurationDirectory));
        }
    }
}
