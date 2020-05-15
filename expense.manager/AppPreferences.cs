using expense.manager.Services;
using expense.manager.ViewModels.PageModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace expense.manager
{
    public static class AppPreferences
    {


        public static IExpenseManagerService Service => DependencyService.Get<IExpenseManagerService>();


        public static void SetCurrentCurrency(Currency currency)
        {
            CurrentCurrency = currency;

        }


        private static Currency _currentCurrency;

        public static Currency CurrentCurrency
        {
            get
            {

                if (_currentCurrency == null)
                {

                    _currentCurrency = Service.GetCurrency(Preferences.Get("currency", "USD")).Result;

                    return _currentCurrency;
                }

                return _currentCurrency;
            }
            set
            {
                _currentCurrency = value;
                Preferences.Set("currency", value.cc);
            }
        }




        public static string LastUsedDate
        {
            get => Preferences.Get("LastUseDateKey", string.Empty);
            set => Preferences.Set("LastUseDateKey", value);
        }



    }
}
