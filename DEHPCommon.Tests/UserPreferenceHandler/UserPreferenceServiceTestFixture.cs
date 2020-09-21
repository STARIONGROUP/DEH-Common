// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginSettingsServiceTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.UserPreferenceHandler
{
    using System;
    using System.IO;

    using DEHPCommon.UserPreferenceHandler.UserPreferenceService;

    using Newtonsoft.Json;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="UserPreferenceService"/> class.
    /// </summary>
    [TestFixture]
    public class PluginSettingsServiceTestFixture
    {
        private UserPreferenceService userPreferenceService;
        private string expectedUserPreferencePath;
        private TestUserPreference userPreference;

        [SetUp]
        public void SetUp()
        {
            this.expectedUserPreferencePath = 
                Path.Combine(
                    UserPreferenceService.AppDataFolder,
                    UserPreferenceService.ConfigurationDirectoryFolder,
                    "DEHPCommon.Tests.settings.json");
            
            this.userPreferenceService = new UserPreferenceService();

            this.userPreference = new TestUserPreference()
            {
                Identifier = Guid.Parse("78d90eda-bc57-45fe-8bfa-b9ca23130a00"),
                Description = "This is a description"
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
            Assert.Throws<ArgumentNullException>(() => this.userPreferenceService.Write<TestUserPreference>(null));
        }

        [Test]
        public void Verify_that_the_settings_can_be_written_to_disk()
        {
            Assert.DoesNotThrow(() => this.userPreferenceService.Write(this.userPreference));
            var test = Path.Combine(TestContext.CurrentContext.TestDirectory);
            File.Copy(this.expectedUserPreferencePath, test);
            
            var expectedUserPreferenceContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "UserPreferenceService", "DEHPCommon.Tests.settings.json"));
            var writtenContent = File.ReadAllText(this.expectedUserPreferencePath);
            Assert.AreEqual(expectedUserPreferenceContent, writtenContent);
        }

        [Test]
        public void Verify_that_the_settings_can_be_read_from_disk()
        {
            this.userPreferenceService.CheckConfigurationDirectory();

            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "PluginSettingService", "expectedSettings.settings.json"), this.expectedUserPreferencePath);
            Assert.IsTrue(File.Exists(this.expectedUserPreferencePath));

            var readSettings = this.userPreferenceService.Read<TestUserPreference>();
            Assert.AreEqual(this.userPreference.Identifier, readSettings.Identifier);
            Assert.AreEqual(this.userPreference.Description, readSettings.Description);
        }

        [Test]
        public void Verify_that_settings_can_be_read_and_written_to_disk()
        {
            this.userPreferenceService.CheckConfigurationDirectory();

            File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "PluginSettingService", "expectedSettings.settings.json"), this.expectedUserPreferencePath);
            Assert.IsTrue(File.Exists(this.expectedUserPreferencePath));

            var readSettings = this.userPreferenceService.Read<TestUserPreference>();
            Assert.AreEqual(this.userPreference.Identifier, readSettings.Identifier);
            Assert.AreEqual(this.userPreference.Description, readSettings.Description);

            var id = Guid.NewGuid();
            var description = "this is a new description";

            readSettings.Identifier = id;
            readSettings.Description = description;

            this.userPreferenceService.Write(readSettings);

            var newSettings = this.userPreferenceService.Read<TestUserPreference>();

            Assert.AreEqual(id, newSettings.Identifier);
            Assert.AreEqual(description, newSettings.Description);
        }
    }
}