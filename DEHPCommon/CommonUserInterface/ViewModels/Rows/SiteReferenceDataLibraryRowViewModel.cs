// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.CommonUserInterface.ViewModels.Rows
{
    using CDP4Common.SiteDirectoryData;

    /// <summary>
    /// Row class representing a <see cref="SiteReferenceDataLibrary"/> as a plain object
    /// </summary>
    public class SiteReferenceDataLibraryRowViewModel : DefinedThingRowViewModel<SiteReferenceDataLibrary>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteReferenceDataLibraryRowViewModel"/> class
        /// </summary>
        /// <param name="modelSetup">The <see cref="SiteReferenceDataLibrary"/> associated with this row</param>
        public SiteReferenceDataLibraryRowViewModel(SiteReferenceDataLibrary siteReferenceDataLibrary) : base(siteReferenceDataLibrary)
        {
        }
    }
}
