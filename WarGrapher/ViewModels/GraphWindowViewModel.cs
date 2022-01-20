using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WarGrapher.Common;
using WarGrapher.Models;
using WarGrapher.Models.Calculation;
using WarGrapher.Models.Calculation.Utility;
using WarGrapher.ViewModels.Commands;
using WarGrapher.ViewModels.ViewFactories;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Represents an entity that uses features of a plot calculation object.
    /// </summary>
    interface IPlotCalculator
    {
        PlotDataCalculation Calculation { get; set; }
    }

    /// <summary>
    /// Represents the view-model of a chart window.
    /// </summary>
    class GraphWindowViewModel : WindowViewModel, IObserver, IPlotCalculator
    {
        /// <summary>
        /// Gets the observable collection that contains data of chart series. This property is intended for binding to a view.
        /// </summary>
        public ObservableCollection<Series> DataSeries { get; }

        private GraphInfo _graphData;
        /// <summary>
        /// Gets additional data about a chart represented by this view-model. This property is intended for binding to a view.
        /// </summary>
        public GraphInfo GraphData
        {
            get { return _graphData; }
            private set
            {
                _graphData = value;
                OnPropertyChanged(nameof(GraphData));
            }
        }

        private bool _isSubscribe;
        /// <summary>
        /// Gets a value that indicates whether this view-model is subscribed to updates of the application data model.
        /// This determines whether the view-model responds to changing of user-selected data.
        /// This property is intended for binding to a view.
        /// </summary>
        public bool IsSubscribe
        {
            get { return _isSubscribe; }
            private set
            {
                _isSubscribe = value;
                OnPropertyChanged(nameof(IsSubscribe));
            }
        }

        /// <summary>
        /// Gets the command that executes subscibing of this view-model to the application data model.
        /// </summary>
        public ICommand SubscribeGraphCommand { get; }
        /// <summary>
        /// Gets the command that executes unsubscibing of this view-model from the application data model.
        /// </summary>
        public ICommand UnsubscribeGraphCommand { get; }

        private PlotDataCalculation _calculation;
        /// <summary>
        /// Gets or sets a plot calculation object that provides data of chart series to this view-model.
        /// </summary>
        [Bindable(false)]
        public PlotDataCalculation Calculation
        {
            get { return _calculation; }
            set
            {
                _calculation = value;
                RefreshSeries();
                RefreshGraphData(value.ChartInfo);
            }
        }

        private readonly WindowFactory _viewFactory;
        private readonly IWindowRegistrator _mainViewModel;
        private readonly IObservable _model;
        private readonly IErrorRecorder _errorViewModel;

        public GraphWindowViewModel()
        {
            DataSeries = new ObservableCollection<Series>();
            IsSubscribe = false;

            SubscribeGraphCommand = new RelayCommand(
                execute: Subscribe,
                canExecute: CanSubscribe);
            UnsubscribeGraphCommand = new RelayCommand(
                execute: Unsubscribe,
                canExecute: CanUnsubscribe);

            _viewFactory = new MainWindowFactory();
            _mainViewModel = (IWindowRegistrator)_viewFactory.GetCommonViewModel();

            _viewFactory = new ErrorWindowFactory();
            _errorViewModel = (IErrorRecorder)_viewFactory.GetCommonViewModel();

            _model = ModelFactory.ModelInstance;

            if (SubscribeGraphCommand.CanExecute(null))
                SubscribeGraphCommand.Execute(null);
        }

        void IObserver.Update()
        {
            RefreshSeries();
        }

        protected override void SetupViewHook()
        {
            _mainViewModel.RegisterWindow(this);
        }

        protected override void CloseViewHook()
        {
            _mainViewModel.UnregisterWindow(this);

            if (UnsubscribeGraphCommand.CanExecute(null))
                UnsubscribeGraphCommand.Execute(null);
        }

        private void RefreshSeries()
        {
            DataSeries.Clear();

            var plotPoints = new Dictionary<string, List<Point>>();
            try
            {
                if (_calculation != null)
                    plotPoints = _calculation.GetPlotData();
            }
            catch (NoEquipmentDataException ex)
            {
                string message = "not enought equipment data, make sure that you chose all required items of these types: " + ex.EquipmentTypes;
                _errorViewModel.SendError(ErrorType.UserError, message);
            }
            catch (CalculationException ex)
            {
                _errorViewModel.SendError(ErrorType.DesignError, ex);
            }

            foreach (var pointsSerie in plotPoints)
            {
                Series lineSerie = new Series(
                    title: pointsSerie.Key,
                    points: pointsSerie.Value);

                DataSeries.Add(lineSerie);
            }
        }

        private void RefreshGraphData(CalculationInfo calcInfo)
        {
            GraphData = new GraphInfo()
            {
                GraphLabel = calcInfo.ChartName,
                XLabel = calcInfo.XAxisName + (calcInfo.XAxisUnit == null ? String.Empty : $" (" + calcInfo.XAxisUnit + ")"),
                YLabel = calcInfo.YAxisName + (calcInfo.YAxisUnit == null ? String.Empty : $" (" + calcInfo.YAxisUnit + ")")
            };
        }

        private bool CanSubscribe() => !IsSubscribe;
        private void Subscribe()
        {
            _model.RegisterObserver(this);
            IsSubscribe = true;

            RefreshSeries();
        }

        private bool CanUnsubscribe() => IsSubscribe;
        private void Unsubscribe()
        {
            _model.RemoveObserver(this);
            IsSubscribe = false;
        }


        /// <summary>
        /// Represents one series of a chart.
        /// </summary>
        public struct Series
        {
            public string Title { get; }
            public IReadOnlyCollection<Point> Points { get; }

            public Series(string title, IReadOnlyCollection<Point> points)
            {
                Title = title;
                Points = points;
            }
        }

        /// <summary>
        /// Represents a kit of general data about a chart.
        /// </summary>
        public struct GraphInfo
        {
            public string XLabel { get; internal set; }
            public string YLabel { get; internal set; }
            public string GraphLabel { get; internal set; }
        }
    }
}
