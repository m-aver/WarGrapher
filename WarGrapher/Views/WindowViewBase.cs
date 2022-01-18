using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using WindowViewModel = WarGrapher.ViewModels.WindowViewModel;

namespace WarGrapher.Views
{
    /// <summary>
    /// Serves as the base class for a view that is an entire window.
    /// </summary>
    public abstract class WindowViewBase : Window, INotifyPropertyChanged
    {
        private WindowViewModel _viewModel;
        /// <summary>
        /// Gets and sets the view model for binding from xaml
        /// </summary>
        /// <exception cref="ViewModelValidationFailedException"/>
        [Bindable(true, BindingDirection.OneWay)]
        public WindowViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (ValidateViewModel(value))
                {
                    _viewModel = value;
                    OnPropertyChanged(nameof(ViewModel));
                }
                else
                {
                    throw new ViewModelValidationFailedException();
                }
                    
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the window is closed.
        /// </summary>
        [Bindable(false)]
        public bool IsClosed { get; private set; }

        public WindowViewBase()
        {
        }

        /// <summary>
        /// Validates the passed view-model in derived classes.
        /// </summary>
        protected abstract bool ValidateViewModel(WindowViewModel viewModel);      

        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;
            base.OnClosed(e);
        }

        #region property changed code
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

    /// <summary>
    /// The exception that occurs if a view model does not match a validation condition of a client window.
    /// </summary>
    [System.Serializable]
    class ViewModelValidationFailedException : Exception
    {
        public ViewModelValidationFailedException() { }
        public ViewModelValidationFailedException(string message) : base(message) { }
        public ViewModelValidationFailedException(string message, Exception innerException) : base(message, innerException) { }
        protected ViewModelValidationFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
