using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WarGrapher.Models.Equipment
{
    /// <summary>
    /// Represents shoes
    /// </summary>
    public class LegsArmor : Armor
    {
        public LegsArmor(string name, BitmapImage icon, Dictionary<string, double> paramBase) 
            : base(name, icon, paramBase)
        {
        }
    }
}
