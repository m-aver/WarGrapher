using System;

namespace WarGrapher.Common
{
    /// <summary>
    /// Represents a part of the body hitbox model
    /// </summary>
    public enum BodyPart
    {
        [BodyPartCast(EquipType.BodyArmor)]
        Body,
        [BodyPartCast(EquipType.HeadArmor)]
        Head,
        [BodyPartCast(EquipType.ArmArmor)]
        Arms,
        [BodyPartCast(EquipType.LegArmor)]
        Legs,
    }

    /// <summary>
    /// Defines the mapping between constants of <see cref="WarGrapher.Common.BodyPart"/> 
    /// and <see cref="WarGrapher.Common.EquipType"/> enumerations
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class BodyPartCastAttribute : Attribute
    {
        /// <summary>
        /// The equipment type that corresponds to the source body part
        /// </summary>
        public readonly EquipType EquipType;

        public BodyPartCastAttribute(EquipType equipType)
        {
            EquipType = equipType;
        }
    }
}
