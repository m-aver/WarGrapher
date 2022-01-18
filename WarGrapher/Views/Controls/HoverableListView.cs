using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace WarGrapher.Views.Controls
{
    /// <summary>
    /// Represents <see cref="ListView"/> that supports tracking a current item under the mouse pointer.
    /// </summary>
    class HoverableListView : ListView, INotifyPropertyChanged
    {
        private object _hoveredItem;
        /// <summary>
        /// Gets the item that now located under the mouse pointer or null if there no item under the mouse pointer.
        /// </summary>
        public object HoveredItem
        {
            get { return _hoveredItem; }
            private set
            {
                _hoveredItem = value;
                OnPropertyChanged(nameof(HoveredItem));
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                HandleNewItems(base.Items);
            }

            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                HandleNewItems(e.NewItems);
            }
        }

        private void HandleNewItems(IEnumerable newItems)
        {
            var listItems = newItems?
                .OfType<object>()
                .Select(i => base.ItemContainerGenerator.ContainerFromItem(i))
                .OfType<ListViewItem>();

            if (listItems != null)
            {
                foreach(ListViewItem item in listItems)
                {
                    item.MouseEnter += ItemMouseEnterHandler;
                    item.MouseLeave += ItemMouseLeaveHandler;
                }
            }
        }

        private void ItemMouseEnterHandler(object sender, MouseEventArgs e)
        {
            if(sender is ListViewItem)
            {
                HoveredItem = base.ItemContainerGenerator.ItemFromContainer(sender as ListViewItem);
            }            
        }

        private void ItemMouseLeaveHandler(object sender, MouseEventArgs e)
        {
            HoveredItem = null;
        }

        #region -- Property changed code --
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler propertyChangedHandler = this.PropertyChanged;
            propertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Verify that the property name matches a real, public, instance property on this object. 
        /// </summary>
        private void VerifyPropertyName(string propertyName)
        {
            if (this.GetType().GetProperty(propertyName) == null)
            {
                string msg = "Invalid property name: " + propertyName;
                throw new Exception(msg);
            }
        }
        #endregion
    }
}
