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
    using System.Linq;

    using DEHPCommon.UserPreferenceHandler;
    using DEHPCommon.UserPreferenceHandler.UserPreferenceService;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="UserPreferenceService"/> class.
    /// </summary>
    [TestFixture]
    public class UserPreferenceServiceTestFixture
    {
        private UserPreferenceService userPreferenceService;
        private string expectedUserPreferencePath;
        private UserPreference userPreference;
        private ServerConnection serverConnection1;
        private ServerConnection serverConnection2;
        private ServerConnection serverConnection3;

        private string serverType;
        private string uri;

        [SetUp]
        public void SetUp()
        {
            this.expectedUserPreferencePath =
                Path.Combine(
                    UserPreferenceService.UserPreferenceDataFolder,
                    UserPreferenceService.DirectoryFolder,
                    "DEHPCommon.settings.json");

            this.userPreferenceService = new UserPreferenceService();

            this.serverConnection1 = new ServerConnection()
            {
                ServerType = "CDP4 WebServices",
                Uri = "http://localhost:5000",
            };

            this.serverConnection2 = new ServerConnection()
            {
                ServerType = "OCDT WebServices",
                Uri = "http://localhost:4000",
            };

            this.userPreference = new UserPreference();
            this.userPreference.SavedServerConections.Add(this.serverConnection1);
            this.userPreference.SavedServerConections.Add(this.serverConnection2);

            this.serverConnection3 = new ServerConnection()
            {
                ServerType = this.serverType,
                Uri = this.uri,
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
            Assert.Throws<ArgumentNullException>(() => this.userPreferenceService.Write<UserPreference>(null));
        }

        [Test]
        public void Verify_that_the_settings_can_be_written_to_disk()
        {
            Assert.DoesNotThrow(() => this.userPreferenceService.Write(this.userPreference));
            var expectedUserPreferenceContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "UserPreferenceHandler", "expectedUserPreference.json"));
            var writtenContent = File.ReadAllText(this.expectedUserPreferencePath);
            Assert.AreEqual(expectedUserPreferenceContent, writtenContent);
        }

        [Test]
        public void Verify_that_the_settings_can_be_read_from_disk()
        {
            this.userPreferenceService.CheckConfigurationDirectory();

            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "UserPreferenceHandler", "expectedUserPreference.json"), this.expectedUserPreferencePath);
            Assert.IsTrue(File.Exists(this.expectedUserPreferencePath));

            var readUserPreference = this.userPreferenceService.Read<UserPreference>();
            Assert.AreEqual(this.userPreference.SavedServerConections[0].ServerType, readUserPreference.SavedServerConections[0].ServerType);
            Assert.AreEqual(this.userPreference.SavedServerConections[0].Uri, readUserPreference.SavedServerConections[0].Uri);
            Assert.AreEqual(this.userPreference.SavedServerConections[1].ServerType, readUserPreference.SavedServerConections[1].ServerType);
            Assert.AreEqual(this.userPreference.SavedServerConections[1].Uri, readUserPreference.SavedServerConections[1].Uri);

        }

        [Test]
        public void Verify_that_settings_can_be_read_and_written_to_disk()
        {
            this.userPreferenceService.CheckConfigurationDirectory();

            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "UserPreferenceHandler", "expectedUserPreference.json"), this.expectedUserPreferencePath);
            Assert.IsTrue(File.Exists(this.expectedUserPreferencePath));

            var readUserPreference = this.userPreferenceService.Read<UserPreference>();
            Assert.AreEqual(this.userPreference.SavedServerConections[0].ServerType, readUserPreference.SavedServerConections[0].ServerType);
            Assert.AreEqual(this.userPreference.SavedServerConections[0].Uri, readUserPreference.SavedServerConections[0].Uri);
            Assert.AreEqual(this.userPreference.SavedServerConections[1].ServerType, readUserPreference.SavedServerConections[1].ServerType);
            Assert.AreEqual(this.userPreference.SavedServerConections[1].Uri, readUserPreference.SavedServerConections[1].Uri);

            readUserPreference.SavedServerConections.Add(this.serverConnection3);
            this.userPreferenceService.Write(readUserPreference);

            var newUserPreference = this.userPreferenceService.Read<UserPreference>();
            Assert.AreEqual(this.serverType, newUserPreference.SavedServerConections[2].ServerType);
            Assert.AreEqual(this.uri, newUserPreference.SavedServerConections[2].Uri);
        }
    }
}
