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
        public SphericalRule(Cube input) : base(input)
        {
        }

        public override void Transform()
        {
            this.Output = new Sphere()
            {
                Iid = this.Input.Id.ToByteArray().First(),
                Points = this.Input.Sides.Select(x => new Point(x, ((int)x)^((int)x-1))).ToArray()
            };
        }
    }
}
