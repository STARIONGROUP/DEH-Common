// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHub.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MessageHub
{
    using System;
    using System.Reactive.Linq;

    using CDP4Dal;
    using CDP4Dal.Events;

    using DEHPCommon.MessageHub.Events;

    using ReactiveUI;

    /// <summary>
    /// Definition of the <see cref="MessageHub"/>
    /// </summary>
    public class MessageHub : ReactiveObject
    {
        /// <summary>
        /// Backing field for <see cref="HasSession"/>
        /// </summary>
        private ObservableAsPropertyHelper<bool> hasSession;

        /// <summary>
        /// Backing field for <see cref="IsBusy"/> property
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// Backing field for <see cref="LoadingMessage"/>
        /// </summary>
        private string loadingMessage;

        /// <summary>
        /// The subscription for the IsBusy status
        /// </summary>
        private IDisposable subscription;

        /// <summary>
        /// Gets or sets a value indicating whether the application is busy
        /// </summary>
        public bool IsBusy
        {
            get { return this.isBusy; }
            set { this.RaiseAndSetIfChanged(ref this.isBusy, value); }
        }

        /// <summary>
        /// Gets or sets the loading message
        /// </summary>
        public string LoadingMessage
        {
            get { return this.loadingMessage; }
            set { this.RaiseAndSetIfChanged(ref this.loadingMessage, value); }
        }

        /// <summary>
        /// Gets a value indicating whether there are open sessions
        /// </summary>
        public bool HasSession
        {
            get { return this.hasSession.Value; }
        }

        /// <summary>
        /// Gets a list of open <see cref="ISession"/>s
        /// </summary>
        public ReactiveList<ISession> OpenSessions { get; private set; }

        /// <summary>
        /// The event-handler that is invoked by the subscription that listens for updates
        /// on the <see cref="Session"/> that is being represented by the view-model
        /// </summary>
        /// <param name="sessionChange">
        /// The payload of the event that is being handled
        /// </param>
        private void SessionChangeEventHandler(SessionEvent sessionChange)
        {
            if (sessionChange.Status == SessionStatus.Open)
            {
                this.OpenSessions.Add(sessionChange.Session);
            }
            else if (sessionChange.Status == SessionStatus.Closed)
            {
                this.OpenSessions.Remove(sessionChange.Session);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHub"/> class.
        /// </summary>
        public MessageHub()
        {
            this.OpenSessions = new ReactiveList<ISession>();
            this.OpenSessions.ChangeTrackingEnabled = true;
            this.OpenSessions.CountChanged.Select(x => x != 0).ToProperty(this, x => x.HasSession, out this.hasSession);

            CDPMessageBus.Current.Listen<SessionEvent>().Subscribe(this.SessionChangeEventHandler);

            this.subscription = CDPMessageBus.Current.Listen<IsBusyEvent>()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    this.IsBusy = x.IsBusy;
                    this.LoadingMessage = x.Message;
                });
        }
    }
}
