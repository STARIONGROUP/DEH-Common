// -------------------------------------------------------------------------------------------------
// <copyright file="PublicationParameterOrOverrideRowViewModel.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
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

namespace DEHPCommon.UserInterfaces.ViewModels.PublicationBrowser
{
    using System;
    using System.Globalization;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;

    using ReactiveUI;

    /// <summary>
    /// The row representing a <see cref="ParameterOrOverrideBase" /> in the <see cref="PublicationsViewModel" />
    /// </summary>
    public class PublicationParameterOrOverrideRowViewModel : ParameterOrOverrideBaseRowViewModel, IPublishableRow
    {
        /// <summary>
        /// Backing field for <see cref="PercentageChange" />
        /// </summary>
        private string percentageChange;

        /// <summary>
        /// Backing field for the <see cref="ToBePublished" /> property.
        /// </summary>
        private bool toBePublished;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicationParameterOrOverrideRowViewModel" /> class
        /// </summary>
        /// <param name="parameterOrOverrideBase">
        /// The associated <see cref="ParameterOrOverrideBase" />
        /// </param>
        /// <param name="session">
        /// The associated <see cref="ISession" />
        /// </param>
        /// <param name="containerViewModel">
        /// The container Row.
        /// </param>
        public PublicationParameterOrOverrideRowViewModel(ParameterOrOverrideBase parameterOrOverrideBase, ISession session, IRowViewModelBase<Thing> containerViewModel)
            : base(parameterOrOverrideBase, session, containerViewModel)
        {
            this.WhenAnyValue(vm => vm.ToBePublished).Subscribe(_ => this.ToBePublishedChanged());
            this.IsCheckable = true;
            this.SetProperties();
        }

        /// <summary>
        /// Gets the percentage change representation of the difference between
        /// <see cref="Value" /> and <see cref="Published" /> value.
        /// </summary>
        public string PercentageChange
        {
            get => this.percentageChange;
            private set => this.RaiseAndSetIfChanged(ref this.percentageChange, value);
        }

        /// <summary>
        /// Gets or sets whether this row is checkable.
        /// </summary>
        public bool IsCheckable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ParameterOrOverrideBase" /> is to be published in the next
        /// publication.
        /// </summary>
        public bool ToBePublished
        {
            get => this.toBePublished;

            set => this.RaiseAndSetIfChanged(ref this.toBePublished, value);
        }

        /// <summary>
        /// Sets the row values.
        /// </summary>
        public override void SetProperties()
        {
            base.SetProperties();
            this.ToBePublished = this.Thing.ToBePublished;

            // Set percentage change
            if (!(this.Thing.ParameterType is QuantityKind))
            {
                this.PercentageChange = "-";
                return;
            }

            double oldValue, newValue;

            var newParsed = double.TryParse(this.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out newValue);
            var oldParsed = double.TryParse(this.Published, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out oldValue);

            if (!oldParsed || !newParsed)
            {
                this.PercentageChange = "-";
                return;
            }

            var percentageChage = (newValue - oldValue) / oldValue;
            this.PercentageChange = string.Format(CultureInfo.InvariantCulture, "{0:0.0%}", percentageChage);
        }

        /// <summary>
        /// Execute change of status on whether the row is to be published.
        /// </summary>
        private void ToBePublishedChanged()
        {
            this.Thing.ToBePublished = this.ToBePublished;
        }
    }
}
