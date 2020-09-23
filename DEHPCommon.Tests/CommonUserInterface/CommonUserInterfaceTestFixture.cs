// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonUserInterfaceTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.CommonUserInterface
{
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="CommonUserInterfaceTestFixture"/> class.
    /// </summary>
    [TestFixture]
    public class CommonUserInterfaceTestFixture
    {
        private string serverType;
        private string uri;
        private string userName;

        [SetUp]
        public void SetUp()
        {

            this.serverType = "CDP4 WebServicesNew";
            this.uri = "http://localhost:4000";
            this.userName = "DEHP-UserNew";
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Verify_that_on_write_ArgumentNullException_is_thrown()
        {
            //Assert.Throws<ArgumentNullException>(() => this.userPreferenceService.Write<UserPreference>(null));
        }
    }
}
