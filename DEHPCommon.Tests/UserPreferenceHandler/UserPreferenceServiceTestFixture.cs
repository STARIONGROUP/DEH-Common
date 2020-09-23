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

        private string serverType;
        private string uri;
        private string userName;

        [SetUp]
        public void SetUp()
        {
            this.expectedUserPreferencePath = 
                Path.Combine(
                    UserPreferenceService.UserPreferenceDataFolder,
                    UserPreferenceService.ConfigurationDirectoryFolder,
                    "DEHPCommon.settings.json");
            
            this.userPreferenceService = new UserPreferenceService();

            this.userPreference = new UserPreference()
            {
                ServerType = "CDP4 WebServices",
                Uri = "http://localhost:5000",
                UserName = "DEHP-User"
            };

            this.serverType = "CDP4 WebServicesNew";
            this.uri = "http://localhost:4000";
            this.userName = "DEHP-UserNew";
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
            Assert.AreEqual(this.userPreference.ServerType, readUserPreference.ServerType);
            Assert.AreEqual(this.userPreference.Uri, readUserPreference.Uri);
            Assert.AreEqual(this.userPreference.UserName, readUserPreference.UserName);
        }

        [Test]
        public void Verify_that_settings_can_be_read_and_written_to_disk()
        {
            this.userPreferenceService.CheckConfigurationDirectory();

            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "UserPreferenceHandler", "expectedUserPreference.json"), this.expectedUserPreferencePath);
            Assert.IsTrue(File.Exists(this.expectedUserPreferencePath));

            var readUserPreference = this.userPreferenceService.Read<UserPreference>();
            Assert.AreEqual(this.userPreference.ServerType, readUserPreference.ServerType);
            Assert.AreEqual(this.userPreference.Uri, readUserPreference.Uri);
            Assert.AreEqual(this.userPreference.UserName, readUserPreference.UserName);

            readUserPreference.ServerType = this.serverType;
            readUserPreference.Uri = this.uri;
            readUserPreference.UserName = this.userName;

            this.userPreferenceService.Write(readUserPreference);
            var newUserPreference = this.userPreferenceService.Read<UserPreference>();
            Assert.AreEqual(this.serverType, newUserPreference.ServerType);
            Assert.AreEqual(this.uri, newUserPreference.Uri);
            Assert.AreEqual(this.userName, newUserPreference.UserName);
        }
    }
}
