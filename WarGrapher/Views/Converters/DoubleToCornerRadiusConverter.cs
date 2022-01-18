using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WarGrapher.Views.Converters
{
    class DoubleToCornerRadiusConverter : IValueConverter
    {
        /// <summary>
        /// Converts a double number to a <see cref="CornerRadius"/> object depending on a parameter value.
        /// </summary>
        /// <param name="value">The double number that specifies the value of radii of a output <see cref="CornerRadius"/> object.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">The string parameter that might have two possible values: "top" or "bottom". That defines which of redii will apply to output.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>
        /// New <see cref="CornerRadius"/> object that is compiled by the parameter value.
        /// Or a <see cref="CornerRadius"/> object with same values of radii equal to the source double number if the parameter is not specified or has an invalid value.
        /// Or a <see cref="CornerRadius"/> obkect with zero values of radii if the source value is not a double.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Double)
            {
                double corner = (double)value;
                string side = (parameter as System.String)?.ToLower();
                switch (side)
                {
                    case "top":
                        return new CornerRadius(corner, corner, 0, 0);
                    case "bottom":
                        return new CornerRadius(0, 0, corner, corner);
                    default:
                        return new CornerRadius(corner);
                }
            }
            else
                return new CornerRadius(0);
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
