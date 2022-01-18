using System.Collections.Generic;
using WarGrapher.Models.Equipment;

namespace WarGrapher.DataAccessLayer
{
    /// <summary>
    /// Provides access to external data of equipment.
    /// </summary>
    interface IEquipmentDataContext
    {
        IReadOnlyCollection<EquipItem> GetAllEquipment();
    }
}
