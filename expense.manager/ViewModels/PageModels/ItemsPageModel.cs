using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.Services;
using expense.manager.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using expense.manager.Resources;
using expense.manager.Utils;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class ItemsPageModel : ListPageModel<BaseViewModel>
    {

        public ItemsContext ItemsContext { get; set; }



        protected override async Task EditItemImpl(BaseViewModel item)
        {


            if (item is CategoryVm categ)
            {

                await NavigationService.NavigateTo<AddCategoryPageModel>(categ);
            }
            else if (item is ExpenseVm expense)
            {
                await NavigationService.NavigateTo<AddExpensePageModel>(expense);

            }


        }


        public virtual bool IsHomeContext { get; set; }
        protected override async Task AddItemImpl()
        {
            var actionKey = await NavigationService.DisplayActionSheet(new[] { AppContent.Category, AppContent.Expense });
            if (actionKey == AppContent.Expense)
            {
                await NavigationService.NavigateTo<AddExpensePageModel>(
                    new ExpenseVm()
                    {
                        CreationDate = DateTime.Now,
                        CategoryId = ItemsContext.Category.Id
                    });

            }
            else if (actionKey == AppContent.Category)
            {
                await NavigationService.NavigateTo<AddCategoryPageModel>(
                    new CategoryVm
                                        {
                        ParentCategory = ItemsContext.Category 
                    }
                    );

            }
        }


        protected override async Task ItemSelectionImpl()
        {
            await EnsureIsBusy(async () =>
            {
                if (SelectedItem is CategoryVm categ)
                {
                    await NavigationService.NavigateTo<ItemsPageModel>(new ItemsContext { ContextDate = ItemsContext.ContextDate, MonthId = ItemsContext.MonthId, Category = categ });

                }



            });
        }


        protected override async Task DeleteItemImpl(BaseViewModel item)
        {


            if (item is ExpenseVm expense && await NavigationService.DisplayYesNoMessage(AppContent.DeleteExpenseConfirmation))
            {
                await Service.DeleteExpense(expense.Map<ExpenseVm, Expense>());



            }
            else if (item is CategoryVm categ && await NavigationService.DisplayYesNoMessage(AppContent.DeleteCategoryConfirmation))
            {
                await Task.Run(() => EnsureIsBusy(() => Service.DeleteCategory(categ.Map<CategoryVm, Category>())));

                MessagingService.Send(MessagingKeys.DeleteItemKey);
            }


            await CollectionUpdateService.QueueDeleteCollectionItemTasks();



        }




        private Command _changeSelectedMonthCommand;

        public Command ChangeSelectedMonthCommand => _changeSelectedMonthCommand ?? (
                                             _changeSelectedMonthCommand = new Command<int>(async (increment) =>await SwitchSelectedMonthImpl(increment)


                                             ));




        public  async Task SwitchSelectedMonthImpl(int monthIncrement)
        {

            ItemsContext.ContextDate = ItemsContext.ContextDate.AddMonths(monthIncrement);

            await EnsureIsBusy(() => LoadData());
            AfterLoadingData();


        }






        private Command _budgetCategoryCommand;

        public Command BudgetCategoryCommand => _budgetCategoryCommand ??= new Command<CategoryVm>(async (categ) => await BudgetCategoryImpl(categ)


        );




        public async Task BudgetCategoryImpl(CategoryVm categ)
        {

            await NavigationService.NavigateTo<BudgetPageModel>((categ, ItemsContext));
        }


        protected override async Task<IEnumerable<BaseViewModel>> LoadItems()
        {

            try
            {

                var result = new List<BaseViewModel>();


                var categories = (await Service.GetCategoriesRecap(ItemsContext.MonthId, ItemsContext.Category.Map<CategoryVm, Category>())).Select(n => n.Map<Category, CategoryVm>());

                if (categories != null && categories.Any())
                {
                    result.AddRange(categories);
                }




                var expenses = (await Service.GetExpensesRecap(ItemsContext.MonthId, ItemsContext.Category.Id)).Select(n => n.Map<Expense, ExpenseVm>());

                if (expenses != null && expenses.Any())
                {
                    result.AddRange(expenses);
                }



                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }


        }





        private Command _navigateToSettings;

        public Command NavigateToSettings => _navigateToSettings ??= new Command(async () =>
        {
            await NavigationService.NavigateTo<ParametersPageModel>();
        });



        protected override void BeforeLoadingData()
        {
            base.BeforeLoadingData();
            MessagingCenter.Subscribe<MessagingService>(this, MessagingKeys.CurrencyChange, (sender) =>
            {
                foreach (var cell in Items)
                {
                    cell.RaiseAllPropertiesChanged();
                }

                ComputeHomeContextTitle();
            });

            ItemsContext = Parameter as ItemsContext;


            if (ItemsContext == null)
            {
                IsHomeContext = true;
                //No parameter, we are on the home page, we must initialize the parameters
                ItemsContext = new ItemsContext { Category = new CategoryVm(), ContextDate = DateTime.Now };
                HandleBackCommandImpl = false;
            }
            else
            {
                Title = $"{ItemsContext.Category.Name} {ItemsContext.ContextDate:MMMM yyyy}";


            }



        }


        protected override void AfterLoadingData()
        {

            if (IsHomeContext)
            {
                ComputeHomeContextTitle();

            }
        }

        private void ComputeHomeContextTitle()
        {
            var sum = Items?.Sum(n =>
            {
                switch (n)
                {
                    case ExpenseVm expense:
                        return expense.Ammount;
                    case CategoryVm category:
                        return category.AmmountSpent;
                    default:
                        return 0;
                }
            });

            if (sum == null) return;
            {
                var budget = Items.Sum(n =>
                {
                    if (n is CategoryVm categ)
                    {
                        return categ.Budget;
                    }
                    return 0;
                });

                var sumForBudget= Items.Sum(n =>
                {
                    if (n is CategoryVm categ && categ.HasBudget)
                    {
                        return categ.AmmountSpent;
                    }
                    return 0;
                });

                if (budget!=null && budget != 0 && sumForBudget!=0)
                {
                    Title = $"{sum:0.##} {AppPreferences.CurrentCurrency?.symbol}";

                    BudgetRatioInfo = $"{((sumForBudget / budget) * 100):0.##} % {AppContent.OfBudget}";

                }
                else
                {
                    Title = $"{sum:0.##} {AppPreferences.CurrentCurrency?.symbol}";
                    BudgetRatioInfo = null;

                }
            }

        }


        public string BudgetRatioInfo
        {
            get => _budgetRatioInfo;
            set => SetProperty(ref _budgetRatioInfo, value);
        }

        string _budgetRatioInfo = string.Empty;

    }

}
