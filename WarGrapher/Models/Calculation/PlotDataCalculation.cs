using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using WarGrapher.Common;
using WarGrapher.Models.Calculation.Utility;
using WarGrapher.Models.Equipment;

namespace WarGrapher.Models.Calculation
{
    /// <summary>
    /// Serves as the base class for a specific implementation of the calculation.
    /// </summary>
    public abstract class PlotDataCalculation
    {
        /// <summary>
        /// Gets a info about the calculation from derived classes.
        /// </summary>
        public abstract CalculationInfo ChartInfo { get; }

        /// <summary>
        /// Gets data of the user-selected weapons.
        /// </summary>
        protected IReadOnlyCollection<Weapon> weaponsData { get; private set; }
        /// <summary>
        /// Gets data of the user-selected vest.
        /// </summary>
        protected BodyArmor bodyArmorData { get; private set; }
        /// <summary>
        /// Gets data of the armor that associated to the user-selected part of the person hitbox.
        /// </summary>
        protected Armor focusedBodyPartArmorData { get; private set; }
        /// <summary>
        /// Gets the user-selected targed part of the person hitbox.
        /// </summary>
        protected BodyPart focusedBodyPart { get; private set; }

        private readonly ISelectedDataConsumer _model;
        private readonly EquipType[] _requiredEquipment;

        public PlotDataCalculation()
        {
            _model = ModelFactory.ModelInstance;

            _requiredEquipment =
                this.GetType().GetCustomAttribute<RequiredEquipmentAttribute>()?
                .EquipmentTypes.GetFlags().Cast<EquipType>().ToArray()
                ?? Enum.GetValues(typeof(EquipType)).Cast<EquipType>().ToArray();
        }

        /// <summary>Calculates the plot data in derived classes.</summary>
        /// <returns>The dictionary that contains named data of chart series</returns>
        protected abstract Dictionary<string, List<Point>> ExecuteCalculation();

        /// <summary>Calculates data for a chart.</summary>
        /// <returns>The dictionary that contains named data of chart series</returns>
        /// <exception cref="CalculationException"/>
        /// <exception cref="NoEquipmentDataException"/>
        internal Dictionary<string, List<Point>> GetPlotData()
        {
            RequestEquipmentData();

            Dictionary<string, List<Point>> calculationResult = null;
            try
            {
                calculationResult = ExecuteCalculation();
            }
            catch (Exception ex)
            {
                throw new CalculationException("a calculation time error", ex);
            }

            return calculationResult;
        }

        /// <summary>
        /// Requests data of selected equipments from the application data model
        /// </summary>
        /// <exception cref="NoEquipmentDataException"/>
        private void RequestEquipmentData()
        {
            weaponsData = _model.GetSelectedDataOfType(EquipType.Weapon).Cast<Weapon>().Distinct().ToArray();
            bodyArmorData = _model.GetSelectedDataOfType(EquipType.BodyArmor).Cast<BodyArmor>().FirstOrDefault();
            focusedBodyPartArmorData = _model.GetFocusedBodyPartData().FirstOrDefault();
            focusedBodyPart = _model.FocusedBodyPart;

            //validation
            EquipType? missingEquipment = null;
            if (bodyArmorData == null &&
                _requiredEquipment.Contains(EquipType.BodyArmor))
                missingEquipment = ConcatenateEquipment(missingEquipment, EquipType.BodyArmor);
            if (focusedBodyPartArmorData == null &&
                _requiredEquipment.Contains(focusedBodyPart.CastToEquipment()))
                missingEquipment = ConcatenateEquipment(missingEquipment, focusedBodyPart.CastToEquipment());
            if (weaponsData == null &&
                _requiredEquipment.Contains(EquipType.Weapon))
                missingEquipment = ConcatenateEquipment(missingEquipment, EquipType.Weapon);
            if (missingEquipment != null)
                throw new NoEquipmentDataException("equipment data is not received", missingEquipment.Value);
        }

        private EquipType ConcatenateEquipment(EquipType? main, EquipType sub)
        {
            if (main == null)
                return sub;
            else
                return main.Value | sub;                 
        }
    }
}
