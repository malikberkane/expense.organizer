using expense.manager.ViewModels.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using expense.manager.Models;
using Xamarin.Forms;

namespace expense.manager.Views
{
    public class CurrencySearchHandler : SearchHandler
    {

        public static readonly BindableProperty SelectedCurrencyProperty =
                    BindableProperty.Create(nameof(SelectedCurrency), typeof(Currency), typeof(CurrencySearchHandler));


        public Currency SelectedCurrency
        {
            get => (Currency)this.GetValue(SelectedCurrencyProperty);
            set => this.SetValue(SelectedCurrencyProperty, value);
        }



        public static readonly BindableProperty CurrenciesProperty =
              BindableProperty.Create(nameof(Currencies), typeof(IEnumerable<Currency>), typeof(CurrencySearchHandler));


        public IEnumerable<Currency> Currencies
        {
            get => (IEnumerable<Currency>)this.GetValue(CurrenciesProperty);
            set => this.SetValue(CurrenciesProperty, value);
        }


        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ItemsSource = Currencies.Where(n => n.name.ToLower().Contains(newValue.ToLower()));

            }
        }

        protected override void OnItemSelected(object item)
        {
            base.OnItemSelected(item);

            if (item is Currency selectedCurrency)
            {
                SelectedCurrency = selectedCurrency;

            }

        }
    }

}
