using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WarGrapher.Models.Equipment
{
    /// <summary>
    /// Represents the base class for an equipment element.
    /// </summary>
    public abstract class EquipItem
    {
        /// <summary>
        /// Gets the name of an item.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the icon of an item.
        /// </summary>
        public BitmapImage Icon { get; }
        /// <summary>
        /// Gets the collection of names and values of item parameters.
        /// </summary>
        protected Dictionary<string, double> paramBase { get; }

        /// <summary>
        /// Occurs when the requested parameter was not found.
        /// </summary>
        public event EventHandler<ParamNotFoundEventArgs> ParamNotFound;

        protected EquipItem(string name, BitmapImage icon, Dictionary<string, double> paramBase)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (icon == null) throw new ArgumentNullException(nameof(icon));
            if (paramBase == null) throw new ArgumentNullException(nameof(paramBase));

            this.Name = name;
            this.Icon = icon;
            this.paramBase = paramBase;
        }

        /// <summary>
        /// Retrieves the value of a parameter with the passed name
        /// </summary>
        /// <param name="paramName">The name of the requested parameter</param>
        /// <returns>The value of a parameter or null if the parameter was not found.</returns>
        public double? GetParam(string paramName)
        {
            if (paramBase.ContainsKey(paramName))
            {
                return paramBase[paramName];
            }
            else
            {
                OnParamNotFound(paramName);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all parameter's names of this equipment item
        /// </summary>
        public IEnumerable<string> GetParamNames() => paramBase.Keys;

        /// <summary>
        /// Compares two equipment item by their name and type
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return
                (obj as EquipItem)?.Name == this.Name &&
                obj.GetType() == this.GetType();
        }
        public sealed override int GetHashCode()
        {
            unchecked 
            { 
                int hash = 13;
                hash = (hash * 7) + this.Name.GetHashCode();
                hash = (hash * 7) + this.GetType().GetHashCode();
                return hash;
            }
        }


        protected virtual void OnParamNotFound(string paramName)
        {
            EventHandler<ParamNotFoundEventArgs> paramNotFoundHandler = ParamNotFound;
            paramNotFoundHandler?.Invoke(this, new ParamNotFoundEventArgs(paramName));
        }
    }

    public class ParamNotFoundEventArgs : EventArgs
    {
        public string ParamName { get; }

        public ParamNotFoundEventArgs(string paramName)
        {
            ParamName = paramName;
        }
    }
}
