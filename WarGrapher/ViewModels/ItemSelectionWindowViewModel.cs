using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WarGrapher.ViewModels.Commands;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Represents a view-model for selecting single item.
    /// </summary>
    /// <typeparam name="T">A type of items.</typeparam>
    interface IDataSelector<T> : INotifyPropertyChanged, IWindowViewModel
    {
        ObservableCollection<T> SourceData { get; set; }
        T SelectedItem { get; }
    }

    /// <summary>
    /// Represents the view-model of an item selection window.
    /// </summary>
    /// <typeparam name="T">A type of items.</typeparam>
    class ItemSelectionWindowViewModel<T> : WindowViewModel, IDataSelector<T>
    {
        /// <summary>
        /// Gets the command that executes selecting of a passed item.
        /// This command accepts a value of type <typeparamref name="T"/> as a parameter.
        /// </summary>
        public ICommand ItemSelectionCommand { get; }

        private ObservableCollection<T> _sourceData;
        /// <summary>
        /// Gets or sets an observable collection of source items.
        /// This property is intended for binding to a view.
        /// </summary>
        [Bindable(true, BindingDirection.OneWay)]
        public ObservableCollection<T> SourceData
        {
            get { return _sourceData; }
            set
            {
                _sourceData = value;
                OnPropertyChanged(nameof(SourceData));
            }
        }

        private T _selectedItem;
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public T SelectedItem
        {
            get { return _selectedItem; }
            private set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ItemSelectionWindowViewModel()
        {
            SourceData = new ObservableCollection<T>();

            ItemSelectionCommand = new RelayCommand<T>(
                execute: SelectItem);
        }

        private void SelectItem(T item) => SelectedItem = item;
    }
}
