using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WarGrapher.Models.Equipment
{
    /// <summary>
    /// Represents a helmet
    /// </summary>
    public class HeadArmor : Armor
    {
        public HeadArmor(string name, BitmapImage icon, Dictionary<string, double> paramBase)
            : base(name, icon, paramBase)
        {
        }
    }
}
