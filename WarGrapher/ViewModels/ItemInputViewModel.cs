using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WarGrapher.Common;
using WarGrapher.Models;
using WarGrapher.ViewModels.Commands;
using WarGrapher.ViewModels.ViewFactories;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Represents a view-model that provides commands and properties to a view for selection of an equipment item.
    /// </summary>
    class ItemInputViewModel : ElementViewModel
    {
        /// <summary>
        /// Gets a collection of an available equipment for selection by this view-model.
        /// </summary>
        public IReadOnlyCollection<EquipItemViewModel> AcceptableItems { get; private set; }

        private EquipType _equipType;
        /// <summary>
        /// Gets or sets a type of equipment that wiil be acceptable to this view-model.
        /// </summary>
        public EquipType EquipType
        {
            get { return _equipType; }
            set
            {
                _equipType = value;
                OnPropertyChanged(nameof(EquipType));
                RequestItems(value);
            }
        }

        private EquipItemViewModel _sentItem;
        /// <summary>
        /// Gets an equipment item that is sent to the application data model as the selected item.
        /// </summary>
        public EquipItemViewModel SentItem
        {
            get { return _sentItem; }
            private set
            {
                _sentItem = value;
                OnPropertyChanged(nameof(SentItem));
            }
        }

        /// <summary>
        /// Gets the command that executes sending of an equipment item by its name.
        /// This command accepts a <see cref="string"/> value as a parameter.
        /// </summary>
        public ICommand SendNameCommand { get; }
        /// <summary>
        /// Gets the command that executes sending of a passed equipment item.
        /// This command accepts an <see cref="EquipItemViewModel"/> value as a parameter.
        /// </summary>
        public ICommand SendItemCommand { get; }
        /// <summary>
        /// Gets the command that executes reset of a selected equipment item.
        /// The item to reset is taken from the <see cref="SentItem"/> property.
        /// </summary>
        public ICommand ResetItemCommand { get; }
        /// <summary>
        /// Gets the command that executes opening of an equipment selection window.
        /// </summary>
        public ICommand OpenEquipSelectionWindowCommand { get; }

        private readonly IEquipmentDataProvider _model;
        private readonly WindowFactory _selectionWindowFactory;
        private IDataSelector<EquipItemViewModel> _selectionViewModel;

        public ItemInputViewModel()
        {
            _model = ModelFactory.ModelInstance;
            _selectionWindowFactory = new ItemSelectionWindowFactory();

            SendNameCommand = new RelayCommand<string>(
                execute: SendItemName,
                canExecute: CanSendItemName);
            SendItemCommand = new RelayCommand<EquipItemViewModel>(
                execute: SendItem,
                canExecute: CanSendItem);
            ResetItemCommand = new RelayCommand(
                execute: ResetItem,
                canExecute: CanResetItem);
            OpenEquipSelectionWindowCommand = new RelayCommand(
                execute: OpenSelectionWindow,
                canExecute: CanOpenSelectionWindow);            
        }

        private void RequestItems(EquipType type)
        {
            AcceptableItems = _model.GetAllItemsOfType(type).Select(ei => new EquipItemViewModel(ei)).ToArray();
            OnPropertyChanged(nameof(AcceptableItems));
        }

        private bool CanSendItemName(string itemName) => null != AcceptableItems.SingleOrDefault(item => item.Name == itemName);
        private void SendItemName(string itemName)
        {
            var equipItem = AcceptableItems.SingleOrDefault(item => item.Name == itemName);
            if (SendItemCommand.CanExecute(equipItem))
                SendItemCommand.Execute(equipItem);
        }

        private bool CanSendItem(EquipItemViewModel equipItem) => null != AcceptableItems.SingleOrDefault(item =>item.Equals(equipItem));
        private void SendItem(EquipItemViewModel equipItem)
        {
            SentItem = equipItem;
            _model.SetSelectedData(this, equipItem.EquipItem);
        }

        private bool CanResetItem() => SentItem != null;
        private void ResetItem()
        {
            SentItem = null;
            _model.SetSelectedData(this, null);
        }

        private bool CanOpenSelectionWindow() => _selectionViewModel == null || !_selectionViewModel.HasView;
        private void OpenSelectionWindow()
        {
            _selectionViewModel = (IDataSelector<EquipItemViewModel>)_selectionWindowFactory.CreateWindow();
            _selectionViewModel.SourceData = new ObservableCollection<EquipItemViewModel>(AcceptableItems);
            //AcceptableItems.ForEach(ei => _selectViewModel.SourceData.Add(ei));
            _selectionViewModel.PropertyChanged += (obj, arg) =>
            {
                if (arg.PropertyName == nameof(IDataSelector<EquipItemViewModel>.SelectedItem) && 
                    SendItemCommand.CanExecute(_selectionViewModel.SelectedItem))
                {
                    SendItemCommand.Execute(_selectionViewModel.SelectedItem);
                }
            };
        }
    }
}
