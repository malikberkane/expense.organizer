
using System;
using expense.manager.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using expense.manager.Services;
using expense.manager.Views;
using AutoMapper;
using expense.manager.Mapping;
using expense.manager.Utils;

namespace expense.manager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            DependencyService.Register<Repository>();
            DependencyService.Register<ExpenseManagerService>();
            DependencyService.Register<MessagingService>();

            PersitPreviousBudgets();

            MapperHelper.Intialize();
            MainPage = new AppShell();

        }

        private static void PersitPreviousBudgets()
        {
            if (DateTime.TryParse(AppPreferences.LastUsedDate, out var lastUsedDate))
            {
                var service = DependencyService.Get<IExpenseManagerService>();

                var firstDayCurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var firstDayLastUsedMonth= new DateTime(lastUsedDate.Year, lastUsedDate.Month, 1);

                if (firstDayLastUsedMonth< firstDayCurrentMonth)
                {
                    var monthToUpdate = firstDayLastUsedMonth;
                    while (monthToUpdate < firstDayCurrentMonth)
                    {
                        service.PersistPreviousMonthBudget(monthToUpdate.ToMonthId());
                        monthToUpdate= monthToUpdate.AddMonths(1);

                    }

                }
            }


            AppPreferences.LastUsedDate = DateTime.Now.Date.ToString();
        }


        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var swipeViewSent = (sender as SwipeView);

            if (swipeViewSent.Parent is CollectionView collectionView)
            {
                collectionView.SelectedItem = swipeViewSent.BindingContext;
            }

        }



        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
