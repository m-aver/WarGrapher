using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using WarGrapher.Models.Equipment;
using WarGrapher.ViewModels.ViewFactories;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Serves as a wrapper of an equipment item for interaction with a view.
    /// </summary>
    public class EquipItemViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets a wrapped equipment item.
        /// </summary>
        [Bindable(false)]
        public EquipItem EquipItem { get; }

        private readonly WindowFactory _viewFactory;
        private readonly IErrorRecorder _errorViewModel;

        public EquipItemViewModel(EquipItem equipItem)
        {
            if (equipItem == null) throw new ArgumentNullException(nameof(equipItem));

            _viewFactory = new ErrorWindowFactory();
            _errorViewModel = (IErrorRecorder)_viewFactory.GetCommonViewModel();

            EquipItem = equipItem;
            EquipItem.ParamNotFound += HandleParamNotFound;
        }

        //wrappers over properties and methods of an equipment item
        public string Name => EquipItem.Name;
        public BitmapImage Icon => EquipItem.Icon;

        public IEnumerable<string> GetParamNames() => EquipItem.GetParamNames();
        public double? GetParam(string paramName) => EquipItem.GetParam(paramName);

        //overriding equals and gethashcode for equipment comparision
        public sealed override bool Equals(object obj) => (obj as EquipItemViewModel)?.EquipItem.Equals(this.EquipItem) ?? false;
        public sealed override int GetHashCode() => EquipItem.GetHashCode();

        private void HandleParamNotFound(object sender, ParamNotFoundEventArgs e)
        {
            string message = $"the parameter <{e.ParamName}> of the item {EquipItem.Name} [type: {EquipItem.GetType().Name}] was not found";
            _errorViewModel.SendError(ErrorType.DataError, message);
        }

        //TODO:
        //override operator == for compare EquipItem
        //implicit type casting to EquipItem
    }
}
