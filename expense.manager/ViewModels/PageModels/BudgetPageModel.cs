using System.Threading.Tasks;
using expense.manager.Mapping;
using expense.manager.Models;
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
            var param = Parameter as (CategoryVm categ, string monthId)?;


            
            if (param?.categ != null && param?.monthId !=null)
            {

                Category = param.Value.categ;
                CurrentMonthId = param.Value.monthId;
                InitialSpecialBudget=SpecifiedBudget = await Service.GetSpecifiedBudget(Category.Map<CategoryVm, Category>(), param.Value.monthId);

                InitialRecurringBudget = Category.RecurringBudget;
            }
            
        }



        private Command _addItemCommand;

        public Command AddItemCommand => _addItemCommand ??= new Command(async () =>
            {


                if (Category.RecurringBudget.HasValue && InitialRecurringBudget != Category.RecurringBudget)
                {
                    await Service.AddOrUpdateCategory(Category.Map<CategoryVm, Category>());

                    await Service.UpdateParentRecurringBudgets(Category.ParentCategory.Map<CategoryVm, Category>(),
                        InitialRecurringBudget, Category.RecurringBudget.Value);

                }

                if (SpecifiedBudget.HasValue && InitialSpecialBudget!=SpecifiedBudget)
                {
                    await Service.AddOrUpdateSpecialBudget(Category.Map<CategoryVm, Category>(), SpecifiedBudget.Value,
                        CurrentMonthId);
                }


                await NavigateBackImpl();

                await CollectionUpdateService.QueueAddOrUpdateCollectionItemTasks();

            }
        );

        public double? SpecifiedBudget { get; set; }
    }
}