// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferControlViewModel.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
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
    using System.Reactive;
    using System.Windows.Input;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using ReactiveUI;

    /// <summary>
    /// View model for dst adapters transfer/cancel button and progress bar 
    /// </summary>
    public abstract class TransferControlViewModel : ReactiveObject, ITransferControlViewModel
    {
        /// <summary>
        /// Backing field for <see cref="IsIndeterminate"/>
        /// </summary>
        private bool isIndeterminate;

        /// <summary>
        /// Gets or sets a value indicating whether the progress bar is in a indeterminate state
        /// </summary>
        public bool IsIndeterminate
        {
            get => this.isIndeterminate; 
            set => this.RaiseAndSetIfChanged(ref this.isIndeterminate, value);
        }
        
        /// <summary>
        /// Backing field for <see cref="Progress"/>
        /// </summary>
        private int progress;

        /// <summary>
        /// Gets or sets the current progress value displayed in the pogress bar
        /// </summary>
        public int Progress
        {
            get => this.progress;
            set => this.RaiseAndSetIfChanged(ref this.progress, value);
        }

        /// <summary>
        /// Backing field for <see cref="NumberOfThing"/>
        /// </summary>
        private int numberOfThing;

        /// <summary>
        /// Gets or sets the current number of item that will be transfered
        /// </summary>
        public int NumberOfThing
        {
            get => this.numberOfThing;
            set
            {
                this.RaiseAndSetIfChanged(ref this.numberOfThing, value);

                switch (this.numberOfThing)
                {
                    case > 0:
                        var thereIsMoreThanOneThing = this.numberOfThing > 1 ? "s" : string.Empty;
                        this.TransferButtonText = $"Transfer {this.NumberOfThing} thing{thereIsMoreThanOneThing}";
                        break;

                    default:
                        this.TransferButtonText = "Transfer";
                        break;
                }
            }
        }

        /// <summary>
        /// Backing field for <see cref="TransferButtonText"/>
        /// </summary>
        private string transferButtonText = "Transfer";

        /// <summary>
        /// Gets or sets the current number of item that will be transfered
        /// </summary>
        public string TransferButtonText
        {
            get => this.transferButtonText;
            set => this.RaiseAndSetIfChanged(ref this.transferButtonText, value);
        }
        
        /// <summary>
        /// Gets or sets the <see cref="ICommand"/> that triggers the transfer
        /// </summary>
        public ReactiveCommand<Unit> TransferCommand { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICommand"/> that cancels the transfer command
        /// </summary>
        public ReactiveCommand<Unit> CancelCommand { get; set; }
    }
}
