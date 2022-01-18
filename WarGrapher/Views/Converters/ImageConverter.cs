using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WarGrapher.Views.Converters
{
    class ImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="BitmapImage"/> object to an <see cref="Image"/> object.
        /// </summary>
        /// <param name="value">Source <see cref="BitmapImage"/> object.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>
        /// An <see cref="Image"/> object is constructed from a source <see cref="BitmapImage"/> object.    
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapImage)
                return new Image() { Source = (BitmapImage)value };
            else
                throw new ArgumentException($"Converting value should have {typeof(BitmapImage).Name} type", nameof(value));
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
