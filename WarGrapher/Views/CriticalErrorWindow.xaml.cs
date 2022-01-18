using System;
using System.ComponentModel;
using System.Windows;
using WarGrapher.Common;

namespace WarGrapher.Views
{
    /// <summary>
    /// Represents the lightweight error window that notifies about a critical error and the application shutdown. 
    /// This window is not a part of the MVVM template.
    /// </summary>
    public partial class CriticalErrorWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a message of an error. This property is intended for binding to XAML.
        /// </summary>
        public string ErrorMessage { get; private set; }

        public CriticalErrorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sends an error object to record.
        /// </summary>
        public void SendError(Exception error)
        {
            error = error == null ? new Exception("an error desciption is not received") : error;
            ErrorMessage = error.GetExceptionStackDescription();
            OnPropertyChanged(nameof(ErrorMessage));
        }

        #region Property changed code
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChangedHandler = this.PropertyChanged;
            propertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
