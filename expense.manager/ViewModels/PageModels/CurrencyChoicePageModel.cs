using expense.manager.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class CurrencyChoicePageModel : BasePageModel
    {
        private IEnumerable<Currency> _currencies;

        public IEnumerable<Currency> Currencies { get => _currencies; set => SetProperty(ref _currencies, value); }


        public override Task LoadData()
        {


            Currencies = Service.GetCurrencies();
            return Task.CompletedTask;
        }

        public Currency SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem,value); }


        private Command _itemSelection;
        private Currency _selectedItem;

        public Command ItemSelection => _itemSelection ?? (
                                    _itemSelection = new Command(async () =>
                                    {

                                        if (SelectedItem == null)
                                        {
                                            return;
                                        }


                                        if(await NavigationService.DisplayYesNoMessage($"{AppContent.Select} {SelectedItem}?"))
                                        {
                                            AppPreferences.SetCurrentCurrency(SelectedItem);
                                            MessagingService.Send(MessagingKeys.CurrencyChange);
                                            await NavigationService.PopToRoot();


                                        }

                                        SelectedItem = null;

                                    }
                                    )
                                );

    }


}

