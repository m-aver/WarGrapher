using System;
using WarGrapher.Common;

namespace WarGrapher.Models.Calculation.Utility
{
    /// <summary>
    /// Defines the attribute that contains properties for represenation a calculation to the user interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CalculationViewAttribute : Attribute
    {
        public string CalculationName { get; }
        public string CalculationDescription { get; }

        public CalculationViewAttribute(string name, string description)
        {
            CalculationName = name;
            CalculationDescription = description;
        }
    }

    /// <summary>
    /// Represents the attribute that specified which of the equipment types is required for a calculation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RequiredEquipmentAttribute : Attribute
    {
        /// <summary>
        /// Bit field of <see cref="EquipType"/> defining a required types.
        /// </summary>
        public EquipType EquipmentTypes { get; }

        public RequiredEquipmentAttribute(EquipType equipmentTypes)
        {
            EquipmentTypes = equipmentTypes;
        }
    }
}
