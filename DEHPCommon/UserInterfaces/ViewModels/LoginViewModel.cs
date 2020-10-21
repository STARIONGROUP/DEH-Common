// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski.
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

namespace DEHPCommon.UserInterfaces.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    using CDP4Common.EngineeringModelData;

    using CDP4Dal.DAL;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.UserInterfaces.Behaviors;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Rows;
    using DEHPCommon.UserPreferenceHandler.Enums;

    using NLog;

    using ReactiveUI;

    /// <summary>
    /// The view-model for the Login that allows users to connect to different datasources
    /// </summary>
    public class LoginViewModel : ReactiveObject, ILoginViewModel
    {
        /// <summary>
        /// The <see cref="NLog"/> logger
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger(typeof(LoginViewModel));

        /// <summary>
        /// The <see cref="IHubController"/> instance
        /// </summary>
        private readonly IHubController hubController;

        /// <summary>
        /// Gets or sets datasource server type
        /// </summary>
        public Dictionary<ServerType, string> DataSourceList { get; } = new Dictionary<ServerType, string>()
        {
            { ServerType.Cdp4WebServices, "CDP4 WebServices"},
            { ServerType.OcdtWspServer, "OCDT WSP Server"}
        };

        /// <summary>
        /// Backing field for the <see cref="SelectedServerType"/> property
        /// </summary>
        private KeyValuePair<ServerType, string> selectedServerType;

        /// <summary>
        /// Gets or sets server serverType value
        /// </summary>
        public KeyValuePair<ServerType, string> SelectedServerType
        {
            get => this.selectedServerType;
            set => this.RaiseAndSetIfChanged(ref this.selectedServerType, value);
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
        /// Backing field for the <see cref="LoginSuccessfull"/> property
        /// </summary>
        private bool loginSuccessfull;

        /// <summary>
        /// Gets or sets login succesfully flag
        /// </summary>
        public bool LoginSuccessfull
        {
            get => this.loginSuccessfull;
            private set => this.RaiseAndSetIfChanged(ref this.loginSuccessfull, value);
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
        /// Gets or sets engineering models list
        /// </summary>
        public ReactiveList<EngineeringModelRowViewModel> EngineeringModels { get; set; } = new ReactiveList<EngineeringModelRowViewModel>() { ChangeTrackingEnabled = true };

        /// <summary>
        /// Backing field for <see cref="SelectedEngineeringModel"/>
        /// </summary>
        private EngineeringModelRowViewModel selectedEngineeringModel;
        
        /// <summary>
        /// Gets or sets the selected <see cref="EngineeringModel"/>
        /// </summary>
        public EngineeringModelRowViewModel SelectedEngineeringModel
        {
            get => this.selectedEngineeringModel;
            set => this.RaiseAndSetIfChanged(ref this.selectedEngineeringModel, value);
        }

        /// <summary>
        /// Gets or sets engineering models list
        /// </summary>
        public ReactiveList<IterationRowViewModel> Iterations { get; set; } = new ReactiveList<IterationRowViewModel>() { ChangeTrackingEnabled = true };

        /// <summary>
        /// Backing field for <see cref="SelectedIteration"/>
        /// </summary>
        private IterationRowViewModel selectedIteration;

        /// <summary>
        /// Gets or sets the selected <see cref="EngineeringModel"/>
        /// </summary>
        public IterationRowViewModel SelectedIteration
        {
            get => this.selectedIteration;
            set => this.RaiseAndSetIfChanged(ref this.selectedIteration, value);
        }
        
        /// <summary>
        /// Gets or sets engineering models list
        /// </summary>
        public ReactiveList<DomainOfExpertiseRowViewModel> DomainsOfExpertise { get; set; } = new ReactiveList<DomainOfExpertiseRowViewModel>() { ChangeTrackingEnabled = true };

        /// <summary>
        /// Backing field for <see cref="SelectedDomainOfExpertise"/>
        /// </summary>
        private DomainOfExpertiseRowViewModel selectedDomainOfExpertise;

        /// <summary>
        /// Gets or sets the selected <see cref="EngineeringModel"/>
        /// </summary>
        public DomainOfExpertiseRowViewModel SelectedDomainOfExpertise
        {
            get => this.selectedDomainOfExpertise;
            set => this.RaiseAndSetIfChanged(ref this.selectedDomainOfExpertise, value);
        }

        /// <summary>
        /// Backing field for <see cref="LogMessage"/>
        /// </summary>
        private string logMessage;

        /// <summary>
        /// Gets or sets the log message
        /// </summary>
        public string LogMessage
        {
            get => this.logMessage;
            set => this.RaiseAndSetIfChanged(ref this.logMessage, $"{DateTime.Now:t}: {value}");
        }

        /// <summary>
        /// Gets the server login command
        /// </summary>
        public ReactiveCommand<Unit> LoginCommand { get; private set; }

        /// <summary>
        /// Gets the close command that closes the view when everything is setup
        /// </summary>
        public ReactiveCommand<Unit> CloseCommand { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="ICloseWindowBehavior"/> instance
        /// </summary>
        public ICloseWindowBehavior CloseWindowBehavior { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="hubController">The <see cref="IHubController"/></param>
        public LoginViewModel(IHubController hubController)
        {
            this.hubController = hubController;

            var canLogin = this.WhenAnyValue(
                vm => vm.SelectedServerType,
                vm => vm.UserName,
                vm => vm.Password,
                vm => vm.Uri,
                (serverType, username, password, uri) =>
                    !string.IsNullOrEmpty(serverType.Value) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) &&
                    !string.IsNullOrEmpty(uri));

            this.WhenAnyValue(x => x.SelectedEngineeringModel).Where(x => x != null).Subscribe(_ => this.PopulateIterations());
            this.WhenAnyValue(x => x.SelectedIteration).Where(x => x != null).Subscribe(_ => this.PopulateDomainOfExpertise());
            
            var canClose = this.WhenAnyValue(
                x => x.LoginSuccessfull, 
                x => x.SelectedIteration,
                x => x.SelectedEngineeringModel,
                x => x.SelectedDomainOfExpertise,
                (loginSuccess, iteration, engineeringModel, domain) =>
                    loginSuccess && iteration != null && engineeringModel != null && domain != null).Where(x => x is true);
            
            this.LoginCommand = ReactiveCommand.CreateAsyncTask(canLogin, async _ => await this.ExecuteLogin());
            this.CloseCommand = ReactiveCommand.CreateAsyncTask(canClose, async _ => await this.CloseCommandExecute());

            this.LoginSuccessfull = false;
            this.LoginFailed = false;
        }

        /// <summary>
        /// Executes the <see cref="CloseCommand"/>
        /// </summary>
        private async Task CloseCommandExecute()
        {
            try
            {
                var model = new EngineeringModel(this.SelectedEngineeringModel.Thing.EngineeringModelIid, this.hubController.Session.Assembler.Cache, this.hubController.Session.Credentials.Uri)
                {
                    EngineeringModelSetup = this.SelectedEngineeringModel.Thing
                };

                var iteration = new Iteration(this.SelectedIteration.Thing.IterationIid, this.hubController.Session.Assembler.Cache, this.hubController.Session.Credentials.Uri);

                model.Iteration.Add(iteration);
                await this.hubController.GetIteration(iteration, this.SelectedDomainOfExpertise.Thing);
                this.CloseWindowBehavior?.Close();
            }
            catch (Exception exception)
            {
                this.logger.Error($"Loading Iteration failed: {exception}");
                this.LogMessage = $"Loading Iteration failed: {exception.Message}";
            }
        }

        /// <summary>
        /// Executes login command
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task ExecuteLogin()
        {
            this.LogMessage = "Loggin in...";
            this.LoginSuccessfull = false;
            this.LoginFailed = false;

            try
            {
                var credentials = new Credentials(this.UserName, this.Password, new Uri(this.Uri));
                this.LoginSuccessfull = await this.hubController.Open(credentials, this.SelectedServerType.Key);
                this.PopulateEngineeringModels();
                this.LogMessage = "Loggin successful";
            }
            catch (Exception exception)
            {
                this.LoginFailed = true;
                this.logger.Error($"Loggin failed: {exception}");
                this.LogMessage = $"Loggin failed: {exception.Message}";
            }
        }
        
        /// <summary>
        /// Populates the <see cref="Iterations"/> collection
        /// </summary>
        private void PopulateDomainOfExpertise()
        {
            this.DomainsOfExpertise.Clear();


            var activeParticipant = this.SelectedEngineeringModel.Thing.Participant.Single(x => x.Person == this.hubController.Session.ActivePerson);
            
            if (activeParticipant.Domain.Count != 0)
            {
                this.DomainsOfExpertise.AddRange(activeParticipant.Domain.OrderBy(x => x.Name).Select(x => new DomainOfExpertiseRowViewModel(x)));

                this.SelectedDomainOfExpertise = this.DomainsOfExpertise.Any(x => x.Thing == activeParticipant.Person.DefaultDomain)
                    ? new DomainOfExpertiseRowViewModel(activeParticipant.Person.DefaultDomain)
                    : this.DomainsOfExpertise.First();
            }
        }

        /// <summary>
        /// Populates the <see cref="Iterations"/> collection
        /// </summary>
        private void PopulateIterations()
        {
            this.Iterations.Clear();
            this.Iterations.AddRange(this.SelectedEngineeringModel.Thing.IterationSetup.OrderBy(x => x.IterationNumber).Select(x => new IterationRowViewModel(x)));
        }

        /// <summary>
        /// Populates engineering models to the reactive list
        /// </summary>
        private void PopulateEngineeringModels()
        {
            this.EngineeringModels.Clear();
            
            this.EngineeringModels.AddRange(
                this.hubController.GetEngineeringModels()
                    .OrderBy(m => m.Name)
                    .Select(x => new EngineeringModelRowViewModel(x)));
        }
    }
}
