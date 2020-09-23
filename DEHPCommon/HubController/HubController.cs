// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubController.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.HubController
{
    using CDP4Dal;

    using DEHPCommon.HubController.Interfaces;

    /// <summary>
    /// Definition of the <see cref="HubController"/>
    /// </summary>
    public class HubController : IHubController
    {
        /// <summary>
        /// Gets the <see cref="Session"/> object that is encapsulated by the current <see cref="HubController"/>.
        /// </summary>
        public ISession Session { get; set; }

    }
}
