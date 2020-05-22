using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace expense.manager
{
    public class NumericToStringDisplayConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsDecimal = value as double?;


            if (valueAsDecimal!=null)
            {
                if (valueAsDecimal == 0) { return null; }
                return valueAsDecimal.ToString().Replace(",",".");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == string.Empty)
            {
                return default(double);
            }
            return value;

        }
    }


    public class NumericInput : Entry
    {

        public static BindableProperty AllowNegativeProperty = BindableProperty.Create("AllowNegative", typeof(bool), typeof(NumericInput), false, BindingMode.TwoWay);
        public static BindableProperty AllowFractionProperty = BindableProperty.Create("AllowFraction", typeof(bool), typeof(NumericInput), false, BindingMode.TwoWay);

        public NumericInput()
        {
            this.Keyboard = Keyboard.Numeric;
        }

        public bool AllowNegative
        {
            get { return (bool)GetValue(AllowNegativeProperty); }
            set { SetValue(AllowNegativeProperty, value); }
        }

        public bool AllowFraction
        {
            get { return (bool)GetValue(AllowFractionProperty); }
            set { SetValue(AllowFractionProperty, value); }
        }
    }
}
