// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusBarControlViewModelTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPCommon.Tests.UserInterfaces.ViewModels
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    using DEHPCommon.Enumerators;
    using DEHPCommon.Services.NavigationService;
    using DEHPCommon.UserInterfaces.ViewModels;

    using Moq;

    [TestFixture]
    public class StatusBarControlViewModelTestFixture
    {
        private StatusBarControlViewModelTestClass viewModel;
        private const string TextMessage = "Executed the user setting command";

        private class StatusBarControlViewModelTestClass : StatusBarControlViewModel
        {
            /// <summary>
            /// Executes the <see cref="StatusBarControlViewModel.UserSettingCommand"/>
            /// </summary>
            protected override void ExecuteUserSettingCommand()
            {
                this.Append(TextMessage);
            }

            /// <summary>
            /// Initializes a new <see cref="StatusBarControlViewModel"/>
            /// </summary>
            /// <param name="navigationService">The <see cref="NavigationService"/></param>
            public StatusBarControlViewModelTestClass(INavigationService navigationService) : base(navigationService)
            {
            }
        }

        [SetUp]
        public void Setup()
        {
            this.viewModel = new StatusBarControlViewModelTestClass(new Mock<INavigationService>().Object);
        }

        [Test]
        public void VerifyExecuteUserSettingCommand()
        {
            this.viewModel.UserSettingCommand.Execute(null);
            Assert.IsTrue(this.viewModel.Message.Contains(TextMessage));
        }

        [Test]
        public void VerifyProperties()
        {
            Assert.IsNull(this.viewModel.Message);
            Assert.AreEqual(StatusBarMessageSeverity.None, this.viewModel.Severity);
            Assert.IsNotNull(this.viewModel.UserSettingCommand);
        }

        [Test]
        public void VerifyAppend()
        {
            const string message = "testMessage";
            const string errorMessage = "errorMessage";
            this.viewModel.Append(message);
            Assert.AreEqual(message, this.viewModel.Message.Split(' ').Last());
            Assert.AreEqual(StatusBarMessageSeverity.Info, this.viewModel.Severity);

            this.viewModel.Append(errorMessage, StatusBarMessageSeverity.Error);
            Assert.AreEqual(errorMessage, this.viewModel.Message.Split(' ').Last());
            Assert.AreEqual(StatusBarMessageSeverity.Error, this.viewModel.Severity);

            this.viewModel.Append(errorMessage, StatusBarMessageSeverity.None);
            Assert.AreEqual(errorMessage, this.viewModel.Message.Split(' ').Last());
            Assert.AreEqual(StatusBarMessageSeverity.None, this.viewModel.Severity);

            this.viewModel.Append(errorMessage, StatusBarMessageSeverity.Warning);
            Assert.AreEqual(errorMessage, this.viewModel.Message.Split(' ').Last());
            Assert.AreEqual(StatusBarMessageSeverity.Warning, this.viewModel.Severity);

            Assert.Throws<ArgumentOutOfRangeException>(() => this.viewModel.Append(errorMessage, (StatusBarMessageSeverity)42));
        }
    }
}
