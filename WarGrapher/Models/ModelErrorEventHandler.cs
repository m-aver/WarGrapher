using System;

namespace WarGrapher.Models
{
    /// <summary>
    /// Represents the method that will handle the <see cref="Model.ErrorOccured"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    delegate void ModelErrorEventHandler(object sender, ModelErrorEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="Model.ErrorOccured"/> event.
    /// </summary>
    class ModelErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the exception that was raised when executing code of the model.
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// Gets or sets the value that indicates whether the exception was handled.
        /// </summary>
        public bool Handled { get; set; }       //если исключение не обработано, то необходимо будет выбросить его обратно

        public ModelErrorEventArgs(Exception exception)
        {
            Exception = exception;
            Handled = false;
        }
    }
}
