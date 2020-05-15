using expense.manager.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace expense.manager
{
    public class NullToPlaceholderConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
            {
                return AppContent.None;
            }
            else if(value is CategoryVm categ)
            {
                return categ.Name;
            }

            return value;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    
    }
}
