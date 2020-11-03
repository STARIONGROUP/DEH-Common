// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserViewModelBase.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2020 RHEA System S.A.
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
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using CDP4Dal;
    using CDP4Dal.Events;
    using CDP4Dal.Operations;

    using DEHPCommon.Enumerators;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// The View-Model-base that shall be used by a view-model representing a Browser
    /// </summary>
    /// <typeparam name="T">The <see cref="Thing"/> the browser is associated to</typeparam>
    public abstract class BrowserViewModel<T> : BrowserViewModelBase, IBrowserViewModelBase<T> where T : Thing
    {
        /// <summary>
        /// Gets the <see cref="Thing"/> that is represented by the view-model
        /// </summary>
        public T Thing { get; private set; }

        /// <summary>
        /// The <see cref="CamelCaseToSpaceConverter"/> converter.
        /// </summary>
        private readonly CamelCaseToSpaceConverter camelCaseToSpaceConverter = new CamelCaseToSpaceConverter();
        
        /// <summary>
        /// Backing field for <see cref="SelectedThing"/>
        /// </summary>
        private IRowViewModelBase<Thing> selectedThing;

        /// <summary>
        /// Backing field for <see cref="FocusedRow"/>
        /// </summary>
        private IRowViewModelBase<Thing> focusedRow;

        /// <summary>
        /// Backing field for <see cref="Person"/> property
        /// </summary>
        private string person;

        /// <summary>
        /// The selected thing class kind string.
        /// </summary>
        private string selectedThingClassKindString;

        /// <summary>
        /// Backing field for the <see cref="Feedback"/> property
        /// </summary>
        private string feedback;

        /// <summary>
        /// Backing field for <see cref="HasUpdateStarted"/> property
        /// </summary>
        private bool hasUpdateStarted;

        /// <summary>
        /// Backing field for <see cref="CanWriteSelectedThing"/> property
        /// </summary>
        private bool canWriteSelectedThing;
        
        /// <summary>
        /// Backing Field for Caption
        /// </summary>
        private string caption;

        /// <summary>
        /// Backing Field For ToolTip
        /// </summary>
        private string tooltip;

        /// <summary>
        /// Backing field for <see cref="DomainOfExpertise"/>
        /// </summary>
        private string domainOfExpertise;

        /// <summary>
        /// Gets or sets a value indicating whether it is possible to write on the <see cref="SelectedThing"/>
        /// </summary>
        public bool CanWriteSelectedThing
        {
            get => this.canWriteSelectedThing;
            set => this.RaiseAndSetIfChanged(ref this.canWriteSelectedThing, value);
        }

        /// <summary>
        /// Gets the current <see cref="DomainOfExpertise"/> name
        /// </summary>
        public string DomainOfExpertise
        {
            get => this.domainOfExpertise;
            protected set => this.RaiseAndSetIfChanged(ref this.domainOfExpertise, value);
        }

        /// <summary>
        /// Gets a value indicating whether the browser is dirty
        /// </summary>
        /// <remarks>
        /// May be overriden to show a confirmation on close
        /// </remarks>
        public virtual bool IsDirty => false;

        /// <summary>
        /// Gets or sets the Inspect Command
        /// </summary>
        public ReactiveCommand<object> RefreshCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the Inspect Command
        /// </summary>
        public ReactiveCommand<object> ExportCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the Inspect Command
        /// </summary>
        public ReactiveCommand<object> HelpCommand { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> that changes the focus of a grid
        /// </summary>
        public ReactiveCommand<object> ChangeFocusCommand { get; private set; }

        /// <summary>
        /// Gets the Expand Rows Command
        /// </summary>
        public ReactiveCommand<object> ExpandRowsCommand { get; private set; }

        /// <summary>
        /// Gets the Expand Rows Command
        /// </summary>
        public ReactiveCommand<object> CollpaseRowsCommand { get; private set; }

        /// <summary>
        /// Gets the Context Menu for this browser
        /// </summary>
        public ReactiveList<ContextMenuItemViewModel> ContextMenu { get; private set; }

        /// <summary>
        /// Gets the "create" <see cref="ContextMenuItemViewModel"/>
        /// </summary>
        public ReactiveList<ContextMenuItemViewModel> CreateContextMenu { get; private set; }

        /// <summary>
        /// Gets the data-source
        /// </summary>
        public string DataSource => this.Thing.IDalUri.ToString();

        /// <summary>
        /// Gets or sets the name of the active <see cref="Person"/>
        /// </summary>
        public string Person
        {
            get => this.person;
            set => this.RaiseAndSetIfChanged(ref this.person, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether an update is occurring
        /// </summary>
        public bool HasUpdateStarted
        {
            get => this.hasUpdateStarted;
            protected set => this.RaiseAndSetIfChanged(ref this.hasUpdateStarted, value);
        }

        /// <summary>
        /// Gets or sets the selected row that represents a <see cref="Thing"/>
        /// </summary>
        public IRowViewModelBase<Thing> SelectedThing
        {
            get => this.selectedThing;
            set => this.RaiseAndSetIfChanged(ref this.selectedThing, value);
        }

        /// <summary>
        /// Gets the selected rows
        /// </summary>
        public ReactiveList<IRowViewModelBase<Thing>> SelectedRows { get; protected set; }

        /// <summary>
        /// Gets or sets the focused row that represents a <see cref="Thing"/>
        /// </summary>
        public IRowViewModelBase<Thing> FocusedRow
        {
            get => this.focusedRow;
            set => this.RaiseAndSetIfChanged(ref this.focusedRow, value);
        }

        /// <summary>
        /// Gets or sets the type of the selected row
        /// </summary>
        public string SelectedThingClassKindString
        {
            get => this.selectedThingClassKindString;
            set => this.RaiseAndSetIfChanged(ref this.selectedThingClassKindString, value);
        }

        /// <summary>
        /// Gets or sets the feedback
        /// </summary>
        public string Feedback
        {
            get => this.feedback;
            set => this.RaiseAndSetIfChanged(ref this.feedback, value);
        }

        /// <summary>
        /// Gets the unique identifier of the view-model
        /// </summary>
        public Guid Identifier { get; private set; }

        /// <summary>
        /// Gets or sets the Caption
        /// </summary>
        public string Caption
        {
            get => this.caption;
            protected set => this.RaiseAndSetIfChanged(ref this.caption, value);
        }

        /// <summary>
        /// Gets or sets the ToolTip of the control
        /// </summary>
        public string ToolTip
        {
            get => this.tooltip;
            protected set => this.RaiseAndSetIfChanged(ref this.tooltip, value);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserViewModel{T}"/> class
        /// </summary>
        /// <param name="thing"> The <see cref="Thing"/> that contains the data to browse. </param>
        /// <param name="session"> The <see cref="ISession"/> that manages the current view-model. </param>
        protected BrowserViewModel(T thing, ISession session) : base(session)
        {
            this.Thing = thing;
            this.SetProperties();

            this.WhenAnyValue(vm => vm.SelectedThing).Subscribe(_ =>
            {
                this.OnSelectedThingChanged();
                this.ComputePermission();
                this.PopulateContextMenu();
            });

            var activePerson = this.Session.ActivePerson;
            this.Person = (activePerson == null) ? string.Empty : this.Session.ActivePerson.Name;

            if (activePerson != null)
            {
                var personSubscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.Session.ActivePerson)
                    .Where(
                        objectChange =>
                            objectChange.EventKind == EventKind.Updated &&
                            objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        _ =>
                        {
                            this.Person = this.Session.ActivePerson.Name;
                        });

                this.Disposables.Add(personSubscription);
            }

            var thingSubscription = CDPMessageBus.Current.Listen<ObjectChangedEvent>(this.Thing)
                .Where(objectChange => objectChange.EventKind == EventKind.Updated && objectChange.ChangedThing.RevisionNumber > this.RevisionNumber)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(this.ObjectChangeEventHandler);

            this.Disposables.Add(thingSubscription);
        }

        /// <summary>
        /// Sets the properties of this view model
        /// </summary>
        private void SetProperties()
        {
            this.ContextMenu = new ReactiveList<ContextMenuItemViewModel>();
            this.CreateContextMenu = new ReactiveList<ContextMenuItemViewModel>();
            this.SelectedRows = new ReactiveList<IRowViewModelBase<Thing>>();

            this.Identifier = Guid.NewGuid();
            
            this.RevisionNumber = this.Thing.RevisionNumber;
            this.IDalUri = this.Thing.IDalUri;

            var defaultThingClassKind = this.GetDefaultThingClassKind();
            this.SelectedThingClassKindString = this.camelCaseToSpaceConverter.Convert(defaultThingClassKind, null, null, null)?.ToString();
        }

        /// <summary>
        /// Execute the <see cref="RefreshCommand"/>
        /// </summary>
        protected virtual async void ExecuteRefreshCommand()
        {
            try
            {
                await this.Session.Refresh();
            }
            catch (Exception e)
            {
                this.Logger.Error(e, "The refresh operation failed: {0}");
            }
        }

        /// <summary>
        /// Execute the <see cref="ExportCommand"/>
        /// </summary>
        protected virtual void ExecuteExportCommand()
        {
            this.Logger.Info("Export Command called");
        }

        /// <summary>
        /// Execute the <see cref="HelpCommand"/>
        /// </summary>
        protected virtual void ExecuteHelpCommand()
        {
            this.Logger.Info("Help Command called");
        }
        
        /// <summary>
        /// Gets a value indicating whether it is possible to Delete the Selected Thing />
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> that needs to be checked</param>
        /// <returns>True if delete is allowed, otherwise false</returns>
        protected virtual bool IsDeleteAllowed(Thing thing) => true;

        /// <summary>
        /// Write the inline operations to the Data-access-layer
        /// </summary>
        /// <param name="transaction">The <see cref="ThingTransaction"/> that contains the operations</param>
        protected async Task DalWrite(ThingTransaction transaction)
        {
            try
            {
                this.IsBusy = true;
                var operationContainer = transaction.FinalizeTransaction();
                await this.Session.Write(operationContainer);
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "The inline update operation failed");
                this.Feedback = exception.Message;
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Gets the default thing class kind string to use before a row is selected
        /// </summary>
        /// <returns>
        /// The SelectedThingClassKindString <see cref="string"/>.
        /// </returns>
        private string GetDefaultThingClassKind()
        {
            var defaulClassKindString = this.ToString().Replace("BrowserViewModel", string.Empty);
            return defaulClassKindString.Substring(defaulClassKindString.LastIndexOf('.') + 1);
        }

        /// <summary>
        /// Handles the action of a user selecting different row in the browser
        /// </summary>
        private void OnSelectedThingChanged()
        {
            var thing = this.SelectedThing?.Thing;

            if (thing == null)
            {
                return;
            }

            this.SelectedThingClassKindString = thing.ClassKind == ClassKind.NotThing
                ? string.Empty
                : this.camelCaseToSpaceConverter.Convert(thing.ClassKind, null, null, null)?.ToString();
        }

        /// <summary>
        /// Handles the <see cref="SessionEvent"/> message
        /// </summary>
        /// <param name="sessionEvent">
        /// The <see cref="SessionEvent"/>
        /// </param>
        protected virtual void OnAssemblerUpdate(SessionEvent sessionEvent)
        {
            this.HasUpdateStarted = sessionEvent.Status == SessionStatus.BeginUpdate;
        }

        /// <summary>
        /// Executes the <see cref="ChangeFocusCommand"/>
        /// </summary>
        protected virtual void ExecuteChangeFocusCommand()
        {
        }

        /// <summary>
        /// Executes the expand rows logic
        /// </summary>
        private void ExecuteExpandRows()
        {
            this.SelectedThing?.ExpandAllRows();
        }

        /// <summary>
        /// Executes the collapse rows logic
        /// </summary>
        private void ExecuteCollapseRows()
        {
            this.SelectedThing?.CollapseAllRows();
        }

        /// <summary>
        /// Initializes the Commands that can be executed from this view model. The commands are initialized
        /// before the <see cref="PopulateContextMenu"/> is invoked
        /// </summary>
        protected virtual void InitializeCommands()
        {
            var sessionEventListener = CDPMessageBus.Current.Listen<SessionEvent>()
                .Where(sessionEvent => sessionEvent.Session == this.Session && (sessionEvent.Status == SessionStatus.BeginUpdate || sessionEvent.Status == SessionStatus.EndUpdate))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(this.OnAssemblerUpdate);

            this.Disposables.Add(sessionEventListener);

            this.RefreshCommand = ReactiveCommand.Create();
            this.RefreshCommand.Subscribe(_ => this.ExecuteRefreshCommand());

            this.ExportCommand = ReactiveCommand.Create();
            this.ExportCommand.Subscribe(_ => this.ExecuteExportCommand());

            this.HelpCommand = ReactiveCommand.Create();
            this.HelpCommand.Subscribe(_ => this.ExecuteHelpCommand());

            this.ChangeFocusCommand = ReactiveCommand.Create();
            this.ChangeFocusCommand.Subscribe(_ => this.ExecuteChangeFocusCommand());

            this.ExpandRowsCommand = ReactiveCommand.Create();
            this.ExpandRowsCommand.Subscribe(_ => this.ExecuteExpandRows());

            this.CollpaseRowsCommand = ReactiveCommand.Create();
            this.CollpaseRowsCommand.Subscribe(_ => this.ExecuteCollapseRows());

            var iteration = this.Thing as Iteration ?? this.Thing.GetContainerOfType<Iteration>();
            
            if (iteration != null)
            {
                var domainSwitchSubscription = CDPMessageBus.Current.Listen<DomainChangedEvent>()
                    .Where(x => x.Iteration.Iid == iteration.Iid)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(this.UpdateDomain);
                
                this.Disposables.Add(domainSwitchSubscription);
            }
        }

        /// <summary>
        /// Populate the <see cref="ContextMenu"/>
        /// </summary>
        public virtual void PopulateContextMenu()
        {
            this.ContextMenu.Clear();
            
            if (this.SelectedThing == null)
            {
                return;
            }

            if (this.SelectedThing != null && this.SelectedThing.ContainedRows.Count > 0)
            {
                this.ContextMenu.Add(this.SelectedThing.IsExpanded 
                    ? new ContextMenuItemViewModel("Collapse Rows", "", this.CollpaseRowsCommand, MenuItemKind.None, ClassKind.NotThing) 
                    : new ContextMenuItemViewModel("Expand Rows", "", this.ExpandRowsCommand, MenuItemKind.None, ClassKind.NotThing));
            }
        }
        
        /// <summary>
        /// Handles the <see cref="DomainChangedEvent"/>
        /// </summary>
        /// <param name="domainChangeEvent">The <see cref="DomainChangedEvent"/></param>
        protected virtual void UpdateDomain(DomainChangedEvent domainChangeEvent)
        {
            this.DomainOfExpertise = domainChangeEvent.SelectedDomain == null ? "None" : $"{domainChangeEvent.SelectedDomain.Name} [{domainChangeEvent.SelectedDomain.ShortName}]";
        }

        /// <summary>
        /// Compute the permissions for the current user
        /// </summary>
        public virtual void ComputePermission()
        {
            if (this.SelectedThing == null)
            {
                this.CanWriteSelectedThing = false;
                return;
            }

            this.CanWriteSelectedThing = this.PermissionService.CanWrite(this.SelectedThing.Thing);
        }
    }
}
