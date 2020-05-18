using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.Services;
using expense.manager.ViewModels.Base;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class AddCategoryPageModel : BasePageModel
    {

        private CategoryVm _category;
        private CategoryVm parentCategory;
        public CategoryVm Category { get => _category; set => SetProperty(ref _category, value); }
        public string CurrentMonthId { get => _currentMonthId; set => SetProperty(ref _currentMonthId, value); }
        public CategoryEditionTracker CategoryEditionTracker { get; private set; }

        public double? SpecifiedBudget { get => _specifiedBudget; set => SetProperty(ref _specifiedBudget, value); }

        public CategoryVm ParentCategory { get => parentCategory; set => SetProperty(ref parentCategory, value); }

        private double? initialRecurringBudget;
        private double? initialSpecialBudget;
        private CategoryVm initialParentCategory;


        public override async Task LoadData()
        {
            var param = Parameter as (CategoryVm categ, string monthId)?;



            if (param.HasValue && param.Value.categ is CategoryVm categoryToEdit)
            {
                Category = categoryToEdit;

                CurrentMonthId = param.Value.monthId;



                SpecifiedBudget = await Service.GetSpecifiedBudget(Category.Map<CategoryVm, Category>(), CurrentMonthId);

                if (Category.ParentCategory != null)
                {
                    var currentParentCategory = await Service.GetCategory(Category.ParentCategory.Id);

                    ParentCategory = currentParentCategory?.Map<Category, CategoryVm>();
                }


                initialParentCategory = ParentCategory ?? new CategoryVm();

                if (Category?.RecurringBudget == 0)
                {
                    Category.RecurringBudget = null;
                }
                initialRecurringBudget = Category?.RecurringBudget;
                initialSpecialBudget = SpecifiedBudget;
            }


            MessagingCenter.Subscribe<MessagingService, CategoryVm>(this, MessagingKeys.SelectParentKey, async (obj, item) =>
             {
               
                 ParentCategory = item;
                 Category.ParentCategory = item;
             });




        }


        private Command _addItemCommand;

        public Command AddItemCommand => _addItemCommand ?? (
                                             _addItemCommand = new Command(async () =>
                                                 {

                                                     CategoryEditionTracker = new CategoryEditionTracker
                                                     {
                                                         RecurringBudget = (initialRecurringBudget, Category.RecurringBudget),
                                                         SpecialBudget = (initialSpecialBudget, SpecifiedBudget),
                                                         ParentCategory = (initialParentCategory.Map<CategoryVm, Category>(), ParentCategory?.Map<CategoryVm, Category>() ?? new Category())
                                                     };


                                                     var resultSave = await Service.AddCategory(Category.Map<CategoryVm, Category>(), CategoryEditionTracker, CurrentMonthId);
                                                     Category.Id = resultSave;


                                                     await NavigateBackImpl();

                                                     await CollectionUpdateService.QueueAddOrUpdateCollectionItemTasks(Category);

                                                 }
                                             )
                                         );


        private Command _selectParentCommand;
        private Command _displaySpecialBudgetDialog;

        private string _currentMonthId;
        private double? _specifiedBudget;

        public Command SelectParentCommand => _selectParentCommand ?? (
                                             _selectParentCommand = new Command(async () =>
                                             {
                                                 var allCategories = await Service.GetAllCategories();
                                                 await NavigationService.NavigateTo<SelectParentPageModel>(new SelectParentParameter() { LevelId = 0, AllCategories = allCategories });
                                             }
                                             )
                                         );


        public Command DisplaySpecialBudgetDialog => _displaySpecialBudgetDialog ?? (
                                     _displaySpecialBudgetDialog = new Command(async () =>
                                     {
                                         await NavigationService.DisplayAlert(AppContent.SpecificBudgetNote, title:AppContent.Info);
                                     }
                                     )
                                 );



        protected override void BeforePop()
        {
            MessagingCenter.Unsubscribe<MessagingService, CategoryVm>(this, MessagingKeys.SelectParentKey);
        }


        protected override void BeforeLoadingData()
        {

            var param = Parameter as (CategoryVm categ, string monthId)?;

            if (param?.categ == null || param.Value.categ.Id == 0)
            {
                Title = AppContent.AddCategoryPageTitle;
            }
            else
            {
                Title = AppContent.UpdateCategoryPageTitle;
            }

        }





    }


    public class CategoryEditionTracker
    {

        public (double? initial, double? final) RecurringBudget { get; set; }

        public (double? initial, double? final) SpecialBudget { get; set; }

        public (Category initial, Category final) ParentCategory { get; set; }


    }
}
