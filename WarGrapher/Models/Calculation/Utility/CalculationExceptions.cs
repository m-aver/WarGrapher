using System;
using System.Runtime.Serialization;
using WarGrapher.Common;

namespace WarGrapher.Models.Calculation.Utility
{
    /// <summary>
    /// An exception that occurs if equipments data was not found
    /// </summary>
    [Serializable]
    public class NoEquipmentDataException : Exception
    {
        /// <summary>
        /// Types of equipment that were not found
        /// </summary>
        public EquipType EquipmentTypes { get; }

        public NoEquipmentDataException(EquipType equipmentTypes) { EquipmentTypes = equipmentTypes; }
        public NoEquipmentDataException(string message, EquipType equipmentTypes) : base(message) { EquipmentTypes = equipmentTypes; }
        public NoEquipmentDataException(string message, Exception inner, EquipType equipmentTypes) : base(message, inner) { EquipmentTypes = equipmentTypes; }
        protected NoEquipmentDataException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// An exception that occurs during calculation processing
    /// </summary>
    [Serializable]
    public class CalculationException : Exception
    {
        public CalculationException() { }
        public CalculationException(string message) : base(message) { }
        public CalculationException(string message, Exception inner) : base(message, inner) { }
        protected CalculationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
