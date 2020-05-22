using expense.manager.ViewModels.PageModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using expense.manager.ViewModels.Base;
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

            if (args.Source== ShellNavigationSource.ShellSectionChanged && Current.Navigation.NavigationStack.Count>1)
            {
                Current.Navigation.PopToRootAsync(animated:false);
            }
            base.OnNavigated(args);
        }

        protected override bool OnBackButtonPressed()
        {
            var page = (Current?.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage;

            if (!(page?.BindingContext is BasePageModel pageModel)) return base.OnBackButtonPressed();
            pageModel.NavigateBackImpl();
            return true;

        }
    }
}
