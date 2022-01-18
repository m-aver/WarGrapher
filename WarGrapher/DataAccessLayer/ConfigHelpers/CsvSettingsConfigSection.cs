using System;
using System.Configuration;

namespace WarGrapher.DataAccessLayer.ConfigHelpers
{
    /// <summary>
    /// Represents the config section that defines settings for reading csv data files
    /// </summary>
    internal class CsvSettingsConfigSection : ConfigurationSection
    {
        //The configuration attribute names
        private const string ColumnSeparatorPropertyName = "columnSeparator";
        private const string DecimalSeparatorPropertyName = "decimalSeparator";

        /// <summary>
        /// Gets or sets the character of column separator that will be applied on reading data
        /// </summary>
        [CsvSeparatorsValidator(ColumnSeparatorPropertyName)]
        [ConfigurationProperty(name: ColumnSeparatorPropertyName, DefaultValue = ";", IsRequired = false)]
        public char ColumnSeparator
        {
            get { return (char)base[ColumnSeparatorPropertyName]; }
            set { base[ColumnSeparatorPropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the character of decimal separator that will be applied on reading data
        /// </summary>
        [CsvSeparatorsValidator(DecimalSeparatorPropertyName, InvalidCharacters = "-0123456789")]
        [ConfigurationProperty(name: DecimalSeparatorPropertyName, DefaultValue = ",", IsRequired = false)]
        public char DecimalSeparator
        {
            get { return (char)base[DecimalSeparatorPropertyName]; }
            set { base[DecimalSeparatorPropertyName] = value; }
        }

        protected override void Init()
        {
            base.Init();
            ValidateSection();
        }

        private void ValidateSection()
        {
            if (ColumnSeparator == DecimalSeparator)
            {
                throw new ConfigurationErrorsException(
                    $"Properties {DecimalSeparatorPropertyName} and {ColumnSeparatorPropertyName}"
                    + "can't have same values");
            }
        }
    }

    /// <summary>
    /// The class that performs a validation of the configuration settings of the data separators in the target csv file
    /// </summary>
    internal sealed class CsvSeparatorsValidator : ConfigurationValidatorBase
    {
        public string PropertyName { get; }
        public string InvalidCharacters { get; }

        public CsvSeparatorsValidator(string propertyName, string invalidCharacters)
        {
            PropertyName = propertyName;
            InvalidCharacters = invalidCharacters;
        }

        public override bool CanValidate(Type type)
        {
            return type == typeof(char);
        }

        public override void Validate(object value)
        {
            char separator = (char)value;

            if (InvalidCharacters != null &&
                InvalidCharacters != String.Empty)
            {
                foreach (char invalidSymbol in InvalidCharacters)
                {
                    if (separator == invalidSymbol)
                        throw new ConfigurationErrorsException($"The property <{PropertyName}> can't have value \'{invalidSymbol}\'");
                }
            }
        }
    }

    /// <summary>
    /// The attribute that allows to define conditions of the separator validation
    /// </summary>
    internal sealed class CsvSeparatorsValidatorAttribute : ConfigurationValidatorAttribute
    {
        public string PropertyName { get; }
        public string InvalidCharacters { get; set; }

        public CsvSeparatorsValidatorAttribute(string propertyName)
        {
            PropertyName = propertyName;
            InvalidCharacters = String.Empty;
        }

        public override ConfigurationValidatorBase ValidatorInstance
        {
            get { return new CsvSeparatorsValidator(PropertyName, InvalidCharacters); }
        }
    }
}
