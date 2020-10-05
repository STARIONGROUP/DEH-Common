// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerType.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserPreferenceHandler.Enums
{
    /// <summary>
    /// Possible value of the <see cref="ServerType"/>
    /// </summary>
    public enum ServerType
    {
        /// <summary>
        /// Reprensents a CDP4 Webservice
        /// </summary>
        Cdp4WebServices,

        /// <summary>
        /// Represents a OCDT WSP server
        /// </summary>
        OcdtWspServer
    }
}
