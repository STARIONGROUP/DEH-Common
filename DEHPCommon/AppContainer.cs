// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContainer.cs" company="RHEA System S.A.">
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

namespace DEHPCommon
{
    using Autofac;

    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.Services.FileDialogService;
    using DEHPCommon.Services.IconCacheService;
    using DEHPCommon.Services.NavigationService;
    using DEHPCommon.UserInterfaces.ViewModels;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    /// <summary>
    /// Provides the IOContainer for DI for this Application
    /// </summary>
    public static class AppContainer
    {
        /// <summary>
        /// Gets or sets the <see cref="IContainer"/>
        /// </summary>
        public static IContainer Container { get; set; }

        /// <summary>
        /// Builds the <see cref="Container"/>
        /// </summary>
        /// <param name="containerBuilder">An optional <see cref="Container"/></param>
        public static void BuildContainer(ContainerBuilder containerBuilder = null)
        {
            containerBuilder ??= new ContainerBuilder();
            RegisterViewModels(containerBuilder);
            containerBuilder.RegisterType<IconCacheService>().As<IIconCacheService>().SingleInstance();
            containerBuilder.RegisterType<HubController.HubController>().As<IHubController>().SingleInstance();
            containerBuilder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            containerBuilder.RegisterType<OpenSaveFileDialogService>().As<IOpenSaveFileDialogService>().SingleInstance();
            Container = containerBuilder.Build();
        }

        /// <summary>
        /// Registers all the view model so the depency can be injected
        /// </summary>
        /// <param name="containerBuilder">The <see cref="ContainerBuilder"/></param>
        private static void RegisterViewModels(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<LoginViewModel>().As<ILoginViewModel>();
        }
    }
}
