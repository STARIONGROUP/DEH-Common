// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationRowViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.Rows
{
    using CDP4Common.SiteDirectoryData;

    using ReactiveUI;

    /// <summary>
    /// Row class representing a <see cref="IterationSetup"/> as a plain object
    /// </summary>
    public class IterationRowViewModel : ThingRowViewModel<IterationSetup>
    {
        /// <summary>
        /// Backing field for <see cref="Number"/>
        /// </summary>
        private string number;

        /// <summary>
        /// Backing field for <see cref="Description"/>
        /// </summary>
        private string description;

        /// <summary>
        /// Backing field for <see cref="FrozenOn"/>
        /// </summary>
        private string frozenOn;

        /// <summary>
        /// Gets or sets the iteration number
        /// </summary>
        public string Number
        {
            get => this.number;
            private set => this.RaiseAndSetIfChanged(ref this.number, value);
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description
        {
            get => this.description;
            private set => this.RaiseAndSetIfChanged(ref this.description, value);
        }
        
        /// <summary>
        /// Gets or sets the datetime when the iteration has been frozen
        /// </summary>
        public string FrozenOn
        {
            get => this.frozenOn;
            private set => this.RaiseAndSetIfChanged(ref this.frozenOn, value);
        }

        /// <summary>
        /// Initializes a new <see cref="IterationRowViewModel"/>
        /// </summary>
        /// <param name="iterationSetup"></param>
        public IterationRowViewModel(IterationSetup iterationSetup) : base(iterationSetup)
        {
            this.Number = iterationSetup.IterationNumber.ToString();
            this.Description = iterationSetup.Description;
            this.FrozenOn = iterationSetup.FrozenOn != null ? iterationSetup.FrozenOn.Value.ToString("g") : "Active";
        }
    }
}
