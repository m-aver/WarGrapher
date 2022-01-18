using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WarGrapher.Views.Converters
{
    class ColorOpacityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a passed color by changing its opacity as specified in the parameter
        /// </summary>
        /// <param name="value">Color object to transform.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Target opacity as a string or number in the range of [0.0 - 1.0]</param>
        /// <param name="culture">A culture of the target opacity parameter string for parsing its to number</param>
        /// <returns>
        /// Color object with the transformed A-channel or source object if passed value is not a Color or the parameter parsing is failed.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double opacity;

            if (value is Color &&
                Double.TryParse(
                   parameter?.ToString(),
                   NumberStyles.Float,
                   culture ?? CultureInfo.GetCultureInfo("EN-US"),
                   out opacity) &&
                opacity >= 0 &&
                opacity <= 1)
            {
                byte aChannel = (byte)(Byte.MaxValue * opacity);

                Color color = (Color)value;
                color.A = aChannel;
                return color;
            }
            else
            {
                return value;
            }
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
