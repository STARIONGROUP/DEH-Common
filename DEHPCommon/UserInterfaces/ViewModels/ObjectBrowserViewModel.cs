// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectBrowserViewModel.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;

    using System.Windows.Input;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using DEHPCommon.Enumerators;
    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.Services.ObjectBrowserTreeSelectorService;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="ObjectBrowserViewModel"/> is a View Model that is responsible for managing the data and interactions with that data for a view
    /// that shows all the <see cref="Thing"/>s contained by a data-source following the containment tree that is modelled in 10-25 and the CDP4 extensions.
    /// </summary>
    public class ObjectBrowserViewModel : ReactiveObject, IObjectBrowserViewModel, IDisposable, IHaveContextMenuViewModel
    {
        /// <summary>
        /// The <see cref="IHubController"/>
        /// </summary>
        protected readonly IHubController HubController;

        /// <summary>
        /// The <see cref="IObjectBrowserTreeSelectorService"/>
        /// </summary>
        private readonly IObjectBrowserTreeSelectorService objectBrowserTreeSelectorService;

        /// <summary>
        /// Backing field for <see cref="IsBusy"/>
        /// </summary>
        private bool? isBusy;

        /// <summary>
        /// Gets or sets a value indicating whether the browser is busy
        /// </summary>
        public bool? IsBusy
        {
            get => this.isBusy;
            set => this.RaiseAndSetIfChanged(ref this.isBusy, value);
        }

        /// <summary>
        /// Backing field for the <see cref="SelectedThing"/>
        /// </summary>
        private object selectedThing;

        /// <summary>
        /// Gets or sets the selected thing
        /// </summary>
        public object SelectedThing
        {
            get => this.selectedThing;
            set => this.RaiseAndSetIfChanged(ref this.selectedThing, value);
        }

        /// <summary>
        /// Gets or sets the selected things collection
        /// </summary>
        public ReactiveList<object> SelectedThings { get; set; } = new ReactiveList<object>();

        /// <summary>
        /// Gets the collection of <see cref="IRowViewModelBase{T}"/> to be displayed in the tree
        /// </summary>
        public ReactiveList<BrowserViewModelBase> Things { get; } = new ReactiveList<BrowserViewModelBase>();
        
        /// <summary>
        /// Gets the Context Menu for the implementing view model
        /// </summary>
        public ReactiveList<ContextMenuItemViewModel> ContextMenu { get; } = new ReactiveList<ContextMenuItemViewModel>();

        /// <summary>
        /// Gets the command that allows to map the selected things
        /// </summary>
        public ReactiveCommand<object> MapCommand { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IObservable{T}"/> of <see cref="bool"/> that is bound to the <see cref="MapCommand"/> <see cref="ReactiveCommand{T}.CanExecute"/> property
        /// </summary>
        /// <remarks>This observable is intended to be Merged with another observable</remarks>
        public IObservable<bool> CanMap { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBrowserViewModel"/> class.
        /// </summary>
        /// <param name="hubController">The <see cref="IHubController"/></param>
        /// <param name="objectBrowserTreeSelectorService">The <see cref="IObjectBrowserTreeSelectorService"/></param>
        public ObjectBrowserViewModel(IHubController hubController, IObjectBrowserTreeSelectorService objectBrowserTreeSelectorService)
        {
            this.HubController = hubController;
            this.objectBrowserTreeSelectorService = objectBrowserTreeSelectorService;
            this.Caption = "Hub Object Browser";
            this.InitializesCommandsAndObservables();
        }

        /// <summary>
        /// Initializes this view model <see cref="ICommand"/>s
        /// </summary>
        private void InitializesCommandsAndObservables()
        {
            this.WhenAnyValue(x => x.HubController.OpenIteration).ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    this.IsBusy = true;

                    if (this.HubController.IsSessionOpen && this.HubController.OpenIteration != null)
                    {
                        this.ToolTip = $"{this.HubController.Session.DataSourceUri}\n{this.HubController.Session.ActivePerson.Name}";
                        this.BuildTrees();
                    }
                    else
                    {
                        this.Things.Clear();
                    }

                    this.IsBusy = false;
                });
            
            this.CanMap = this.WhenAny(
                vm => vm.SelectedThing,
                vm => vm.SelectedThings.CountChanged,
                vm => vm.HubController.OpenIteration,
                (selected, selection, iteration) =>
                    iteration.Value != null && (selected.Value != null || this.SelectedThings.Any()));
        }
        
        /// <summary>
        /// Adds to the <see cref="Things"/> collection the specified by <see cref="IObjectBrowserTreeSelectorService"/> trees
        /// </summary>
        public virtual void BuildTrees()
        {
            foreach (var thingKind in this.objectBrowserTreeSelectorService.ThingKinds)
            {
                if (thingKind == typeof(ElementDefinition) && 
                    this.Things.OfType<IBrowserViewModelBase<Thing>>().All(x => x.Thing.Iid != this.HubController.OpenIteration.Iid))
                {
                    this.Things.Add(new ElementDefinitionsBrowserViewModel(this.HubController.OpenIteration, this.HubController.Session));
                }
            }
        }

        /// <summary>
        /// Populate the context menu for the implementing view model
        /// </summary>
        public void PopulateContextMenu()
        {
            this.ContextMenu.Clear();

            if (this.SelectedThing == null)
            {
                return;
            }

            this.ContextMenu.Add(new ContextMenuItemViewModel("Map selection", "", this.MapCommand,
                MenuItemKind.Export, ClassKind.NotThing));
        }

        /// <summary>
        /// Gets the Caption of the control
        /// </summary>
        public string Caption { get; private set; }
        
        /// <summary>
        /// Gets the tooltip of the control
        /// </summary>
        public string ToolTip { get; private set; }

        /// <summary>
        /// Gets the list of <see cref="IDisposable"/> objects that are referenced by this class
        /// </summary>
        protected IEnumerable<IDisposable> Disposables { get; } = new List<IDisposable>();

        /// <summary>
        /// Dispose of this <see cref="ObjectBrowserViewModel"/>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of this <see cref="ObjectBrowserViewModel"/>
        /// </summary>
        /// <param name="disposing">Assert whether to dispose of this.<see cref="IDisposable"/></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            foreach (var disposable in this.Disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
