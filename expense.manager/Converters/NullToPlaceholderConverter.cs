using System;
using System.Globalization;
using expense.manager.Resources;
using expense.manager.ViewModels;
using Xamarin.Forms;

namespace expense.manager.Converters
{
    public class NullToPlaceholderConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return AppContent.None;
            }

          
             if(value is CategoryVm categ)
             {
                 return categ.Id != 0 ? categ.Name : AppContent.None;
             }

            return value;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    
    }
}
