// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetChangePreviewViewModel.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.UserInterfaces.ViewModels.NetChangePreview
{
    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.Services.ObjectBrowserTreeSelectorService;
    using DEHPCommon.UserInterfaces.ViewModels.NetChangePreview.Interfaces;

    /// <summary>
    /// View model for the preview net change pane allowing the user to preview the change that will be transfered to either the Dst of the Hub
    /// </summary>
    public abstract class NetChangePreviewViewModel : ObjectBrowserViewModel, INetChangePreviewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBrowserViewModel"/> class.
        /// </summary>
        /// <param name="hubController">The <see cref="IHubController"/></param>
        /// <param name="objectBrowserTreeSelectorService">The <see cref="IObjectBrowserTreeSelectorService"/></param>
        protected NetChangePreviewViewModel(IHubController hubController, IObjectBrowserTreeSelectorService objectBrowserTreeSelectorService) : base(hubController, objectBrowserTreeSelectorService)
        {
        }

        /// <summary>
        /// Computes the old values for each <see cref="ObjectBrowserViewModel.Things"/>
        /// </summary>
        public abstract void ComputeValues();
    }
}
