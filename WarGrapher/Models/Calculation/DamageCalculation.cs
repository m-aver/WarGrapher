using System;
using System.Collections.Generic;
using System.Windows;
using WarGrapher.Common;
using WarGrapher.Models.Calculation.Utility;

namespace WarGrapher.Models.Calculation
{
    /// <summary>
    /// Represents the calculation of the fall of damage curves
    /// </summary>
    [CalculationView("Damage", "Exposes a damage drop curve")]
    [RequiredEquipment(EquipType.Weapon)]
    public class DamageCalculation : PlotDataCalculation
    {
        public override CalculationInfo ChartInfo { get; } =
            new CalculationInfo()
            {
                ChartName = "Damage chart",
                XAxisName = "Distance",
                XAxisUnit = "m",
                YAxisName = "Damage"
            };

        private double _maxDistance = 30;

        public DamageCalculation()
        {
        }

        internal DamageCalculation(ISelectedDataConsumer model) : base(model)
        {
        }

        protected override Dictionary<string, List<Point>> ExecuteCalculation()
        {
            var output = new Dictionary<string, List<Point>>();

            foreach (var weapon in weaponsData)
            {
                var chartPoints = new List<Point>();

                double minDamageDistance = (weapon.MaxDamage - weapon.MinDamage) / weapon.DropDamagePerMeter + weapon.Distance;
                double damageAtMaxDistance = weapon.MaxDamage - (_maxDistance - weapon.Distance) * weapon.DropDamagePerMeter;

                chartPoints.Add(new Point(0, weapon.MaxDamage));
                if (_maxDistance <= weapon.Distance)
                    chartPoints.Add(new Point(_maxDistance, weapon.MaxDamage));
                else
                {
                    chartPoints.Add(new Point(weapon.Distance, weapon.MaxDamage));
                    if (_maxDistance < minDamageDistance)
                        chartPoints.Add(new Point(_maxDistance, damageAtMaxDistance));
                    else
                    {
                        chartPoints.Add(new Point(minDamageDistance, weapon.MinDamage));
                        chartPoints.Add(new Point(_maxDistance, weapon.MinDamage));
                    }
                }
                output.Add(weapon.Name, chartPoints);
            }
            return output;
        }
    }
}
