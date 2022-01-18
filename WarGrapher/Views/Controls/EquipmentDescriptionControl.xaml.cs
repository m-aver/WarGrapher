using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WarGrapher.ViewModels;

namespace WarGrapher.Views.Controls
{
    /// <summary>
    /// Represents a control that displays the characteristics of an equipment item.
    /// </summary>
    public partial class EquipmentDescriptionControl : UserControl
    {
        #region Dependency properties
        public static DependencyProperty EquipItemProperty =
            DependencyProperty.Register(
                nameof(EquipItem),
                typeof(EquipItemViewModel),
                typeof(EquipmentDescriptionControl)
                );

        public static DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register(
                nameof(LabelBackground),
                typeof(Brush),
                typeof(EquipmentDescriptionControl)
                );

        public static DependencyProperty TableBackgroundProperty =
            DependencyProperty.Register(
                nameof(TableBackground),
                typeof(Brush),
                typeof(EquipmentDescriptionControl)
                );

        public static DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(Double),
                typeof(EquipmentDescriptionControl)
                );

        public static DependencyProperty TableBorderThicknessProperty =
            DependencyProperty.Register(
                nameof(TableBorderThickness),
                typeof(Double),
                typeof(EquipmentDescriptionControl)
                );

        public static DependencyProperty TableBorderBrushProperty =
            DependencyProperty.Register(
                nameof(TableBorderBrush),
                typeof(Brush),
                typeof(EquipmentDescriptionControl)
                );

        public static DependencyProperty DescriptionAreaPaddingProperty =
            DependencyProperty.Register(
                nameof(DescriptionAreaPadding),
                typeof(Thickness),
                typeof(EquipmentDescriptionControl)
                );

        public static DependencyProperty HeaderRowBorderBrushProperty =
            DependencyProperty.Register(
                nameof(HeaderRowBorderBrush),
                typeof(Brush),
                typeof(EquipmentDescriptionControl)
                );

        public static DependencyProperty HeaderRowBackgroundProperty =
            DependencyProperty.Register(
                nameof(HeaderRowBackground),
                typeof(Brush),
                typeof(EquipmentDescriptionControl)
                );
        #endregion

        /// <summary>
        /// Gets or sets the equipment item for representing.
        /// </summary>
        public EquipItemViewModel EquipItem
        {
            get { return ((EquipItemViewModel)(base.GetValue(EquipItemProperty))); }
            set { base.SetValue(EquipItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets a background brush of the control label containing the name of an equipment item.
        /// </summary>
        public Brush LabelBackground
        {
            get { return ((Brush)(base.GetValue(LabelBackgroundProperty))); }
            set { base.SetValue(LabelBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a background brush of the table area containing the names and values of item parameters
        /// </summary>
        public Brush TableBackground
        {
            get { return ((Brush)(base.GetValue(TableBackgroundProperty))); }
            set { base.SetValue(TableBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value of the control corners radius.
        /// </summary>
        public Double CornerRadius
        {
            get { return ((Double)(base.GetValue(CornerRadiusProperty))); }
            set { base.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value of the grid lines thickness of the item parameters table.
        /// </summary>
        public Double TableBorderThickness
        {
            get { return ((Double)(base.GetValue(TableBorderThicknessProperty))); }
            set { base.SetValue(TableBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets a brush of the grid lines of the item parameters table.
        /// </summary>
        public Brush TableBorderBrush
        {
            get { return ((Brush)(base.GetValue(TableBorderBrushProperty))); }
            set { base.SetValue(TableBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a padding of the label and the item parameters table.
        /// </summary>
        public Thickness DescriptionAreaPadding
        {
            get { return ((Thickness)(base.GetValue(DescriptionAreaPaddingProperty))); }
            set { base.SetValue(DescriptionAreaPaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a border brush of the header row of the items parameters table.
        /// </summary>
        public Brush HeaderRowBorderBrush
        {
            get { return ((Brush)(base.GetValue(HeaderRowBorderBrushProperty))); }
            set { base.SetValue(HeaderRowBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a background brush of the header row of the item parameters table.
        /// </summary>
        public Brush HeaderRowBackground
        {
            get { return ((Brush)(base.GetValue(HeaderRowBackgroundProperty))); }
            set { base.SetValue(HeaderRowBackgroundProperty, value); }
        }


        public EquipmentDescriptionControl()
        {
            InitializeComponent();
        }
    }
}
