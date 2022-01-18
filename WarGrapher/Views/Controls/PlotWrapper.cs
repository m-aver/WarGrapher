using OxyPlot;
using OxyPlot.Wpf;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Series = WarGrapher.ViewModels.GraphWindowViewModel.Series;

namespace WarGrapher.Views.Controls
{
    /// <summary>
    /// Serves as a wrapper over the <see cref="OxyPlot.Wpf.Plot.Series"/> property which is not accesible for binging by default.
    /// Converts <see cref="WarGrapher.ViewModels.GraphWindowViewModel.Series"/> to <see cref="OxyPlot.Wpf.LineSeries"/> as well.
    /// </summary>
    class PlotWrapper : Plot
    {
        public static DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(PlotWrapper.Series),
                typeof(IEnumerable<Series>),
                typeof(PlotWrapper),
                new UIPropertyMetadata(SeriesChanged)
                );

        // Chart series for binding from XAML
        new public IEnumerable<Series> Series
        {
            get { return ((IEnumerable<Series>)(base.GetValue(SeriesProperty))); }
            set { base.SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="Series"/> property.
        /// Assigns the handler of source collection changes to track changes of its items structure 
        /// (if the source collection implements the <see cref="INotifyCollectionChanged"/> interface)
        /// and keep the appropriate structure on the <see cref="OxyPlot.Wpf.Plot.Series"/> property behind.
        /// </summary>
        private static void SeriesChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            Plot plot = depObj as Plot;
            plot.Series.Clear();            

            //the action that will execute every time the collection at the Series property is changed
            var action = new NotifyCollectionChangedEventHandler(
                    (o, e) =>
                    {
                        if (plot != null &&
                            o is IEnumerable<Series>)
                        {
                            switch (e.Action)
                            {
                                case NotifyCollectionChangedAction.Add:
                                    AddSeriesToPlot(plot, e.NewItems.Cast<Series>());
                                    break;
                                case NotifyCollectionChangedAction.Remove:      
                                    plot.Series.Clear();
                                    AddSeriesToPlot(plot, e.OldItems.Cast<Series>());
                                    break;
                                case NotifyCollectionChangedAction.Reset:
                                    plot.Series.Clear();
                                    break;
                                default:
                                    break;
                            }

                            plot.ActualModel.InvalidatePlot(true);
                        }
                    });

            if (args.OldValue is INotifyCollectionChanged)
            {
                var oldCollection = (INotifyCollectionChanged)args.OldValue;
                oldCollection.CollectionChanged -= action;   // Unsubscribe from CollectionChanged on the old collection
            }

            if (args.NewValue != null)
            {
                if(args.NewValue is INotifyCollectionChanged)
                {
                    var newCollection = (INotifyCollectionChanged)args.NewValue;
                    newCollection.CollectionChanged += action;   // Subscribe to CollectionChanged on the new collection
                }

                //add initial series from the source collection
                AddSeriesToPlot(plot, (IEnumerable<Series>)args.NewValue);                             
            }

            plot.ActualModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Adds series to the original <see cref="OxyPlot.Wpf.Plot.Series"/> property to display them on the plot.
        /// </summary>
        private static void AddSeriesToPlot(Plot plot, IEnumerable<Series> seriesCollection)
        {
            foreach (Series series in seriesCollection)
            {
                plot.Series.Add(
                   new LineSeries()
                   {
                       Title = series.Title,
                       ItemsSource = ConvertToDataPoints(series.Points),
                   });
            }
        }

        private static IEnumerable<DataPoint> ConvertToDataPoints(IEnumerable<Point> points)
        {
            return points.Select(point => new DataPoint(point.X, point.Y));
        }
    }
}
