// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHubTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.MessageHub
{
    using System.Reactive.Concurrency;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.MessageHub;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    /// <summary>
    /// Suite of tests for the <see cref="MessageHubTestFixture"/> class.
    /// </summary>
    public class MessageHubTestFixture
    {
        private MessageHub messageHub;
        private Mock<ISession> session;

        [SetUp]
        public void SetUp()
        {
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;

            this.messageHub = new MessageHub();
            this.session = new Mock<ISession>();
        }

        [TearDown]
        public void TearDown()
        {
            this.messageHub = null;
        }

        [Test]
        public void VerifyThatSessionArePopulated()
        {
            CDPMessageBus.Current.SendMessage<SessionEvent>(new SessionEvent(this.session.Object, SessionStatus.Open));
            Assert.AreEqual(1, this.messageHub.OpenSessions.Count);

            CDPMessageBus.Current.SendMessage<SessionEvent>(new SessionEvent(this.session.Object, SessionStatus.Closed));
            Assert.AreEqual(0, this.messageHub.OpenSessions.Count);
        }
    }
}
