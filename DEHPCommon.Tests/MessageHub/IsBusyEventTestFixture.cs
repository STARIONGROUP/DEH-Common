// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsBusyEventTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.MessageHub
{
    using DEHPCommon.MessageHub.Events;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="IsBusyEventTestFixture"/> class.
    /// </summary>
    [TestFixture]
    public class IsBusyEventTestFixture
    {
        private IsBusyEvent isBusyEvent;
        private bool isBussy;
        private string message;

        [SetUp]
        public void SetUp()
        {
            this.isBussy = true;
            this.message = "Is Busy Event";
        }

        [Test]
        public void VerifyThatSessionArePopulated()
        {
            this.isBusyEvent = new IsBusyEvent(this.isBussy,this.message);

            Assert.That(this.isBusyEvent.IsBusy, Is.EqualTo(this.isBussy));
            Assert.That(this.isBusyEvent.Message, Is.EqualTo(this.message));
        }
    }
}
