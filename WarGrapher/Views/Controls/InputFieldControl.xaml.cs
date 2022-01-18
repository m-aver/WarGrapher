using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarGrapher.Common;

namespace WarGrapher.Views.Controls
{
    /// <summary>
    /// Represents a control intended for input an item name through the text field and the dropdown list.
    /// </summary>
    public partial class InputFieldControl : UserControl, IDataErrorInfo, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(
                nameof(DataSource),
                typeof(IEnumerable<string>),
                typeof(InputFieldControl),
                new PropertyMetadata(new List<string>())
                );

        /// <summary>
        /// Gets or sets a source collection of a possible input values.
        /// </summary>
        public IEnumerable<string> DataSource
        {
            get { return (IEnumerable<string>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }
        
        /// <summary>
        /// Gets the collection that contains current values in the dropdown list.
        /// </summary>
        [ReadOnly(true)]
        public ObservableCollection<string> DataToInput { get; } = new ObservableCollection<string>();


        public InputFieldControl()
        {
            InitializeComponent();
            TextField.TextChanged += HandleTextChanges;
        }


        #region -- Control event handlers --
        // the handlers for servicing input from the dropdowm list
        private void TextFieldFocusedHandler(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            RefreshDataToInput(textBox.Text);
        }

        private void TextFieldUnfocusedHandler(object sender, RoutedEventArgs e)
        {
        }

        private void TextChangedHandler(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            RefreshDataToInput(textBox.Text);
        }

        private void SelectionItemChangedHandler(object sender, SelectionChangedEventArgs e)
        {
            string text = ItemList.SelectedItem?.ToString();
            ItemList.SelectedItem = null;                       // важно присваивать значение текстовому блоку именно после установки SelectedItem в null
            TextField.Text = text;
        }

        private void ItemSelectionButtonClickHandler(object sender, RoutedEventArgs e)
        {
            OnItemSelectionButtonClicked(sender);
        }
        #endregion -- Control event handlers --

        /// <summary>
        /// Updates the <see cref="DataToInput"/> property so that it contains only those values from <see cref="DataSource"/> which match to the passed text.
        /// </summary>
        private void RefreshDataToInput(string text)
        {
            DataToInput.Clear();

            DataSource?.ForEach((s) =>
            {
                if (s.ToLower().Contains(text.ToLower()))
                    DataToInput.Add(s);
            });
        }


        #region -- Validation and Notification --
        #region Dependency properties
        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register(
                nameof(InputText),
                typeof(String),
                typeof(InputFieldControl),
                new PropertyMetadata(String.Empty, HandleInputTextChanges)
                );

        public static readonly DependencyProperty NotifyOnValidTextChangesOnlyProperty =
            DependencyProperty.Register(
                nameof(NotifyOnValidTextChangesOnly),
                typeof(bool),
                typeof(InputFieldControl),
                new PropertyMetadata(false)
                );

        public static readonly DependencyProperty NotifyOnTextChangedOutsideProperty =
            DependencyProperty.Register(
                nameof(NotifyOnTextChangedOutside),
                typeof(bool),
                typeof(InputFieldControl),
                new PropertyMetadata(true)
                );
        #endregion Dependency properties

        /// <summary>
        /// Gets a value that indicates whether a current input text is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to notify a client only on valid input text changes or on any text changes.
        /// </summary>
        public bool NotifyOnValidTextChangesOnly
        {
            get { return (bool)GetValue(NotifyOnValidTextChangesOnlyProperty); }
            set { SetValue(NotifyOnValidTextChangesOnlyProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to notify a client when a text is changed 
        /// outside by programmatic input through the <see cref="InputText"/> property 
        /// or only by manually input from the user interface.
        /// </summary>
        public bool NotifyOnTextChangedOutside
        {
            get { return (bool)GetValue(NotifyOnTextChangedOutsideProperty); }
            set { SetValue(NotifyOnTextChangedOutsideProperty, value); }
        }

        private bool _textChangedFromOutside { get; set; }

        /// <summary>
        /// Sets a text by programmatic input from external code.
        /// </summary>
        public string InputText
        {
            get { return (String)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }
        
        /// <summary>
        /// Gets an entered text (including manyally input).
        /// </summary>
        [ReadOnly(true)]
        [Bindable(true, BindingDirection.OneWay)]
        public string OutputText
        {
            get { return _outputText; }
            set
            {
                _outputText = value;
                OnPropertyChanged(nameof(OutputText));
            }
        }
        private string _outputText;
        #region REMARK
        // атрибуты выставленны с целью инкапсуляции от внешнего кода
        // вроде работает и не мешает привязке из родного XAML и валидации
        #endregion

        // the helper handlers
        static private void HandleInputTextChanges(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            var control = depObj as InputFieldControl;

            control._textChangedFromOutside = true;
            control.TextField.Text = (string)args.NewValue;    
            control._textChangedFromOutside = false;

            #region REMARK
            //почему-то текстовое поле не хочет обновлятся по привязке, если уже был введен какой-то текст
            //приходится обновлять вручную, плюс есть возможность добавить фильтрацию по программному вводу текста извне
            #endregion
        }

        private void HandleTextChanges(object sender, TextChangedEventArgs e)
        {
            IsValid = !Validation.GetHasError(TextField);

            if ((!NotifyOnValidTextChangesOnly || IsValid) &&
                (NotifyOnTextChangedOutside || !_textChangedFromOutside))
            {
                OnOutputTextChanged(sender);
            }
        }

        // the IDataErrorInfo members
        public string Error { get; private set; }
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(OutputText):
                        return (DataSource.Contains(OutputText) || OutputText == String.Empty) ? String.Empty : "Not correct value";
                    default:
                        return String.Empty;
                }
            }
        }
        #endregion -- Validation and Notification --


        #region -- User routed events --
        public static readonly RoutedEvent OutputTextChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(OutputTextChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(InputFieldControl)
                );

        public static readonly RoutedEvent ItemSelectionButtonClickedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ItemSelectionButtonClicked),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(InputFieldControl)
                );


        public event RoutedEventHandler OutputTextChanged
        {
            add { base.AddHandler(InputFieldControl.OutputTextChangedEvent, value); }
            remove { base.RemoveHandler(InputFieldControl.OutputTextChangedEvent, value); }
        }

        public event RoutedEventHandler ItemSelectionButtonClicked
        {
            add { base.AddHandler(InputFieldControl.ItemSelectionButtonClickedEvent, value); }
            remove { base.RemoveHandler(InputFieldControl.ItemSelectionButtonClickedEvent, value); }
        }


        private void OnOutputTextChanged(object sender)
        {
            base.RaiseEvent(new RoutedEventArgs(OutputTextChangedEvent, sender));
        }

        private void OnItemSelectionButtonClicked(object sender)
        {
            base.RaiseEvent(new RoutedEventArgs(ItemSelectionButtonClickedEvent, sender));
        }
        #endregion -- User routed events --


        #region -- Commands --
        // --================ ButtonClickCommand ================--
        public static readonly DependencyProperty ButtonClickCommandProperty =
            DependencyProperty.Register(
                nameof(ButtonClickCommand),
                typeof(ICommand),
                typeof(InputFieldControl)
                );

        public ICommand ButtonClickCommand
        {
            get { return (ICommand)GetValue(ButtonClickCommandProperty); }
            set { SetValue(ButtonClickCommandProperty, value); }
        }

        // --================ CommandParameter ================--
        public static readonly DependencyProperty ButtonClickCommandParameterProperty =
            DependencyProperty.Register(
                nameof(ButtonClickCommandParameter),
                typeof(Object),
                typeof(InputFieldControl)
                );

        public Object ButtonClickCommandParameter
        {
            get { return (Object)GetValue(ButtonClickCommandParameterProperty); }
            set { SetValue(ButtonClickCommandParameterProperty, value); }
        }

        // --================ TextChangedCommand ================--
        public static readonly DependencyProperty TextChangedCommandProperty =
            DependencyProperty.Register(
                nameof(TextChangedCommand),
                typeof(ICommand),
                typeof(InputFieldControl)
                );

        public ICommand TextChangedCommand
        {
            get { return (ICommand)GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }
        #endregion -- Commands --


        #region -- Property changed code--
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
}
