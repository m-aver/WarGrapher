using System;

namespace WarGrapher.Common
{
    /// <summary>
    /// Represents a type of specific sort of game equipment
    /// </summary>
    [Flags]
    public enum EquipType
    {
        Weapon = 1,
        BodyArmor = 2,
        HeadArmor = 4,
        ArmArmor = 8,
        LegArmor = 16,
    }
}
