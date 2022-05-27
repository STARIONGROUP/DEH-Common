// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubSessionControlViewModelTestFixture.cs" company="RHEA System S.A.">
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

using NUnit.Framework;

namespace DEHPCommon.Tests.UserInterfaces.ViewModels
{
    using System.Reactive.Concurrency;
    using System.Threading.Tasks;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.Exceptions;

    using DEHPCommon.Enumerators;
    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using Moq;

    using ReactiveUI;

    [TestFixture]
    public class HubSessionControlViewModelTestFixture
    {
        private HubSessionControlViewModel viewModel;
        private Mock<IHubController> hubController;
        private Mock<IStatusBarControlViewModel> statusBar;

        [SetUp]
        public void Setup()
        {
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;
            this.statusBar = new Mock<IStatusBarControlViewModel>();
            this.hubController = new Mock<IHubController>();

            this.viewModel = new HubSessionControlViewModel(this.hubController.Object, this.statusBar.Object);
        }

        [Test]
        public void VerifyProperties()
        {
            Assert.Zero(this.viewModel.AutoRefreshSecondsLeft);
            Assert.AreEqual(60, this.viewModel.AutoRefreshInterval);
            Assert.IsFalse(this.viewModel.IsAutoRefreshEnabled);
            Assert.IsFalse(this.viewModel.IsSessionOpen);
            Assert.IsNotNull(this.viewModel.RefreshCommand);
            Assert.IsNotNull(this.viewModel.ReloadCommand);
        }

        [Test]
        public void VerifyCommands()
        {
            Assert.IsFalse(this.viewModel.RefreshCommand.CanExecute(null));
            Assert.IsFalse(this.viewModel.ReloadCommand.CanExecute(null));

            this.hubController.Setup(x => x.OpenIteration).Returns(new Iteration());
            this.hubController.Setup(x => x.IsSessionOpen).Returns(true);
            this.viewModel = new HubSessionControlViewModel(this.hubController.Object, this.statusBar.Object);

            Assert.IsTrue(this.viewModel.RefreshCommand.CanExecute(null));
            Assert.IsTrue(this.viewModel.ReloadCommand.CanExecute(null));

            Assert.DoesNotThrowAsync(async () => await this.viewModel.RefreshCommand.ExecuteAsyncTask());
            Assert.DoesNotThrowAsync(async () => await this.viewModel.ReloadCommand.ExecuteAsyncTask());

            this.hubController.Setup(x => x.Refresh()).Throws<IncompleteModelException>();
            this.hubController.Setup(x => x.Reload()).Throws<IncompleteModelException>();
            Assert.DoesNotThrowAsync(async () => await this.viewModel.RefreshCommand.ExecuteAsyncTask());
            Assert.DoesNotThrowAsync(async () => await this.viewModel.ReloadCommand.ExecuteAsyncTask());

            this.hubController.Verify(x => x.Refresh(), Times.Exactly(2));
            this.hubController.Verify(x => x.Reload(), Times.Exactly(2));
            this.statusBar.Verify(x => x.Append(It.IsAny<string>(), StatusBarMessageSeverity.Info), Times.Exactly(2));
            this.statusBar.Verify(x => x.Append(It.IsAny<string>(), StatusBarMessageSeverity.Error), Times.Exactly(2));
        }

        [Test]
        public void VerifyAutoRefreshSet()
        {
            this.hubController.Setup(x => x.Refresh());
            Assert.Zero(this.viewModel.AutoRefreshSecondsLeft);
            Assert.AreEqual(60, this.viewModel.AutoRefreshInterval);
            Assert.IsFalse(this.viewModel.IsAutoRefreshEnabled);

            const uint refreshInterval = 2u;
            this.viewModel.AutoRefreshInterval = refreshInterval;
            this.viewModel.IsAutoRefreshEnabled = true;
            Assert.AreEqual(2, this.viewModel.AutoRefreshSecondsLeft);
        }

        [Test]
        public async Task VerifyOnTimerElapsed()
        {
            this.hubController.Setup(x => x.Refresh());
            Assert.DoesNotThrow(() => this.viewModel.OnTimerElapsed(null, null));

            this.viewModel.AutoRefreshSecondsLeft = 1;
            
            Assert.DoesNotThrow(() => this.viewModel.OnTimerElapsed(null, null));
            await Task.Delay(50);
            this.hubController.Verify(x => x.Refresh(), Times.Once);

            this.statusBar.Verify(x => 
                x.Append(It.IsAny<string>(), It.IsAny<StatusBarMessageSeverity>()), Times.Never);
        }
    }
}
