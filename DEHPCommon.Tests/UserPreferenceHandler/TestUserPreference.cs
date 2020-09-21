// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestUserPreference.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.UserPreferenceHandler
{
    using System;

    using DEHPCommon.UserPreferenceHandler;
    using DEHPCommon.UserPreferenceHandler.UserPreferenceService;

    /// <summary>
    /// A <see cref="UserPreference"/> used for testing the <see cref="IUserPreferenceService"/>
    /// </summary>
    public class TestUserPreference : UserPreference
    {
        public Guid Identifier { get; set; }

        public string Description { get; set; }
    }
}
