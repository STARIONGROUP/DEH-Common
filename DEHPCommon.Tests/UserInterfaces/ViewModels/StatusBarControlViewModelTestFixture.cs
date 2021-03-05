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
    using DEHPCommon.UserInterfaces.ViewModels;

    [TestFixture]
    public class StatusBarControlViewModelTestFixture
    {
        [Test]
        public void VerifyProperties()
        {
            var viewModel = new StatusBarControlViewModel();
            Assert.IsNull(viewModel.Message);
            Assert.AreEqual(StatusBarMessageSeverity.None, viewModel.Severity);
            Assert.IsNotNull(viewModel.UserSettingCommand);
        }

        [Test]
        public void VerifyAppend()
        {
            const string message = "testMessage";
            const string errorMessage = "errorMessage";
            var viewModel = new StatusBarControlViewModel();
            viewModel.Append(message);
            Assert.AreEqual(message,viewModel.Message.Split(' ').Last());
            Assert.AreEqual(StatusBarMessageSeverity.Info, viewModel.Severity);
            
            viewModel.Append(errorMessage, StatusBarMessageSeverity.Error);
            Assert.AreEqual(errorMessage, viewModel.Message.Split(' ').Last());
            Assert.AreEqual(StatusBarMessageSeverity.Error, viewModel.Severity);

            viewModel.Append(errorMessage, StatusBarMessageSeverity.None);
            Assert.AreEqual(errorMessage, viewModel.Message.Split(' ').Last());
            Assert.AreEqual(StatusBarMessageSeverity.None, viewModel.Severity);

            viewModel.Append(errorMessage, StatusBarMessageSeverity.Warning);
            Assert.AreEqual(errorMessage, viewModel.Message.Split(' ').Last());
            Assert.AreEqual(StatusBarMessageSeverity.Warning, viewModel.Severity);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                viewModel.Append(errorMessage, (StatusBarMessageSeverity)42));
        }
    }
}
