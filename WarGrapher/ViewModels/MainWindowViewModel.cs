using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WarGrapher.Common;
using WarGrapher.ViewModels.Commands;
using WarGrapher.ViewModels.ViewFactories;

namespace WarGrapher.ViewModels
{
    interface IWindowRegistrator
    {
        void RegisterWindow(WindowViewModel viewModel);
        void UnregisterWindow(WindowViewModel viewModel);
    }

    /// <summary>
    /// Represents the view-model of the main window.
    /// </summary>
    class MainWindowViewModel : WindowViewModel, IWindowRegistrator
    {
        private int _newHiddenErrorRecordsCount;
        /// <summary>
        /// Gets amount of new errors that yet was not showed in a view. 
        /// </summary>
        public int NewHiddenErrorRecordsCount
        {
            get { return _newHiddenErrorRecordsCount; }
            private set
            {
                _newHiddenErrorRecordsCount = value;
                OnPropertyChanged(nameof(NewHiddenErrorRecordsCount));
            }
        }

        /// <summary>
        /// Gets the command that executes opening of the errors window.
        /// </summary>
        public ICommand OpenErrorWindowCommand { get; }
        /// <summary>
        /// Gets the command that executes closing of all chart windows.
        /// </summary>
        public ICommand CloseAllGraphCommand { get; }

        private readonly WindowFactory _errorViewFactory;
        private readonly IErrorRecorder _errorViewModel;
        private readonly HashSet<WindowViewModel> _subWindows;

        public MainWindowViewModel()
        {
            _errorViewFactory = new ErrorWindowFactory();
            _errorViewModel = (IErrorRecorder)_errorViewFactory.GetCommonViewModel();
            _errorViewModel.ErrorRecorded += HandleErrorRecorded;

            OpenErrorWindowCommand = new RelayCommand(
                execute: OpenErrorWindow,
                canExecute: CanOpenErrorWindow);

            _subWindows = new HashSet<WindowViewModel>();
            CloseAllGraphCommand = new RelayCommand(
                execute: CloseAllGraph,
                canExecute: CanCloseAllGraph);
        }

        private void HandleErrorRecorded(object sender, ErrorRecordedEventArgs e)
        {
            foreach (var errorRecord in e.NewErrorRecords)
            {
                if (!_errorViewModel.HasView)
                    NewHiddenErrorRecordsCount++;
            }
        }
        
        private bool CanOpenErrorWindow() => !_errorViewModel.HasView;
        private void OpenErrorWindow()
        {
            _errorViewFactory.CreateWindow(_errorViewModel as WindowViewModel);
            NewHiddenErrorRecordsCount = 0;
        }

        private bool CanCloseAllGraph() => _subWindows.Where(sw => sw is GraphWindowViewModel).Any(gw => gw.HasView);
        private void CloseAllGraph()
        {
            _subWindows
                .Where(sw => sw is GraphWindowViewModel)
                .ToList()       //fixing the source collection before closing graph windows
                .ForEach<WindowViewModel>(gw =>
            {
                if (gw.CloseViewCommand.CanExecute(null))
                    gw.CloseViewCommand.Execute(null);
            });
        }

        void IWindowRegistrator.RegisterWindow(WindowViewModel viewModel)
        {
            if(!_subWindows.Contains(viewModel))
                _subWindows.Add(viewModel);
        }

        void IWindowRegistrator.UnregisterWindow(WindowViewModel viewModel)
        {
            if (_subWindows.Contains(viewModel))
                _subWindows.Remove(viewModel);
        }
    }
}
