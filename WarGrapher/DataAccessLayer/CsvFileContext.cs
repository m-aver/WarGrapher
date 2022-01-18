using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;
using WarGrapher.DataAccessLayer.ConfigHelpers;
using WarGrapher.Models.Equipment;

namespace WarGrapher.DataAccessLayer
{
    /// <summary>
    /// Provides access to equipment data located in external CSV-files.
    /// </summary>
    class CsvFileContext : IEquipmentDataContext
    {
        private HashSet<EquipItem> _allData;

        /// <summary>
        /// Retrieves once all possible equipment from source files specified in the configuration file.
        /// </summary>
        /// <returns> 
        /// The collection of equipment items compiled in the first call or the empty collection if there was an exception thrown.
        /// </returns>
        /// <exception cref="FileNotFoundException">Occurs when an external source file (.csv data file or image file) is not found.</exception>
        /// <exception cref="FileFormatException">Occurs when data in the source csv-file does not match the required format.</exception>
        /// <exception cref="System.Configuration.ConfigurationException">Occurs when the config file has an incorrect format.</exception>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Occurs when data in the custom sections of the config file has an incorrect format.</exception>
        public IReadOnlyCollection<EquipItem> GetAllEquipment()
        {
            if (_allData == null)
            {
                try
                {
                    IReadOnlyDictionary<Type, ResourceElement> configElements = ConfigReader.GetResourceElements();
                    IReadOnlyCollection<ResourcePaths> sources = configElements.Select(
                        item => new ResourcePaths(
                            item.Value.PathToImagesDirectory,
                            item.Value.PathToDataFile,
                            item.Key)).ToArray();

                    _allData = ReadData(sources);
                }
                catch
                {
                    _allData = new HashSet<EquipItem>();    //FIXIT (SOLVED): 1 исключение - 8 сообщений об ошибке: по 1 на каждый элемент ввода
                    throw;
                }
            }

            return _allData.ToArray();
        }

        /// <summary>
        /// Retrieves all typed equipment that are available from the source files specified in the passed <see cref="ResourcePaths"/> objects.
        /// </summary>
        /// <param name="sources">The collection of <see cref="ResourcePaths"/> that provides paths to a typed set of equipment items.</param>
        /// <returns>The set of equipment elements.</returns>
        /// <exception cref="FileNotFoundException">Occurs when an external source file (.csv data file or image file) is not found.</exception>
        /// <exception cref="FileFormatException">Occurs when the data in csv-file does not match the required format.</exception>
        private HashSet<EquipItem> ReadData(IReadOnlyCollection<ResourcePaths> sources)
        {
            var output = new HashSet<EquipItem>();

            foreach (var itemsSource in sources)
            {
                var csvFilePath = new Uri(itemsSource.CsvDataFilePath, UriKind.Absolute).LocalPath;

                //validation                
                if (!File.Exists(csvFilePath))
                    throw new FileNotFoundException("the file was not found at the path: " + csvFilePath);

                int nameColumnNumber = -1;
                int filenameColumnNumber = -1;
                string[] labelFields = null;

                //parsing
                using (TextFieldParser parser = new TextFieldParser(csvFilePath)
                {
                    Delimiters = new string[] { ConfigReader.GetColumnSeparator().ToString() }
                })
                {
                    if (parser.LineNumber == 1)     //счетчик строк начинается с 1
                    {
                        labelFields = parser.ReadFields();
                        nameColumnNumber = labelFields.Select((s) => s.ToLower()).ToList().IndexOf("name");
                        filenameColumnNumber = labelFields.Select((s) => s.ToLower()).ToList().IndexOf("filename");

                        //validation
                        if (nameColumnNumber == -1)
                            throw new FileFormatException(
                                "the data file does not match the required format: the column <name> is missing"
                                + "\n" + "file: " + csvFilePath);
                        if (filenameColumnNumber == -1)
                            throw new FileFormatException(
                                "the data file does not match the required format: the column <filename> is missing"
                                + "\n" + "file: " + csvFilePath);
                        if (labelFields.Distinct().Count() != labelFields.Length)
                            throw new FileFormatException(
                                "the data file shouldn't contain columns with same names"
                                + "\n" + "file: " + csvFilePath);
                    }

                    //row handling
                    while (!parser.EndOfData)
                    {
                        long currentLine = parser.LineNumber;
                        string[] fields = parser.ReadFields();

                        //validation
                        if (fields.Length != labelFields.Length)
                            throw new FileFormatException(
                                "the data file has an incorrect structure: "
                                + $"the amount of columns in row {currentLine} does not match the total number of columns"
                                + "\n" + "file: " + csvFilePath);

                        //read params
                        var paramSet = new Dictionary<string, double>();
                        int colNum = 0;
                        foreach (var field in fields)
                        {
                            double param;
                            if (TryParseParam(field, out param))
                                paramSet.Add(labelFields[colNum], param);
                            colNum++;
                        }
                        string name = fields[nameColumnNumber];
                        BitmapImage icon = GetImageFromExternalDirectory(itemsSource.ImagesDirectory, fields[filenameColumnNumber]);

                        //creating an instance of EquipItem, adding it to the output collection
                        EquipItem item = Activator.CreateInstance(itemsSource.ItemsType, name, icon, paramSet) as EquipItem;
                        if (!output.Contains(item))
                        {
                            output.Add(item);
                        }
                        else
                        {
                            throw new FileFormatException(
                                $"A duplicate item was detected in row {currentLine}. "
                                + "Make sure that item has an unique name."
                                + "\n" + "file: " + csvFilePath);
                        }
                    }
                }
            }
            return output;
        }

        private BitmapImage GetImageFromExternalDirectory(string directoryAddress, string imageFileName)
        {
            var imageAbsolutePath = directoryAddress + imageFileName;
            var imageAbsoluteUri = new Uri(imageAbsolutePath, UriKind.Absolute);

            if (!File.Exists(imageAbsolutePath))
                throw new FileNotFoundException("the image file was not found at the path: " + imageAbsoluteUri.LocalPath);

            BitmapImage bitmapImage = null;
            try
            {
                bitmapImage = new BitmapImage(imageAbsoluteUri);
            }
            catch(Exception ex)
            {
                throw new FileFormatException("the image file can't be read at the path: " + imageAbsoluteUri.LocalPath, ex);
            }
            return bitmapImage;
        }

        static private bool TryParseParam(string paramString, out double result)
        {
            var format = new NumberFormatInfo() { CurrencyDecimalSeparator = ConfigReader.GetDecimalSeparator().ToString() };
            var style = NumberStyles.Float;
            return Double.TryParse(paramString, style, format, out result);
        }


        /// <summary>
        /// Defines a kit of paths to the locations of typed equipment resouces.
        /// </summary>
        private struct ResourcePaths
        {
            /// <summary>
            /// Gets the type that represents a specific class of equipment and allows to create an instance of equipment item.
            /// </summary>
            public Type ItemsType { get; }
            /// <summary>
            /// Gets the absolute path to the item images directory.
            /// </summary>
            public string ImagesDirectory { get; }
            /// <summary>
            /// Gets the absolute path to the csv file of items data.
            /// </summary>
            public string CsvDataFilePath { get; }

            /// <summary>
            /// Create a new <see cref="ResourcePaths"/> structure.
            /// </summary>
            /// <param name="imagesDirectory">The relative path to the items images directory</param>
            /// <param name="csvDataFilePath">The relative path to the items csv data file</param>
            /// <param name="itemsType">The type of equipment items that correspond to the specified paths</param>
            /// <exception cref="ArgumentException">It's thrown if the type of items is not an equipment type that accessible for creation an instance</exception>
            /// <exception cref="ArgumentNullException">It's thrown if one of the arguments is null</exception>
            public ResourcePaths(string imagesDirectory, string csvDataFilePath, Type itemsType)
            {
                if (imagesDirectory == null) throw new ArgumentNullException(nameof(imagesDirectory));
                if (csvDataFilePath == null) throw new ArgumentNullException(nameof(csvDataFilePath));
                if (itemsType == null) throw new ArgumentNullException(nameof(itemsType));

                ImagesDirectory = App.AppFolder + HandlePathSlashes(imagesDirectory, true);
                CsvDataFilePath = App.AppFolder + HandlePathSlashes(csvDataFilePath);

                if (typeof(EquipItem)
                    .IsAssignableFrom(itemsType) &&
                    !itemsType.IsAbstract &&
                    itemsType.IsPublic &&
                    null != itemsType.GetConstructor(
                        bindingAttr: BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public,
                        binder: null,
                        types: new Type[3] { typeof(string), typeof(BitmapImage), typeof(Dictionary<string, double>) },
                        modifiers: null))
                {
                    ItemsType = itemsType;
                }
                else
                {
                    throw new ArgumentException(
                        $"{nameof(itemsType)} must be derived from {nameof(EquipItem)} and accessible for creation an instance");
                }
            }

            private static string HandlePathSlashes(string path, bool appendSlashToPathEnd = false)
            {
                StringBuilder str = new StringBuilder(path.Replace('/', '\\'));
                if (str.ToString().First() != '\\')
                {
                    str.Insert(0, '\\');
                }
                if (appendSlashToPathEnd &&
                    str.ToString().Last() != '\\')
                {
                    str.Append('\\');
                }
                return str.ToString();
            }
        }
    }
}
