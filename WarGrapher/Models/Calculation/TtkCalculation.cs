using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WarGrapher.Common;
using WarGrapher.Models.Calculation.Utility;

namespace WarGrapher.Models.Calculation
{
    /// <summary>
    /// Represents the calculation of 'time to kill' curves.
    /// </summary>
    [CalculationView("TTK", "Exposes how fast a weapon kills the enemy")]
    [RequiredEquipment(EquipType.Weapon | EquipType.ArmArmor | EquipType.BodyArmor | EquipType.HeadArmor | EquipType.LegArmor)]
    public class TtkCalculation : PlotDataCalculation
    {
        public override CalculationInfo ChartInfo { get; }

        private double _maxDistance = 30;

        public TtkCalculation()
        {
            ChartInfo = new CalculationInfo()
            {
                ChartName = "TTK chart",
                XAxisName = "Distance",
                XAxisUnit = "m",
                YAxisName = "Time to kill",
                YAxisUnit = "ms",
            };
        }

        protected override Dictionary<string, List<Point>> ExecuteCalculation()
        {
            int maxHitNumber = 30;
            var output = new Dictionary<string, List<Point>>();

            //first calculate the damage tresholds for each hit amount on a current armor            
            double totalHP = bodyArmorData.HealthDefaultCapacity + bodyArmorData.ArmorDefaultCapacity + bodyArmorData.ArmorBonusCapacity;
            var damageLimits = new List<Tuple<int, double>>()
                .Select(t => new { HitNumber = t.Item1, Damage = t.Item2 }).ToList();

            for (int hitNum = 1; hitNum <= maxHitNumber; hitNum++)
            {
                double limitDamage = bodyArmorData.DamageAbsorb + totalHP / hitNum;
                damageLimits.Add(new { HitNumber = hitNum, Damage = limitDamage });
            }
            
            //then find where the damage characteristic of a current weapon crosses these theshold
            foreach (var weapon in weaponsData)
            {
                var chartPoints = new List<Point>();                

                double totalDamageFactor = weapon.GetBodyPartFactor(focusedBodyPart) - focusedBodyPartArmorData.DamageFactorSubstract;
                double maxDamaage = weapon.MaxDamage * totalDamageFactor;
                double minDamage = weapon.MinDamage * totalDamageFactor;
                double previousDistance = 0;

                chartPoints.Add(new Point(0, 0));
                foreach (var limit in damageLimits)
                {
                    //get the distance of current theshold damage and calculate TTK at this distance
                    if (maxDamaage >= limit.Damage && minDamage <= limit.Damage)
                    {
                        double currentDistance = (maxDamaage - limit.Damage) / weapon.DropDamagePerMeter + weapon.Distance;
                        double currentTTK = limit.HitNumber * (60000 / weapon.RPM);
                        if (currentDistance > _maxDistance) break;

                        chartPoints.Add(new Point(previousDistance, currentTTK));
                        chartPoints.Add(new Point(currentDistance, currentTTK));
                        previousDistance = currentDistance;
                    }
                }
                if (chartPoints.Capacity > 1) chartPoints.Add(new Point(_maxDistance, chartPoints.Last().Y));
                output.Add(weapon.Name, chartPoints);
            }
            return output;
        }
    }
}
