using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WarGrapher.Views.Converters
{
    class DoubleToThicknessConverter : IValueConverter
    {
        /// <summary>
        /// Converts a double value to a <see cref="Thickness"/> object according to the passed parameter.
        /// </summary>
        /// <param name="value">A double number that specifies value of the sides for an outcome <see cref="Thickness"/> object.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">A string corresponds to the format "x;x;x;x" where x = 0 or 1 that specifies whether a side will be involved or ignored for output.</param>
        /// <param name="culture">A culture for parsing numbers from the parameter.</param>
        /// <returns>
        /// New <see cref="Thickness"/> object is composed by specified sides in the parameter.
        /// Or a <see cref="Thickness"/> object with same values of the sides if the parameter does not corresponds to the required format or parsing is failed.
        /// Or a <see cref="Thickness"/> object with the zero sides if the income value is not a <see cref="double"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Double)
            {
                double thickness = (double)value;
                string[] sides = (parameter as System.String)?.Split(';');
                List<double> sizes = new List<double>(4);

                if (sides != null &&
                    sides.Length == 4 &&
                    sides.All(
                        side =>
                        {
                            int size;
                            bool luck = Int32.TryParse(side, NumberStyles.None, culture ?? CultureInfo.GetCultureInfo("en-US"), out size);
                            luck = luck && (size == 1 || size == 0);
                            sizes.Add(size * thickness);
                            return luck;
                        })
                    )
                {
                    return new Thickness(sizes[0], sizes[1], sizes[2], sizes[3]);
                }
                else
                    return new Thickness(thickness);
            }
            else
                return new Thickness(0);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    //попытка обобщить DoubleToThicknessConverter на использование с другими параметрами, отвечающими за настройку формы прямоугольника
    //например: CornerRadius
    class DoubleToRectangleShapeTypesConverter : DoubleToThicknessConverter, IValueConverter
    {
        new public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness thickness = (Thickness)base.Convert(value, targetType, parameter, culture);

            try
            {
                return
                    Activator.CreateInstance(targetType, thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
            }
            catch (Exception ex)
            {
                throw new
                    InvalidOperationException(
                    "Attempt to use " + nameof(DoubleToRectangleShapeTypesConverter) + "with an unsupported type. " +
                    "Make sure that the type of a bounded parameter has the constructor accepting four double numbers.",
                    ex);
            }
        }

        new public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return base.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
