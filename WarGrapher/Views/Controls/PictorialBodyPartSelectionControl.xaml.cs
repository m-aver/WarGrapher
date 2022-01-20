using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WarGrapher.Common;

namespace WarGrapher.Views.Controls
{
    /// <summary>
    /// Represents a control that is intended for selecting <see cref="BodyPart"/> by an image.
    /// </summary>
    public partial class PictorialBodyPartSelectionControl : UserControl, INotifyPropertyChanged
    {
        #region Dependency properties
        public static readonly DependencyProperty PartsDataProperty =
            DependencyProperty.Register(
                nameof(PartsData),
                typeof(IReadOnlyCollection<PartInfo>),
                typeof(PictorialBodyPartSelectionControl),
                new PropertyMetadata(
                    defaultValue: new List<PartInfo>(), 
                    propertyChangedCallback: HandlePartsDataChanges),
                validateValueCallback: ValidatePartsData
                );

        public static readonly DependencyProperty SelectionCommandProperty =
            DependencyProperty.Register(
                nameof(SelectionCommand),
                typeof(ICommand),
                typeof(PictorialBodyPartSelectionControl)
                );

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(PictorialBodyPartSelectionControl)
                );

        public static DependencyProperty DefaultPartColorProperty =
            DependencyProperty.Register(
                nameof(DefaultPartColor),
                typeof(Color),
                typeof(PictorialBodyPartSelectionControl)
                );

        public static DependencyProperty SelectionPartFillProperty =
            DependencyProperty.Register(
                nameof(SelectionPartFill),
                typeof(Brush),
                typeof(PictorialBodyPartSelectionControl)
                );

        public static DependencyProperty PartStrokeProperty =
            DependencyProperty.Register(
                nameof(PartStroke),
                typeof(Brush),
                typeof(PictorialBodyPartSelectionControl)
                );

        public static DependencyProperty HoveringPartOpacityProperty =
            DependencyProperty.Register(
                nameof(HoveringPartOpacity),
                typeof(Double),
                typeof(PictorialBodyPartSelectionControl)
                );

        public static DependencyProperty DefaultPartOpacityProperty =
            DependencyProperty.Register(
                nameof(DefaultPartOpacity),
                typeof(Double),
                typeof(PictorialBodyPartSelectionControl)
                );
        #endregion

        /// <summary>
        /// Gets or sets the collection of parts data for rendering body parts.
        /// </summary>
        public IReadOnlyCollection<PartInfo> PartsData
        {
            get { return (IReadOnlyCollection<PartInfo>)GetValue(PartsDataProperty); }
            set { SetValue(PartsDataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command that executes when a body part is selected.
        /// Command parameter is the selected <see cref="BodyPart"/>
        /// </summary>
        public ICommand SelectionCommand
        {
            get { return (ICommand)GetValue(SelectionCommandProperty); }
            set { SetValue(SelectionCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source of a background image of this control.
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        #region Appearance properties
        /// <summary>
        /// Get or sets the default color of body parts.
        /// </summary>
        public Color DefaultPartColor
        {
            get { return ((Color)(base.GetValue(DefaultPartColorProperty))); }
            set { base.SetValue(DefaultPartColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the default opacity of body parts.
        /// </summary>
        public Double DefaultPartOpacity
        {
            get { return ((Double)(base.GetValue(DefaultPartOpacityProperty))); }
            set { base.SetValue(DefaultPartOpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the final opacity of a body part that is under the mouse pointer.
        /// </summary>
        public Double HoveringPartOpacity
        {
            get { return ((Double)(base.GetValue(HoveringPartOpacityProperty))); }
            set { base.SetValue(HoveringPartOpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the fill brush of the selected body part.
        /// </summary>
        public Brush SelectionPartFill
        {
            get { return ((Brush)(base.GetValue(SelectionPartFillProperty))); }
            set { base.SetValue(SelectionPartFillProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke brush of body parts.
        /// </summary>
        public Brush PartStroke
        {
            get { return ((Brush)(base.GetValue(PartStrokeProperty))); }
            set { base.SetValue(PartStrokeProperty, value); }
        }
        #endregion


        public PictorialBodyPartSelectionControl()
        {
            InitializeComponent();
        }

        static private bool ValidatePartsData(object data)
        {
            IReadOnlyCollection<PartInfo> partsData = (IReadOnlyCollection<PartInfo>)data;

            //клей пиздец, тут бы алгоритмы прикрутить
            if (partsData.Distinct().Count() != partsData.Count)
                throw new Exception(nameof(PartsData) + " must have unique " + nameof(BodyPart) + " values");
            if (partsData.Where(pd => pd.IsInitiallySelected).Count() > 1)
                throw new Exception(nameof(PartsData) + " may have only one field " + nameof(PartInfo.IsInitiallySelected) + " with true value");

            return true;
        }
        static private void HandlePartsDataChanges(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            var control = (PictorialBodyPartSelectionControl)depObj;
            var partsData = (IReadOnlyCollection<PartInfo>)args.NewValue;

            BodyPart initiallySelectedPart = partsData.SingleOrDefault(pd => pd.IsInitiallySelected).BodyPart;

            if (control.SelectionCommand.CanExecute(initiallySelectedPart))
                control.SelectionCommand.Execute(initiallySelectedPart);
        }

        #region Property changed code
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler propertyChangedHandler = this.PropertyChanged;
            propertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Verify that the property name matches a real, public, instance property on this object. 
        /// </summary>
        private void VerifyPropertyName(string propertyName)
        {
            if (this.GetType().GetProperty(propertyName) == null)
            {
                string msg = "Invalid property name: " + propertyName;
                throw new Exception(msg);
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a kit of properties for configuring a body part from client code.
    /// </summary>
    public struct PartInfo
    {
        public Geometry PathData { get; set; }
        public BodyPart BodyPart { get; set; }
        public bool IsInitiallySelected { get; set; }

        // overriding equals and gethashcode for body part comparision 
        public override bool Equals(object obj) => (obj is PartInfo) && ((PartInfo)obj).BodyPart == this.BodyPart;
        public override int GetHashCode() => this.BodyPart.GetHashCode();
    }
}
