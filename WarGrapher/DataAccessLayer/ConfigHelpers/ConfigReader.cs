using System;
using System.Collections.Generic;
using System.Configuration;
using WarGrapher.Models.Equipment;

namespace WarGrapher.DataAccessLayer.ConfigHelpers
{
    /// <summary>
    /// Provides methods for getting custom configuration elements.
    /// </summary>
    internal static class ConfigReader
    {
        /// <summary>
        /// Defines possible string keys for resources location elements in the config file
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, string> _typedResourceKeys
            = new Dictionary<Type, string>()
            {
                [typeof(Weapon)] = "weapons",
                [typeof(HeadArmor)] = "helmets",
                [typeof(BodyArmor)] = "vests",
                [typeof(ArmsArmor)] = "gloves",
                [typeof(LegsArmor)] = "shoes",
            };

        /// <summary>
        /// Retrieves from the configuration file the collection of <see cref="ResourceElement"/> associated with a type of equipment
        /// </summary>
        /// <exception cref="ConfigurationException">Occurs when the section of the resource locations has an incorrect format</exception>
        /// <exception cref="ConfigurationErrorsException">Occurs when one of the necessaries resource keys was not found in the config file</exception>
        public static IReadOnlyDictionary<Type, ResourceElement> GetResourceElements()
        {
            var output = new Dictionary<Type, ResourceElement>();
            foreach (var item in _typedResourceKeys)
            {
                output.Add(item.Key, GetResourceElementForType(item.Key));
            }
            return output;
        }

        /// <summary>
        /// Retrieves from the configuration file the column separator of resource csv files
        /// </summary>
        /// <exception cref="ConfigurationException">Occurs when the section of the csv file settings has an incorrect format</exception>
        /// <exception cref="ConfigurationErrorsException">Occurs when the separator specified in the config file is not valid</exception>
        public static char GetColumnSeparator()
        {
            CsvSettingsConfigSection section = (CsvSettingsConfigSection)ConfigurationManager.GetSection("CsvFileSettings");
            return section.ColumnSeparator;
        }

        /// <summary>
        /// Retrieves from the configuration file the decimal separator of numerical parameters 
        /// </summary>
        /// <exception cref="ConfigurationException">Occurs when the section of the csv file settings has an incorrect format</exception>
        /// <exception cref="ConfigurationErrorsException">Occurs when the separator specified in the config file is not valid</exception>
        public static char GetDecimalSeparator()
        {
            CsvSettingsConfigSection section = (CsvSettingsConfigSection)ConfigurationManager.GetSection("CsvFileSettings");
            return section.DecimalSeparator;
        }

        private static ResourceElement GetResourceElementForType(Type type)
        {
            string key;
            if (_typedResourceKeys.ContainsKey(type))
            {
                key = _typedResourceKeys[type];
            }
            else
            {
                throw new ArgumentException("the passed type is not supported", nameof(type));
            }

            ResourcesLocationConfigSection section = (ResourcesLocationConfigSection)ConfigurationManager.GetSection("ResourcesLocation");
            ResourceElement element = section.ResourceItems[key];
            if (element != null)
            {
                return element;
            }
            else
            {
                throw new ConfigurationErrorsException($"the key <{key}> was not found in the config file");
            }
        }
    }
}
