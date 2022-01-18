using System;

namespace WarGrapher.Common
{
    #region REMARK
    // Flags
    // возможность выбора нескольких видов экипировки
    // артибут для типов расчета, указывающий какие виды экипировки могут понадобиться.  обработка в CanExecute кнопки расчета
    #endregion

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
