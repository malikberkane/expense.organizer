using expense.manager.ViewModels.PageModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace expense.manager
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()   
        {
            InitializeComponent();


            Initalization = InitializePageModels();
        }

        public Task Initalization { get; }

        public async Task InitializePageModels()
        {
            await TagRecapPageModel.Initialize();
            await MainRecapPageModel.Initialize();
            await ExpenseByDatePageModel.Initialize();
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            if(args.Source== ShellNavigationSource.ShellSectionChanged && Current.Navigation.NavigationStack.Count>1)
            {
                Current.Navigation.PopToRootAsync(animated:false);
            }
            base.OnNavigated(args);
        }

     


    }
}
