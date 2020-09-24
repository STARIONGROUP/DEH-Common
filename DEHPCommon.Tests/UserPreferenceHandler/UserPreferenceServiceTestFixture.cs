// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreferenceServiceTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.UserPreferenceHandler
{
    using System;
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
            this.expectedUserPreferencePath =
                Path.Combine(
                    UserPreferenceService<UserPreference>.ApplicationExecutePath,
                    UserPreferenceService<UserPreference>.UserPreferenceDirectoryName,
                    "DEHPCommon.settings.json");

            this.userPreferenceService = new UserPreferenceService<UserPreference>();

            this.serverConnection1 = new ServerConnection()
            {
                ServerType = ServerType.Cdp4WebServices,
                Uri = "http://localhost:5000",
            };

            this.serverConnection2 = new ServerConnection()
            {
                ServerType = ServerType.OcdtWSPServer,
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
        public void Verify_that_on_write_ArgumentNullException_is_thrown()
        {
            Assert.Throws<ArgumentNullException>(() => this.userPreferenceService.Write(null));
        }

        [Test]
        public void Verify_that_the_UserPreference_settings_can_be_written_to_disk()
        {
            Assert.DoesNotThrow(() => this.userPreferenceService.Write(this.userPreference));
            var expectedUserPreferenceContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "UserPreferenceHandler", "expectedUserPreference.json"));
            var writtenContent = File.ReadAllText(this.expectedUserPreferencePath);
            Assert.AreEqual(expectedUserPreferenceContent, writtenContent);
        }

        [Test]
        public void Verify_that_the_UserPreference_settings_can_be_read_from_disk()
        {
            this.userPreferenceService.CheckConfigurationDirectory();
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
            this.userPreferenceService.Write(this.userPreferenceService.UserPreferenceSettings);

            this.userPreferenceService.Read();
            Assert.AreEqual(this.serverConnection3.ServerType, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[2].ServerType);
            Assert.AreEqual(this.serverConnection3.Uri, this.userPreferenceService.UserPreferenceSettings.SavedServerConections[2].Uri);
        }
    }
}
