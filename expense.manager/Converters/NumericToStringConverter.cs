using System;
using System.Globalization;
using Xamarin.Forms;

namespace expense.manager.Converters
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
}
