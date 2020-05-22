using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class AddExpensePageModel : BasePageModel
    {
        public ExpenseVm Expense { get; set; }
        private CategoryVm _parentCategory;

        public ICollection<TagVm> LinkedTags { get => _linkedTags; set => SetProperty(ref _linkedTags, value);
        }


        public CategoryVm ParentCategory { get => _parentCategory; set => SetProperty(ref _parentCategory, value); }

        public override async Task LoadData()
        {
            Expense = Parameter as ExpenseVm;

            if (Expense != null)
            {
                Expense.InitialAmmount = Expense.Ammount;
            }



            (Category parentCateg, IEnumerable<Tag> linkedTags) ExpenseInfo =
                await Task.Run(async () =>
                {
                    Category  categ= null;
                    if (Expense.CategoryId != 0)
                    {
                        Expense.PreviousCategId = Expense.CategoryId;
                        categ = await Service.GetCategory(Expense.CategoryId);

                    }

                    var localTags = (await Service.GetTagsForExpense(Expense.Id));


                    return (categ, localTags);
                });



            ParentCategory = ExpenseInfo.parentCateg?.Map<Category, CategoryVm>();

            if (ExpenseInfo.linkedTags != null)
            {
                LinkedTags = new ObservableCollection<TagVm>(ExpenseInfo.linkedTags.Select(t => t.Map<Tag, TagVm>()));
            }
            else
            {
                LinkedTags = new ObservableCollection<TagVm>();
            }


            MessagingCenter.Subscribe<MessagingService, CategoryVm>(this, MessagingKeys.SelectParentKey, async (obj, item) =>
            {
                ParentCategory = item;
                Expense.CategoryId = ParentCategory?.Id ?? 0;
            });

            OnPropertyChanged(nameof(ParentCategory));

        }


        private Command _addItemCommand;

        public Command AddItemCommand => this._addItemCommand ??= new Command(async () =>
            {


                if (!EnsureExpenseValid())
                {
                    await NavigationService.DisplayAlert(AppContent.InvalidItemAlert);
                    return;

                }

                await AddExpenseImpl();
                await CleanPopAsync();
                await CollectionUpdateService.QueueAddOrUpdateCollectionItemTasks();

            }
        );

        private async Task AddExpenseImpl()
        {
            Expense.TagList = LinkedTags.Select(tag => tag.Map<TagVm, Tag>());
            await Service.AddOrUpdateExpense(Expense.Map<ExpenseVm, Expense>());
        }


        public bool EnsureExpenseValid()
        {
            return (Expense.Ammount != 0 && !string.IsNullOrWhiteSpace(Expense.ExpenseLabel));
           
        }

        private Command _selectParentCommand;


        public Command SelectParentCommand => _selectParentCommand ??= new Command(async () =>
            {
                var allCategories = (await Service.GetAllCategories())?.Select(c=>c.Map<Category,CategoryVm>())?.ToList();

                if (allCategories == null || !allCategories.Any())
                {
                    await NavigationService.DisplayAlert(AppContent.NoCategoriesAlert);
                    return;
                }
                await NavigationService.NavigateTo<SelectParentPageModel>(new SelectParentParameter() {AllCategories = allCategories });
            }
        );



        private Command _addTagsCommand;
        private ICollection<TagVm> _linkedTags;

        public Command AddTagsCommand => _addTagsCommand ??= new Command(async () =>
            {
                await NavigationService.NavigateTo<TagChoicePageModel>(LinkedTags);
            }
        );



        private Command _removeTagCommand;


        public Command RemoveTagCommand => _removeTagCommand ??= new Command<TagVm>(async (tag) =>
            {
                if(await NavigationService.DisplayYesNoMessage(AppContent.RemoveTagPrompt))
                {
                    LinkedTags.Remove(tag);
                }
            }
        );


        protected override void BeforePop()
        {
            MessagingCenter.Unsubscribe<MessagingService, CategoryVm>(this, MessagingKeys.SelectParentKey);
        }



        protected override void BeforeLoadingData()
        {
            var expense = Parameter as ExpenseVm;

            if(expense==null || expense.Id == 0)
            {
                Title = AppContent.AddExpensePageTitle;
            }
            else
            {
                Title = AppContent.UpdateExpensePageTitle;
            }

        }

    }
}