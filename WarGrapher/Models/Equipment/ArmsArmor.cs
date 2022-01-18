using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WarGrapher.Models.Equipment
{
    /// <summary>
    /// Represents gloves
    /// </summary>
    public class ArmsArmor : Armor
    {
        public ArmsArmor(string name, BitmapImage icon, Dictionary<string, double> paramBase) 
            : base(name, icon, paramBase)
        {
        }
    }
}
