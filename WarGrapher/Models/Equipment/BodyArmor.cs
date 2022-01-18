using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WarGrapher.Models.Equipment
{
    /// <summary>
    /// Represents a vest
    /// </summary>
    public class BodyArmor : Armor
    {
        /// <summary>
        /// Gets the default amount of health points
        /// </summary>
        public double HealthDefaultCapacity { get; }
        /// <summary>
        /// Gets the default amount of armor points
        /// </summary>
        public double ArmorDefaultCapacity { get; }

        /// <summary>
        /// Gets the number of bonus armor points
        /// </summary>
        public double ArmorBonusCapacity { get; }
        /// <summary>
        /// Gets the amount of absorbed damage points per each shot
        /// </summary>
        public double DamageAbsorb { get; }

        public BodyArmor(string name, BitmapImage icon, Dictionary<string, double> paramBase)
            : base(name, icon, paramBase)
        {
            HealthDefaultCapacity = 125;
            ArmorDefaultCapacity = 100;

            ArmorBonusCapacity = GetParam("armor bonus") ?? 0;
            DamageAbsorb = GetParam("damage absorb") ?? 0;
        }
    }
}
