using System;
using System.Collections.Generic;
using WarGrapher.Common;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Represents a type of errors that can occur during the work of the application and intended for user notification.
    /// </summary>
    enum ErrorType
    {
        [ErrorTypeDescription("Other")]
        Other,
        [ErrorTypeDescription("User error")]
        UserError,
        [ErrorTypeDescription("Data access error")]
        DataError,
        [ErrorTypeDescription("Developer error")]
        DesignError,
        [ErrorTypeDescription("Unexpected error")]
        UnhandledError
    }

    /// <summary>
    /// Allows to assign a description for a certain constant of <see cref="ErrorType"/> enumeration.
    /// Assigned value will used to representation of an error in a view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    sealed class ErrorTypeDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public ErrorTypeDescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="ErrorWindowViewModel.ErrorRecorded"/> event.
    /// </summary>
    class ErrorRecordedEventArgs : EventArgs
    {
        public IReadOnlyCollection<ErrorRecord> NewErrorRecords { get; }

        public ErrorRecordedEventArgs(IReadOnlyCollection<ErrorRecord> records)
        {
            NewErrorRecords = records;
        }
    }

    /// <summary>
    /// Represents a record of an error to use in a view.
    /// </summary>
    internal struct ErrorRecord
    {
        public DateTime Date { get; }
        public ErrorType ErrorType { get; }
        public string TypeDescription { get; }
        public string Message { get; }

        public ErrorRecord(ErrorType type, string message)
        {
            Date = DateTime.Now;
            Message = message ?? String.Empty;
            ErrorType = type;

            var attr = type.GetEnumConstAttribute<ErrorTypeDescriptionAttribute>();
            TypeDescription = attr?.Description ?? type.ToString();
        }
    }
}
