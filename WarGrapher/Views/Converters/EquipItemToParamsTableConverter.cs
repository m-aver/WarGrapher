using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using WarGrapher.ViewModels;

namespace WarGrapher.Views.Converters
{
    /// <remarks>
    /// Used in <see cref="Controls.EquipmentDescriptionControl"/>
    /// </remarks>
    class EquipItemToParamsTableConverter : IValueConverter
    {
        /// <summary>
        /// Converts an equipment item to a list of <see cref="ItemParam"/> containing the names and values of available item parameters.
        /// </summary>
        /// <param name="value">An <see cref="EquipItemViewModel"/> object to convert.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>
        /// Enumerable collection of <see cref="ItemParam"/> or the empty collection if passed value is not a <see cref="EquipItemViewModel"/> 
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EquipItemViewModel)
            {
                var equipItem = (EquipItemViewModel)value;

                IEnumerable<ItemParam> @params = new List<ItemParam>(
                    equipItem.GetParamNames().Select(
                    pn => new ItemParam(pn, equipItem.GetParam(pn))
                    ));

                #region REMARK
                //С помощью анонимных объектов тоже работает
                /*
                IEnumerable<object> @params = new List<object>(
                    equipItem.GetParamNames().Select(
                    pn => new { Name = pn, Value = equipItem.GetParam(pn) }
                    ));
                */
                #endregion

                return @params;
            }
            else
            {
                return new ItemParam[0];
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region REMARK
        //можно было бы использовать анонимные объекты и не плодить лишний класс
        //хотя конкретно здесь пожалуй отдельный класс к месту - сразу видно к каким полям будет сделана привязка в таблице 
        #endregion
        private struct ItemParam
        {
            public string Name { get; }
            public double? Value { get; }

            public ItemParam(string name, double? value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
