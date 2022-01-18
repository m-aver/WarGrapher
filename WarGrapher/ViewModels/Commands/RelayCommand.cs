using System;

namespace WarGrapher.ViewModels.Commands
{
    /// <summary>
    /// Represents a command without the parameter.
    /// </summary>
    class RelayCommand : CommandBase
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand() { }
        public RelayCommand(Action execute) : this(execute, null) { }
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public override void Execute(object parameter) => _execute();
    }

    /// <summary>
    /// Represents a relay command with the strongly typed parameter.
    /// </summary>
    /// <typeparam name="T">A type of the command parameter.</typeparam>
    class RelayCommand<T> : RelayCommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute) : this(execute, null) { }
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => parameter is T && (_canExecute == null || _canExecute((T)parameter));
        public override void Execute(object parameter) => _execute((T)parameter); 
    }
}
