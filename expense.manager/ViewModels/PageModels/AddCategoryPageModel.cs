using System.Linq;
using System.Threading.Tasks;
using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.Resources;
using expense.manager.Services;
using expense.manager.Utils;
using expense.manager.ViewModels.Base;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class AddCategoryPageModel : BasePageModel
    {

        private CategoryVm _category;
        public CategoryVm Category { get => _category; set => SetProperty(ref _category, value); }


        public CategoryVm InitialParentCategory { get; set; }

        public override  Task LoadData()
        {
            Category = Parameter as CategoryVm;

            if (Category?.ParentCategory != null)
            {
                InitialParentCategory = new CategoryVm() {Id = Category.ParentCategory.Id};

            }
            
            MessagingCenter.Subscribe<MessagingService, CategoryVm>(this, MessagingKeys.SelectParentKey, async (obj, item) =>
             {
               
                 Category.ParentCategory = item;
             });


            return Task.CompletedTask;
            

        }


        private Command _addItemCommand;

        public Command AddItemCommand => _addItemCommand ??= new Command(async () =>
            {

               

                await Service.AddOrUpdateCategory(Category.Map<CategoryVm, Category>());

                if ( Category.RecurringBudget.HasValue && Category.RecurringBudget!=0 && InitialParentCategory != Category.ParentCategory)
                {

                    await Service.UpdateParentCategsBudgets(InitialParentCategory.Map<CategoryVm, Category>(), Category.ParentCategory.Map<CategoryVm,Category>(),
                        Category.RecurringBudget.Value);
                }

                await NavigateBackImpl();

                await CollectionUpdateService.QueueAddOrUpdateCollectionItemTasks();

            }
        );


        private Command _selectParentCommand;


        public Command SelectParentCommand => _selectParentCommand ??= new Command(async () =>
            {
                var allCategories = (await Service.GetAllCategories())?.Select(c => c.Map<Category, CategoryVm>()).ToList();
                await NavigationService.NavigateTo<SelectParentPageModel>(new SelectParentParameter {UnselectableCategory = Category, AllCategories = allCategories });
            }
        );


    



        protected override void BeforePop()
        {
            MessagingCenter.Unsubscribe<MessagingService, CategoryVm>(this, MessagingKeys.SelectParentKey);
        }


        protected override void BeforeLoadingData()
        {

            var param = Parameter as CategoryVm;

            if (param == null || param.Id == 0)
            {
                Title = AppContent.AddCategoryPageTitle;
            }
            else
            {
                Title = AppContent.UpdateCategoryPageTitle;
            }

        }





    }


}
