using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using WarGrapher.Common;
using WarGrapher.Models;
using WarGrapher.Models.Calculation;
using WarGrapher.Models.Calculation.Utility;
using WarGrapher.ViewModels.Commands;
using WarGrapher.ViewModels.ViewFactories;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Represents a view-model that provides commands and properties to a view for selection of a calculation type and creating a fit chart.
    /// </summary>
    class CalculationSelectionViewModel : ElementViewModel
    {
        /// <summary>
        /// Gets the collection of available calculations to selection in a view 
        /// </summary>
        public IReadOnlyCollection<CalculationInfo> CalculationsData { get; private set; }
        /// <summary>
        /// Gets the command that executes creating a new chart window of the selected calculation type by passed <see cref="CalculationInfo"/>
        /// </summary>
        public ICommand CreateGraphCommand { get; }

        private IPlotCalculator _graphViewModel;
        private readonly IErrorRecorder _errorViewModel;
        private readonly WindowFactory _graphViewFactory;
        private readonly WindowFactory _errorViewFactory;
        private readonly ISelectedDataConsumer _model;

        public CalculationSelectionViewModel()
        {
            _model = ModelFactory.ModelInstance;
            _graphViewFactory = new GraphWindowFactory();
            _errorViewFactory = new ErrorWindowFactory();
            _errorViewModel = (IErrorRecorder)_errorViewFactory.GetCommonViewModel();

            CreateGraphCommand = new RelayCommand<CalculationInfo>(
                execute: CreateGraph,
                canExecute: CanCreateGraph);

            RetrieveCalculationTypes();
        }

        private bool CanCreateGraph(CalculationInfo calcData) =>
            calcData != null &&
            CalculationsData.Contains(calcData) &&
            calcData.RequiredEquipments.All(
                et =>
                _model.GetSelectedDataOfType(et).Count > 0);

        private void CreateGraph(CalculationInfo calcData)
        {
            PlotDataCalculation calcInstance = null;
            try
            {
                calcInstance = Activator.CreateInstance(calcData.Type) as PlotDataCalculation;
            }
            catch (Exception ex)
            {
                _errorViewModel.SendError(ErrorType.DesignError,
                    new Exception("An error occured when an instance of the calculation type was being created" + calcData.Type.Name, ex));
                return;
            }

            _graphViewModel = (IPlotCalculator)_graphViewFactory.CreateWindow();
            _graphViewModel.Calculation = calcInstance;
        }

        #region REMARK
        /*
         * извлечение типов расчета и создание экземпляра расчета со всеми вытекающими проверками
         * можно было бы инкапсулировать в специализированной фабрике
         * возможно туда же присобачить получение инфы из атрибутов
         */
        #endregion
        /// <summary>
        /// Retrieves an available calculation types and fills <see cref="CalculationsData"/> with them.
        /// </summary>
        private void RetrieveCalculationTypes()
        {
            var calcData = new List<CalculationInfo>();
            var calcTypes = typeof(PlotDataCalculation).GetAllDerivedClasses().ToArray();
            foreach (Type type in calcTypes)
            {
                //check for the public default ctor in a type
                var ctorInfo = type.GetConstructor(
                    bindingAttr: BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    types: Type.EmptyTypes,
                    modifiers: null);

                if (ctorInfo == null)
                {
                    _errorViewModel.SendError(ErrorType.DesignError,
                        $"The calculation type {type.Name} does not have a public parameterless constructor."
                        + "\n" + "The corresponding chart is not available for creation.");
                }
                else
                {
                    //add the calculation
                    CalculationInfo data = new CalculationInfo(type);
                    calcData.Add(data);
                }
            }

            this.CalculationsData = calcData;
            OnPropertyChanged(nameof(CalculationsData));
        }

        /// <summary>
        /// Represents a kit of calculation type data that intended for using to a view.
        /// </summary>
        public class CalculationInfo
        {
            /// <summary> Gets the calculation type. </summary>
            public Type Type { get; }
            /// <summary> Gets the calculation name. </summary>
            public string Name { get; private set; }
            /// <summary> Gets the description about this calculation type. </summary>
            public string Description { get; private set; }
            /// <summary> Gets the collection of required equipment types for executing this calculation. </summary>
            public IReadOnlyCollection<EquipType> RequiredEquipments { get; private set; }

            public CalculationInfo(Type calculationType)
            {
                if (calculationType == null)
                    throw new ArgumentNullException("The calucalion type cannot be a null", nameof(calculationType));
                if (!typeof(PlotDataCalculation).IsAssignableFrom(calculationType))
                    throw new ArgumentException("The calculation type must be derived from " + nameof(PlotDataCalculation), nameof(calculationType));
                if (calculationType.IsAbstract)
                    throw new ArgumentException("The calculation type shouldn't be an abstract class", nameof(calculationType));

                Type = calculationType;
                RetrieveCalculationInfo(calculationType);
            }

            //overriding equals and gethashcode for comparing by a calculation type
            public override bool Equals(object obj) => (obj as CalculationInfo)?.Type == this.Type;
            public override int GetHashCode() => Type.GetHashCode();

            private void RetrieveCalculationInfo(Type type)
            {
                var viewAttr = type.GetCustomAttribute<CalculationViewAttribute>();
                if (viewAttr != null)
                {
                    this.Name = viewAttr.CalculationName;
                    this.Description = viewAttr.CalculationDescription;
                }
                else
                {
                    this.Name = type.Name;
                    this.Description = type.Name;
                }

                var equipAttr = type.GetCustomAttribute<RequiredEquipmentAttribute>();
                if (equipAttr != null)
                {
                    this.RequiredEquipments = equipAttr.EquipmentTypes.GetFlags().Cast<EquipType>().ToArray();
                }
                else
                {
                    this.RequiredEquipments = new EquipType[0];     //экипировка не нужна, расчет доступен сразу
                    //this.RequiredEquipments = Enum.GetValues(typeof(EquipType)).Cast<EquipType>().ToArray();      //нужны все типы
                }
            }
        }
    }
}
