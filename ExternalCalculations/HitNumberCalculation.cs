using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WarGrapher.Common;
using WarGrapher.Models.Calculation;
using WarGrapher.Models.Calculation.Utility;

/// <summary>
/// This assembly serves to test and represent the ability of the main application 
/// to automatically read and use calculations from external assemblies.
/// </summary>

namespace ExternalCalculations
{
    /// <summary>
    /// Represents the calculation of 'hits to kill' curves.
    /// </summary>
    [CalculationView("Hits number", "Exposes how much hits are required to kill")]
    [RequiredEquipment(EquipType.Weapon | EquipType.ArmArmor | EquipType.BodyArmor | EquipType.HeadArmor | EquipType.LegArmor)]
    public class HitNumberCalculation : TtkCalculation
    {
        public override CalculationInfo ChartInfo { get; }

        public HitNumberCalculation()
        {
            ChartInfo = new CalculationInfo()
            {
                ChartName = "The chart of hits to kill",
                YAxisName = "Hits number",
                XAxisName = base.ChartInfo.XAxisName,
                XAxisUnit = base.ChartInfo.XAxisUnit,
            };
        }

        protected override Dictionary<string, List<Point>> ExecuteCalculation()
        {
            var calcResult = base.ExecuteCalculation();
            foreach (var weapon in base.weaponsData)
            {
                calcResult[weapon.Name] = calcResult[weapon.Name].
                    Select(
                    point => new Point(point.X, point.Y / (60000 / weapon.RPM))
                    ).ToList();
            }
            return calcResult;
        }
    }
}
