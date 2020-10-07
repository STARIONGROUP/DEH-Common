// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SphericalRule.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Tests.MappingEngine.Dst
{
    using System.Linq;
    using System.Windows;

    using DEHPCommon.MappingRules.Core;

    public class SphericalRule : MappingRule<Cube, Sphere>
    {
        public override Sphere Transform(Cube input)
        {
            return new Sphere()
            {
                Iid = input.Id.ToByteArray().First(),
                Points = input.Sides.Select(x => new Point(x, ((int)x)^((int)x-1))).ToArray()
            };
        }
    }
}
