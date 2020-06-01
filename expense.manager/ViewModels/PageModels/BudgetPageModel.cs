using System;
using System.Linq;
using System.Threading.Tasks;
using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.Resources;
using expense.manager.ViewModels.Base;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class BudgetPageModel: BasePageModel
    {
        private Command _displaySpecialBudgetDialog;


        public Command DisplaySpecialBudgetDialog => _displaySpecialBudgetDialog ??= new Command(async () =>
            {
                await NavigationService.DisplayAlert(AppContent.SpecificBudgetNote, title: AppContent.Info);
            }
        );

        public CategoryVm Category { get;  set; }
        public string CurrentMonthId { get; set; }

        public double? InitialRecurringBudget { get; set; }

        public double? InitialSpecialBudget { get; set; }



        public override async Task LoadData()
        {
            var param = Parameter as (CategoryVm categ, ItemsContext context)?;


            
            if (param?.categ != null && param?.context !=null)
            {

                Category = param.Value.categ;
                CurrentMonthId = param.Value.context.MonthId;
                InitialSpecialBudget=SpecifiedBudget = await Service.GetSpecifiedBudget(Category.Map<CategoryVm, Category>(), param.Value.context.MonthId);

                InitialRecurringBudget = Category.RecurringBudget;

                Title = $"{ AppContent.Budget}: {(Category.Name).ToLower()}";

                ShowSpecifiedBudget = new DateTime(DateTime.Now.Year, DateTime.Now.Month,1) > param.Value.context.ContextDate;


            }


        }



        private Command _addItemCommand;

        public Command AddItemCommand => _addItemCommand ??= new Command(async () =>
            {


                if (Category.RecurringBudget.HasValue && InitialRecurringBudget != Category.RecurringBudget)
                {

                    await Service.AddOrUpdateCategory(Category.Map<CategoryVm, Category>());

                

                }

                if (SpecifiedBudget.HasValue && InitialSpecialBudget!=SpecifiedBudget)
                {
                    if (InitialSpecialBudget.GetValueOrDefault() != 0 && SpecifiedBudget.Value == 0)
                    {
                        await Service.DeleteSpecialBudget(Category.Map<CategoryVm, Category>(), CurrentMonthId);
                    }
                    else
                    {
                        await Service.AddOrUpdateSpecialBudget(Category.Map<CategoryVm, Category>(), SpecifiedBudget.Value,
                            CurrentMonthId);
                    }
        
                }


                await NavigateBackImpl();

                await CollectionUpdateService.QueueAddOrUpdateCollectionItemTasks();

            }
        );

        public double? SpecifiedBudget { get; set; }

        public bool ShowSpecifiedBudget { get; set; }
    }
}