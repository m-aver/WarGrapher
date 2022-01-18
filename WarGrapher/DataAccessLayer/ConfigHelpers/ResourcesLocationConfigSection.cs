using System.Configuration;

namespace WarGrapher.DataAccessLayer.ConfigHelpers
{
    /// <summary>
    /// Represents the custom configuration section that allows to specify locations of resource files.
    /// </summary>
    internal class ResourcesLocationConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("TypedResources")]
        public TypedResourcesCollection ResourceItems
        {
            get { return ((TypedResourcesCollection)(base["TypedResources"])); }
        }
    }

    /// <summary>
    /// Represents the collection of <see cref="ResourceElement"/> containing paths to data of equipment items those grouped by their types 
    /// </summary>
    [ConfigurationCollection(typeof(ResourceElement))]
    internal class TypedResourcesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ResourceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ResourceElement)(element)).TypeCode;
        }

        public ResourceElement this[int idx]
        {
            get { return (ResourceElement)BaseGet(idx); }
        }

        new public ResourceElement this[string key]
        {
            get { return (ResourceElement)BaseGet(key); }
        }
    }

    /// <summary>
    /// Represent a configuration element containing paths to one-type equipment data
    /// </summary>
    internal class ResourceElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets a string key that represents a type of equipment accessibled by this <see cref="ResourceElement"/>
        /// </summary>
        [ConfigurationProperty("typeCode", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string TypeCode
        {
            get { return ((string)(base["typeCode"])); }
            set { base["typeCode"] = value; }
        }

        /// <summary>
        /// Gets or sets a relative path to the CSV file with equipment data
        /// </summary>
        [ConfigurationProperty("pathToDataFile", DefaultValue = "", IsKey = false, IsRequired = true)]
        [StringValidator(InvalidCharacters = ":*?\"<>|")]
        public string PathToDataFile
        {
            get { return ((string)(base["pathToDataFile"])); }
            set { base["pathToDataFile"] = value; }
        }

        /// <summary>
        /// Gets or sets a relative path to the directory of equipment icons
        /// </summary>
        [ConfigurationProperty("pathToImagesDirectory", DefaultValue = "", IsKey = false, IsRequired = true)]
        [StringValidator(InvalidCharacters = ":*?\"<>|")]
        public string PathToImagesDirectory
        {
            get { return ((string)(base["pathToImagesDirectory"])); }
            set { base["pathToImagesDirectory"] = value; }
        }
    }
}
