// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferControlViewModelTestFixture.cs" company="RHEA System S.A.">
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
    using DEHPCommon.UserInterfaces.ViewModels;

    [TestFixture]
    public class TransferControlViewModelTestFixture
    {
        private class TestTransferControlViewModel : TransferControlViewModel
        {
        }

        [Test]
        public void VerifyProperties()
        {
            var vm = new TestTransferControlViewModel
            {
                Progress = 0, IsIndeterminate = false
            };

            Assert.IsFalse(vm.IsIndeterminate);
            Assert.IsNull(vm.TransferCommand);
            Assert.IsNull(vm.CancelCommand);
            Assert.Zero(vm.Progress);
            Assert.Zero(vm.NumberOfThing);
            Assert.IsNotNull(vm.TransferButtonText);
        }

        [Test]
        public void VerifyTransferButtonText()
        {
            var vm = new TestTransferControlViewModel();

            const string transferText = "Transfer";
            Assert.AreEqual(transferText, vm.TransferButtonText);
            vm.NumberOfThing = 1;
            Assert.AreEqual("Transfer 1 thing", vm.TransferButtonText);
            vm.NumberOfThing = 42;
            Assert.AreEqual("Transfer 42 things", vm.TransferButtonText);
            vm.NumberOfThing = 0;
            Assert.AreEqual(transferText, vm.TransferButtonText);
        }
    }
}
