using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfCustomControlsByMe
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(RepeatButton))]
    public class NumericUpDown : Control
    {
        #region ctor
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public NumericUpDown()
        {
            Loaded += OnLoaded;
        }

        
        #endregion

        #region entire control references
        protected TextBox textBox;
        protected RepeatButton increaseButton;
        protected RepeatButton decreaseButton;

        #endregion

        #region attach template parts and commands
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachToVisualTree();
            AttachCommands();
        }

        private void AttachToVisualTree()
        {
            AttachTextBox();
            AttachIncreaseButton();
            AttachDecreaseButton();
        }

        private void AttachTextBox()
        {
            var partTextBox = GetTemplateChild("PART_TextBox") as TextBox;
            if (partTextBox != null)
            {
                textBox = partTextBox;
                textBox.InputBindings.Add(new KeyBinding(IncreaseValueCommand, new KeyGesture(Key.Up)));
                textBox.InputBindings.Add(new KeyBinding(DecreaseValueCommand, new KeyGesture(Key.Down)));
                textBox.InputBindings.Add(new KeyBinding(UpdateValueStringCommand, new KeyGesture(Key.Enter)));
                textBox.LostFocus += TextBoxOnLostFocus;
            }
        }


        private void AttachIncreaseButton()
        {
            var partIncreaseButton = GetTemplateChild("PART_IncreaseButton") as RepeatButton;
            if (partIncreaseButton != null)
            {
                increaseButton = partIncreaseButton;
                increaseButton.Focusable = false;
                increaseButton.Command = IncreaseValueCommand;
            }
        }

        private void AttachDecreaseButton()
        {
            var partDecreaseButton = GetTemplateChild("PART_DecreaseButton") as RepeatButton;
            if (partDecreaseButton != null)
            {
                decreaseButton = partDecreaseButton;
                decreaseButton.Focusable = false;
                decreaseButton.Command = DecreaseValueCommand;
            }
        }
        #endregion

        #region dependency properties

        #region Value
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown), new PropertyMetadata((double)0, OnValueChanged, CoerceValue));

        private static object CoerceValue(DependencyObject element, object baseValue)
        {
            var control = (NumericUpDown)element;
            var value = (double)baseValue;
            control.CoerceValueToBounds(ref value);
            if (control.textBox != null)
            {
                control.textBox.Text = value.ToString(CultureInfo.CurrentCulture);
            }
            return value;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        #region MinValue
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata((double)0, OnMinValueChanged, CoerceMinValue));


        private static void OnMinValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDown)element;
            var minValue = (double)e.NewValue;

            if(minValue > control.MaxValue)
            {
                control.MaxValue = minValue;
            }
            control.Value = control.Value;
        }

        private static object CoerceMinValue(DependencyObject element, object baseValue)
        {
            var minValue = (double)baseValue;
            return minValue;
        }
        #endregion

        #region MaxValue
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(double.MaxValue, OnMaxValueChanged, CoerceMaxValue));


        private static void OnMaxValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDown)element;
            var maxValue = (double)e.NewValue;

            if(maxValue < control.MinValue)
            {
                control.MinValue = maxValue;
            }

            control.Value = control.Value;
        }

        private static object CoerceMaxValue(DependencyObject element, object baseValue)
        {
            var maxValue = (double)baseValue;
            return maxValue;
        }

        #endregion

        #endregion

        #region Commands, funtions of commands and attaching functions to commands

        private readonly RoutedUICommand IncreaseValueCommand =
            new RoutedUICommand("MinorIncreaseValue", "MinorIncreaseValue", typeof(NumericUpDown));

        private readonly RoutedUICommand DecreaseValueCommand =
            new RoutedUICommand("MinorDecreaseValue", "MinorDecreaseValue", typeof(NumericUpDown));

        private readonly RoutedUICommand UpdateValueStringCommand =
            new RoutedUICommand("UpdateValueString", "UpdateValueString", typeof(NumericUpDown));

        private void IncreaseValue()
        {
            if (textBox.IsFocused)
            {
                Focus();
            }
            Value++;
        }

        private void DecreaseValue()
        {
            if (textBox.IsFocused)
            {
                Focus();
            }
            Value--;
        }

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs e)
        {
            double value = ParseStringToDouble(textBox.Text);
            if (!double.IsNaN(value))
            {
                Value = value;
            }
            else
            {
                Value = Value;
            }
        }

        private void AttachCommands()
        {
            CommandBindings.Add(new CommandBinding(IncreaseValueCommand, (a, b) => IncreaseValue()));
            CommandBindings.Add(new CommandBinding(DecreaseValueCommand, (a, b) => DecreaseValue()));
            CommandBindings.Add(new CommandBinding(UpdateValueStringCommand, (a, b) => Focus()));
        }
        #endregion

        #region entirely used functions
        private double ParseStringToDouble(string source)
        {
            double value;
            if (double.TryParse(source, out value))
            {
                return value;
            }
            return double.NaN;
        }

        private void CoerceValueToBounds(ref double value)
        {
            if(value < MinValue)
            {
                value = MinValue;
            }
            if(value > MaxValue)
            {
                value = MaxValue;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InvalidateProperty(ValueProperty);
        }
        #endregion
    }
}
