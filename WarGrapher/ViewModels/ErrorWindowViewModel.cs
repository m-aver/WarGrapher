using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using WarGrapher.Common;
using WarGrapher.Models;
using WarGrapher.ViewModels.Commands;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Provides methods for recording occured errors.
    /// </summary>
    internal interface IErrorRecorder : IWindowViewModel
    {
        void SendError(ErrorType type, string message);
        void SendError(ErrorType type, Exception error);
        event EventHandler<ErrorRecordedEventArgs> ErrorRecorded;
    }
    
    /// <summary>
    /// Represents the view-model of the errors window.
    /// </summary>
    class ErrorWindowViewModel : WindowViewModel, IErrorRecorder
    {
        /// <summary>
        /// Gets the collection of error records for representation in a view.
        /// </summary>
        public ObservableCollection<ErrorRecord> Errors { get; }
        /// <summary>
        /// Gets the command that executes clearing of all error records.
        /// </summary>
        public ICommand ClearErrorRecordsCommand { get; }

        /// <summary>
        /// Raises when an error is recorded.
        /// </summary>
        public event EventHandler<ErrorRecordedEventArgs> ErrorRecorded;

        private IModelErrorNotifier _model;

        public ErrorWindowViewModel()
        {
            Errors = new ObservableCollection<ErrorRecord>();
            Errors.CollectionChanged += HandleErrorCollectionChanged;

            _model = ModelFactory.ModelInstance;
            _model.ErrorOccured += HandleModelError;

            ClearErrorRecordsCommand = new RelayCommand(
                execute: ClearErrorRecords,
                canExecute: CanClearErrorRecords);
        }

        /// <summary>
        /// Sends an error message to record.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void SendError(ErrorType type, string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Errors.Add(new ErrorRecord(type, message));
        }

        /// <summary>
        /// Sends an error object to record.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void SendError(ErrorType type, Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            string message = error.GetExceptionStackDescription();
            SendError(type, message);
        }

        private void HandleErrorCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnErrorRecorded(e.NewItems.Cast<ErrorRecord>().ToArray());
            }
        }

        private void HandleModelError(object sender, ModelErrorEventArgs e)
        {
            #region REMARK
            //хз мб передавать ErrorType через ModelErrorEventArgs, 
            //а то получается надо любые возможные типы исключений проверять
            //можно даже делать это через свойство Data самого Exception, пожалуй это наиболее адекватный варик 
            #endregion
            if (!e.Handled &&
                (e.Exception is System.IO.FileFormatException ||
                 e.Exception is System.IO.FileNotFoundException ||
                 e.Exception is System.Configuration.ConfigurationException ||
                 e.Exception is System.Configuration.ConfigurationErrorsException))
            {
                SendError(ErrorType.DataError, e.Exception);
                e.Handled = true;
            }            
        }

        private void OnErrorRecorded(IReadOnlyCollection<ErrorRecord> records)
        {
            EventHandler<ErrorRecordedEventArgs> errorRecordedHandler = ErrorRecorded;
            errorRecordedHandler?.Invoke(this, new ErrorRecordedEventArgs(records));
        }

        private bool CanClearErrorRecords() => Errors.Count > 0;
        private void ClearErrorRecords() => Errors.Clear();
    }
}
