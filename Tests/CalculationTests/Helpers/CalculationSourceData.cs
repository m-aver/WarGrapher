using System.Collections.Generic;
using WarGrapher.Common;
using WarGrapher.Models.Equipment;

namespace Tests.CalculationTests.Helpers
{
    /// <summary>
    /// Represents the kit of data that is used as input values for a calculation.
    /// </summary>
    internal struct CalculationSourceData
    {
        public IReadOnlyCollection<Weapon> Weapons { get; set; }
        public BodyArmor BodyArmor { get; set; }
        public Armor FocusedBodyPartArmor { get; set; }
        public BodyPart FocusedBodyPart { get; set; }
    }
}
