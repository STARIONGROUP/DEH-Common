// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHubController.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.HubController.Interfaces
{
    using CDP4Dal;

    /// <summary>
    /// Definition of the interface for the <see cref="HubController"/>
    /// </summary>
    public interface IHubController
    {
        /// <summary>
        /// Gets the <see cref="Session"/> object that is encapsulated by the current <see cref="HubController"/>.
        /// </summary>
        ISession Session { get; set; }
    }
}
