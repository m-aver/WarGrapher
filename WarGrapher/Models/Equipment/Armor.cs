using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WarGrapher.Models.Equipment
{
    /// <summary>
    /// Represents the base class for armor equipment
    /// </summary>
    public abstract class Armor : EquipItem
    {
        /// <summary>
        /// Gets the value that acts as the substraction from the damage factor of a body part associated with this equipment type
        /// </summary>
        public double DamageFactorSubstract { get; }

        protected Armor(string name, BitmapImage icon, Dictionary<string, double> paramBase)
            : base(name, icon, paramBase)
        {
            DamageFactorSubstract = GetParam("damage factor sub") ?? 0;
        }
    }
}
