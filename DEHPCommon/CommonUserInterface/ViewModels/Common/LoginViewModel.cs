// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.CommonUserInterface.ViewModels.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Threading.Tasks;

    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.DAL;

    using CDP4ServicesDal;

    using CDP4WspDal;

    using DEHPCommon.CommonUserInterface.ViewModels.Rows;

    using ReactiveUI;

    /// <summary>
    /// The view-model for the Login that allows users to connect to different datasources
    /// </summary>
    public class LoginViewModel : ReactiveObject
    {
        /// <summary>
        /// Gets or sets datasource server type
        /// </summary>
        public static KeyValuePair<string, string>[] DataSourceList { get; } = {
            new KeyValuePair<string, string>("CDP", "CDP4 WebServices"),
            new KeyValuePair<string, string>("OCDT", "OCDT WSP Server"),
        };

        /// <summary>
        /// Backing field for the <see cref="ServerType"/> property
        /// </summary>
        private KeyValuePair<string, string> serverType;

        /// <summary>
        /// Gets or sets server serverType value
        /// </summary>
        public KeyValuePair<string, string> ServerType
        {
            get => this.serverType;

            set => this.RaiseAndSetIfChanged(ref this.serverType, value);
        }

        /// <summary>
        /// Backing field for the <see cref="UserName"/> property
        /// </summary>
        private string username;

        /// <summary>
        /// Gets or sets server username value
        /// </summary>
        public string UserName
        {
            get => this.username;

            set => this.RaiseAndSetIfChanged(ref this.username, value);
        }

        /// <summary>
        /// Backing field for the <see cref="Password"/> property
        /// </summary>
        private string password;

        /// <summary>
        /// Gets or sets server password value
        /// </summary>
        public string Password
        {
            get => this.password;

            set => this.RaiseAndSetIfChanged(ref this.password, value);
        }

        /// <summary>
        /// Backing field for the <see cref="Uri"/> property
        /// </summary>
        private string uri;

        /// <summary>
        /// Gets or sets server uri
        /// </summary>
        public string Uri
        {
            get => this.uri;

            set => this.RaiseAndSetIfChanged(ref this.uri, value);
        }

        /// <summary>
        /// The backing field for available data access layer <see cref="IDal"/>
        /// </summary>
        private IDal dal;

        /// <summary>
        /// Gets the <see cref="Session"/> object that is encapsulated by the current <see cref="LoginViewModel"/>.
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        /// Backing field for the <see cref="LoginSuccessfully"/> property
        /// </summary>
        private bool loginSuccessfully;

        /// <summary>
        /// Gets or sets login succesfully flag
        /// </summary>
        public bool LoginSuccessfully
        {
            get => this.loginSuccessfully;

            private set => this.RaiseAndSetIfChanged(ref this.loginSuccessfully, value);
        }

        /// <summary>
        /// Backing field for the <see cref="LoginFailed"/> property
        /// </summary>
        private bool loginFailed;

        /// <summary>
        /// Gets or sets login failed flag
        /// </summary>
        public bool LoginFailed
        {
            get => this.loginFailed;

            private set => this.RaiseAndSetIfChanged(ref this.loginFailed, value);
        }

        /// <summary>
        /// Backing field for the <see cref="Output"/> property
        /// </summary>
        private string output;

        /// <summary>
        /// Gets or sets output panel log messages
        /// </summary>
        public string Output
        {
            get => this.output;

            set => this.RaiseAndSetIfChanged(ref this.output, value);
        }

        /// <summary>
        /// Backing field for the <see cref="EngineeringModels"/> property
        /// </summary>
        private ReactiveList<EngineeringModelRowViewModel> engineeringModels;

        /// <summary>
        /// Gets or sets engineering models list
        /// </summary>
        public ReactiveList<EngineeringModelRowViewModel> EngineeringModels
        {
            get => this.engineeringModels;
            private set => this.RaiseAndSetIfChanged(ref this.engineeringModels, value);
        }

        /// <summary>
        /// Gets the server login command
        /// </summary>
        public ReactiveCommand<Unit> LoginCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        public LoginViewModel(ISession session)
        {
            var canLogin = this.WhenAnyValue(
                vm => vm.ServerType,
                vm => vm.UserName,
                vm => vm.Password,
                vm => vm.Uri,
                (serverType, username, password, uri) =>
                    !string.IsNullOrEmpty(serverType.Value) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) &&
                    !string.IsNullOrEmpty(uri));

            this.WhenAnyValue(vm => vm.LoginFailed).Subscribe((loginFailed) =>
            {
                if (!loginFailed)
                {
                    return;
                }
            });

            this.WhenAnyValue(vm => vm.LoginSuccessfully).Subscribe(loginSuccessfully =>
            {
                if (!loginSuccessfully)
                {
                    return;
                }
            });

            this.LoginCommand = ReactiveCommand.CreateAsyncTask(canLogin, x => this.ExecuteLogin(session), RxApp.MainThreadScheduler);

            this.LoginSuccessfully = false;
            this.LoginFailed = false;
            this.EngineeringModels = new ReactiveList<EngineeringModelRowViewModel>();
            this.EngineeringModels.ChangeTrackingEnabled = true;
        }

        /// <summary>
        /// Executes login command
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task ExecuteLogin(ISession session)
        {
            this.LoginSuccessfully = false;
            this.LoginFailed = false;

            try
            {
                var credentials = new Credentials(this.UserName, this.Password, new Uri(this.Uri));

                switch (this.ServerType.Key)
                {
                    case "CDP":
                        this.dal = new CdpServicesDal();
                        break;
                    case "OCDT":
                        this.dal = new WspDal();
                        break;
                }

                this.Session = new Session(this.dal, credentials);
                await this.Session.Open();

                this.LoginSuccessfully = true;

                var siteDirectory = session.RetrieveSiteDirectory();
                this.BindEngineeringModels(siteDirectory);
            }
            catch (Exception ex)
            {
                this.LoginFailed = true;
            }
        }

        /// <summary>
        /// Bind engineering models to the reactive list
        /// </summary>
        /// <param name="siteDirectory">The <see cref="SiteDirectory"/> top container</param>
        private void BindEngineeringModels(SiteDirectory siteDirectory)
        {
            this.EngineeringModels.Clear();

            foreach (var modelSetup in siteDirectory.Model.OrderBy(m => m.Name))
            {
                this.EngineeringModels.Add(new EngineeringModelRowViewModel(modelSetup));
            }
        }
    }
}
