// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationServiceTestFixture.cs" company="RHEA System S.A.">
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

using NUnit.Framework;

namespace DEHPCommon.Tests.Services
{
    using System.Threading;

    using Autofac;

    using DEHPCommon.Services.NavigationService;
    using DEHPCommon.Tests.Services.TestView;
    
    [TestFixture, Apartment(ApartmentState.STA)]
    public class NavigationServiceTestFixture
    {
        [Test]
        public void VerifyCorrectViewModelIsInitialized()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<ViewTestViewModel>().As<IViewTestViewModel>();
            AppContainer.BuildContainer(containerBuilder);

            var navigationService = new NavigationService();
            var viewInstance = navigationService.BuildView<ViewTest>();
            Assert.IsNotNull(viewInstance.DataContext);
            Assert.AreEqual(typeof(ViewTestViewModel),viewInstance.DataContext.GetType());
            viewInstance = navigationService.BuildView<ViewTest, IViewTestViewModel>(null);
            Assert.IsNotNull(viewInstance.DataContext);
            Assert.AreEqual(typeof(ViewTestViewModel), viewInstance.DataContext.GetType());
            viewInstance = navigationService.BuildView<ViewTest, IViewTestViewModel>(AppContainer.Container.Resolve<IViewTestViewModel>());
            Assert.IsNotNull(viewInstance.DataContext);
            Assert.AreEqual(typeof(ViewTestViewModel), viewInstance.DataContext.GetType());
        }
    }
}
