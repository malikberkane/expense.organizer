using expense.manager.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using expense.manager.Data;
using expense.manager.Models;
using expense.manager.Resources;
using expense.manager.Utils;
using Xamarin.Essentials;
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


        private Command _exportToCsvCommand;


        public Command ExportToCsvCommand => _exportToCsvCommand ??= new Command( async () =>
        {
            await ExportToCsvCommandImpl();
        });

        private async Task ExportToCsvCommandImpl()
        {
            var permission = new Permissions.StorageWrite();
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            if (status != PermissionStatus.Granted)
            {
                return;
            }

            
            await Task.Run(async () =>
            {
                await EnsureIsBusy(async () =>
                {

                    try
                    {
                        var filePath = await Service.ExportExpensesAsCsv();

                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            if (await NavigationService.DisplayYesNoMessage(
                                $"{AppContent.FileSaveLocation} {filePath}.{AppContent.ShareCsvPrompt}",
                                AppContent.Share))
                            {
                                await Share.RequestAsync(new ShareFileRequest
                                {
                                    Title = Title,
                                    File = new ShareFile(filePath)
                                });
                            }
                        });

                    }
                    catch (Exception ex)
                    {

                        await NavigationService.DisplayAlert(ex.Message);
                    }

                        
                   
                });
            });
        }


          
        }
    }

  

