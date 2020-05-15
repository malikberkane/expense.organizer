using expense.manager.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class ParametersPageModel : BasePageModel
    {

        private Command _navigateToAboutPage;


        public Currency CurrentCurrency { get; set; }

        public Command NavigateToAboutPage => _navigateToAboutPage ?? (
                                             _navigateToAboutPage = new Command(async () =>
                                             {
                                                 await NavigationService.NavigateTo<AboutPageModel>();

                                             }
                                             )
                                         );


        private Command _navigateToCurrencyChoice;

        public Command NavigateToCurrencyChoice => _navigateToCurrencyChoice ?? (
                                             _navigateToCurrencyChoice = new Command(async () =>
                                             {
                                                 await NavigationService.NavigateTo<CurrencyChoicePageModel>();
                                             }));


        public override async Task LoadData()
        {
            CurrentCurrency = await Service.GetCurrency(AppPreferences.CurrentCurrency.cc);
        }
    }
}
