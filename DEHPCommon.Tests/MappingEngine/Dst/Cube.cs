// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cube.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.MappingEngine.Dst
{
    using System;
    using System.Collections.Generic;

    public class Cube
    {
        public Guid Id { get; set; }

        public IEnumerable<double> Sides { get; set; }
    }
}
