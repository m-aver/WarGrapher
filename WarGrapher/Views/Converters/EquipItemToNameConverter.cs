using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using WarGrapher.ViewModels;

namespace WarGrapher.Views.Converters
{
    class EquipItemToNameConverter : IValueConverter
    {
        /// <summary>
        /// Converts an equipment item to a string with its name or a collection of equipment to a corresponding collection of equipment names depending on the type of a target binded property.
        /// </summary>
        /// <param name="value">An <see cref="EquipItemViewModel"/> object or a collection of <see cref="EquipItemViewModel"/> objects to convert.</param>
        /// <param name="targetType">The type of a target binded property that should be <see cref="String"/> or <see cref="IEnumerable{String}"/></param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>
        /// A string with the name of the passed equipment item.
        /// Or an iterator of the equipment names if the passed value is an equipment collection and <paramref name="targetType"/> is a <see cref="IEnumerable{String}"/>
        /// Or the empty string if the type of <paramref cref="value"/> and <paramref name="targetType"/> have invalid or inconsistent values.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EquipItemViewModel &&
                targetType == typeof(String))
            {
                return (value as EquipItemViewModel).Name;
            }
            else if (value is IEnumerable<EquipItemViewModel> &&
                    targetType == typeof(IEnumerable<String>))
            {
                return (value as IEnumerable<EquipItemViewModel>).Select(eivm => eivm.Name);
            }
            else
            {
                return String.Empty;
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
