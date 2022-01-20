using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WarGrapher.Common;
using WarGrapher.DataAccessLayer;
using WarGrapher.Models.Equipment;

namespace WarGrapher.Models
{
    /// <summary>
    /// Represents a state of the application data (e.g. user-selected equipment items)
    /// </summary>
    class Model : IModel
    {
        /// <summary>
        /// Raises when an error has occured in the model
        /// </summary>
        public event ModelErrorEventHandler ErrorOccured;
        
        /// <summary>
        /// Gets or sets the body part that was selected by user as the weapon targed
        /// </summary>
        public BodyPart FocusedBodyPart
        {
            get { return _focusedBodyPart; }
            set
            {
                _focusedBodyPart = value;
                (this as IObservable).NotifyObservers();
            }
        }
        private BodyPart _focusedBodyPart;

        private readonly IEquipmentDataContext _dataContext;
        private readonly Dictionary<object, EquipItem> _selectedData;
        private readonly List<IObserver> _charts;

        public Model()
        {
            _dataContext = new CsvFileContext();
            _selectedData = new Dictionary<object, EquipItem>();
            _charts = new List<IObserver>();
        }

        /// <summary>
        /// Retrieves all one-type equipment available to the application 
        /// </summary>
        /// <param name="equipType">The type of the requested equipment</param>
        /// <returns>
        /// The read-only collection of one-type equipment
        /// or the empty collection if there was an error occured
        /// </returns>
        /// <exception cref="UnhandledModelException"></exception>
        /// <exception cref="System.IO.FileFormatException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.Configuration.ConfigurationException"></exception>
        /// <exception cref="System.Configuration.ConfigurationErrorsException"></exception>
        public IReadOnlyCollection<EquipItem> GetAllItemsOfType(EquipType equipType)
        {
            try
            {
                var data = _dataContext.GetAllEquipment();
                var types = ConvertEquipType(equipType);

                return GetTypedItemsFromSource(types, data).ToArray();
            }
            catch (Exception ex)
            {
                OnErrorOccured(ex);
                return new EquipItem[0];
            }
        }

        /// <summary>
        /// Retrieves one-type items of the user-selected equipment
        /// </summary>
        /// <param name="equipType">The type of the requested equipment</param>
        /// <returns>
        /// The read-only collection of the one-type user-selected equipment
        /// or the empty collection if there was an error occured
        /// </returns>
        /// <exception cref="UnhandledModelException"></exception>
        public IReadOnlyCollection<EquipItem> GetSelectedDataOfType(EquipType equipType)
        {
            try
            {
                var data = _selectedData.Values;
                var types = ConvertEquipType(equipType);

                return GetTypedItemsFromSource(types, data).ToArray();
            }
            catch(Exception ex)
            {
                OnErrorOccured(ex);
                return new EquipItem[0];
            }
        }

        /// <summary>
        /// Sets the equipment item as selected by user or removes it if null is passed
        /// </summary>
        /// <param name="senderElement">An element that provided the selected data</param>
        /// <param name="selectedItem">The user-selected item or null if user has reset the selection</param>
        /// <exception cref="ArgumentNullException">Occurs when <paramref name="senderElement"/> is null 
        ///     and if the exception is not handled by the <see cref="ErrorOccured"/> event.</exception>
        /// <exception cref="UnhandledModelException">Occurs if an error was not handled, serves as the wrapper of it</exception> 
        public void SetSelectedData(object senderElement, EquipItem selectedItem)
        {
            try
            {
                if (senderElement == null)
                    throw new ArgumentNullException(nameof(senderElement));

                //logic of adding data 
                if (selectedItem != null)
                {
                    if (_selectedData.ContainsKey(senderElement))
                        _selectedData[senderElement] = selectedItem;
                    else
                        _selectedData.Add(senderElement, selectedItem);
                }
                else
                {
                    _selectedData.Remove(senderElement);
                }
                
                //updating of charts
                (this as IObservable).NotifyObservers();
            }
            catch(UnhandledModelException)
            {
                throw;
            }
            catch (Exception ex)
            {
                OnErrorOccured(ex);
            }
        }

        /// <summary>
        /// Retrieves the user-selected equipment that associated with the user-selected targed body part
        /// </summary>
        /// <returns>
        /// The read-only collection of the user-selected armor corresponded to the targed body part 
        /// or the empty collection if there was an error occured
        /// </returns>
        /// <exception cref="UnhandledModelException"></exception>
        public IReadOnlyCollection<Armor> GetFocusedBodyPartData()
        {
            try
            {
                var equipType = _focusedBodyPart.CastToEquipment();
                return
                    GetSelectedDataOfType(equipType)
                    .OfType<Armor>()
                    .ToArray();
            }
            catch(Exception ex)
            {
                OnErrorOccured(ex);
                return new Armor[0];
            }
        }


        #region IObservable implements
        /// <summary>
        /// Notifies charts about the application data state has been changed.
        /// </summary>
        void IObservable.NotifyObservers()
        {
            try
            {
                _charts.ForEach(gw => gw.Update());
            }
            catch (Exception ex)
            {
                OnErrorOccured(ex);
            }
        }

        void IObservable.RegisterObserver(IObserver obs)
        {
            if (!_charts.Contains(obs))
                _charts.Add(obs);
        }

        void IObservable.RemoveObserver(IObserver obs)
        {
            if (_charts.Contains(obs))
                _charts.Remove(obs);
        }
        #endregion


        private IEnumerable<Type> ConvertEquipType(EquipType equipType)
        {
            return
                equipType
                .GetFlags()
                .Cast<EquipType>()
                .Select(et => ConvertSingleEquipType(et));
        }

        private Type ConvertSingleEquipType(EquipType equipType)
        {
            if (equipType.GetFlags().Length > 1)
                throw new ArgumentException("this method doesn't work with a multibits constant", nameof(equipType));

            switch (equipType)
            {
                case EquipType.Weapon: return typeof(Weapon);
                case EquipType.HeadArmor: return typeof(HeadArmor);
                case EquipType.BodyArmor: return typeof(BodyArmor);
                case EquipType.ArmArmor: return typeof(ArmsArmor);
                case EquipType.LegArmor: return typeof(LegsArmor);
                default: return typeof(EquipItem);
            }
        }

        private IEnumerable<EquipItem> GetTypedItemsFromSource(IEnumerable<Type> types, IEnumerable<EquipItem> source)
        {
            return
                types.SelectMany(
                    type => source.Where(
                        ei => type.IsInstanceOfType(ei)));
        }

        private void OnErrorOccured(Exception exc)
        {
            ModelErrorEventHandler errorOccured = ErrorOccured;
            var arg = new ModelErrorEventArgs(exc);
            errorOccured?.Invoke(this, arg);

            if (!arg.Handled)
                throw new UnhandledModelException("an unhandled model exception", arg.Exception);
        }
    }

    /// <summary>
    /// Represents an unhandled error of the application data model.
    /// </summary>
    [System.Serializable]
    internal class UnhandledModelException : Exception
    {
        public UnhandledModelException() { }
        public UnhandledModelException(string message) : base(message) { }
        public UnhandledModelException(string message, Exception innerException) : base(message, innerException) { }
        protected UnhandledModelException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
