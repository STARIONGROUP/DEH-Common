// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelRowViewModelTestFixture.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.CommonUserInterface.Rows
{
    using System;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using DEHPCommon.CommonUserInterface.ViewModels.Rows;

    using NUnit.Framework;

    [TestFixture]
    public class EngineeringModelRowViewModelTestFixture
    {
        [Test]
        public void VerifyProperties()
        {
            var model = new EngineeringModelSetup(Guid.NewGuid(), null, null);

            var row = new EngineeringModelRowViewModel(model);

            Assert.AreSame(row.Thing, model);
            Assert.AreEqual(row.Iid, model.Iid);
            Assert.AreSame(row.Name, model.Name);
            Assert.AreSame(row.ShortName, model.ShortName);
            Assert.AreEqual(row.RevisionNumber, model.RevisionNumber);
            Assert.AreEqual(row.Kind, model.Kind);
            Assert.IsTrue(row.IsSelected);
        }
    }
}
