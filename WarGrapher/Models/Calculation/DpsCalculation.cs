using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WarGrapher.Common;
using WarGrapher.Models.Calculation.Utility;

namespace WarGrapher.Models.Calculation
{
    /// <summary>
    /// Represents the calculation of 'damage per second' curves
    /// </summary>
    [CalculationView("DPS", "Exposes how fast a weapon delivers damage")]
    [RequiredEquipment(EquipType.Weapon | EquipType.ArmArmor | EquipType.BodyArmor | EquipType.HeadArmor | EquipType.LegArmor)]
    public class DpsCalculation : DamageCalculation
    {
        public override CalculationInfo ChartInfo { get; } = 
            new CalculationInfo()
            {
                ChartName = "DPS chart",
                XAxisName = "Distance",
                XAxisUnit = "m",
                YAxisName = "Damage per second"
            };

        public DpsCalculation()
        {
        }

        internal DpsCalculation(ISelectedDataConsumer model) : base(model)
        {
        }

        protected override Dictionary<string, List<Point>> ExecuteCalculation()
        {
            var damageCurves = base.ExecuteCalculation();
            var output = new Dictionary<string, List<Point>>();

            foreach (var damageCurve in damageCurves)
            {
                var chartPoints = new List<Point>();
                var currentWeapon = weaponsData.First((wd) => wd.Name == damageCurve.Key);
                double damageFactor = currentWeapon.GetBodyPartFactor(focusedBodyPart) - focusedBodyPartArmorData.DamageFactorSubstract;

                foreach (var damageGraphPoint in damageCurve.Value)
                {
                    double currentPDS = damageGraphPoint.Y * damageFactor * (currentWeapon.RPM / 60);
                    chartPoints.Add(new Point(damageGraphPoint.X, currentPDS));
                }
                output.Add(damageCurve.Key, chartPoints);
            }
            return output;
        }
    }
}
