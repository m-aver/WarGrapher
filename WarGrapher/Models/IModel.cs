using System.Collections.Generic;
using WarGrapher.Common;
using WarGrapher.Models.Equipment;

namespace WarGrapher.Models
{
    /// <summary>
    /// Provides interaction with the application data model to the viewmodels layer.
    /// </summary>
    interface IModel : IEquipmentDataProvider, IBodyPartDataProvider, ISelectedDataConsumer, IModelErrorNotifier, IObservable
    {
    }

    /// <summary>
    /// Provides methods for getting the user-selected data.
    /// </summary>
    interface ISelectedDataConsumer
    {
        BodyPart FocusedBodyPart { get; }
        IReadOnlyCollection<Armor> GetFocusedBodyPartData();
        IReadOnlyCollection<EquipItem> GetSelectedDataOfType(EquipType equipType);
    }

    /// <summary>
    /// Provides the property for getting and setting the user-selected targed part of the hitbox.
    /// </summary>
    interface IBodyPartDataProvider
    {
        BodyPart FocusedBodyPart { get; set; }
    }
    /// <summary>
    /// Provides methods for choosing and setting the user-selected equipment.
    /// </summary>
    interface IEquipmentDataProvider
    {
        void SetSelectedData(object senderElement, EquipItem selectedItem);
        IReadOnlyCollection<EquipItem> GetAllItemsOfType(EquipType equipType);
    }

    /// <summary>
    /// Notifies listeners about an error that has occured in the application data model.
    /// </summary>
    interface IModelErrorNotifier
    {
        event ModelErrorEventHandler ErrorOccured;
    }
}
