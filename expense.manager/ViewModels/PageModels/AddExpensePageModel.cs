using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.Services;
using expense.manager.ViewModels.Base;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class AddExpensePageModel : BasePageModel
    {
        public ExpenseVm Expense { get; set; }
        private CategoryVm parentCategory;

        public ICollection<TagVm> LinkedTags { get => linkedTags; set { SetProperty(ref linkedTags, value); } }


        public CategoryVm ParentCategory { get => parentCategory; set => SetProperty(ref parentCategory, value); }

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

        public Command AddItemCommand => this._addItemCommand ?? (
                                             this._addItemCommand = new Command(async () =>
                                             {


                                                 if (!EnsureExpenseValid())
                                                 {
                                                     await NavigationService.DisplayAlert(AppContent.InvalidItemAlert);
                                                     return;

                                                 }

                                                 await AddExpenseImpl();
                                                 await CleanPopAsync();
                                                 await CollectionUpdateService.QueueAddOrUpdateCollectionItemTasks(Expense);

                                             }
                                             )  
                                         );

        private async Task AddExpenseImpl()
        {
            Expense.TagList = LinkedTags.Select(tag => tag.Map<TagVm, Tag>());
            var resultSave = await Service.AddExpense(Expense.Map<ExpenseVm, Expense>());
            Expense.Id = resultSave;
        }


        public bool EnsureExpenseValid()
        {
            return (Expense.Ammount != 0 && !string.IsNullOrWhiteSpace(Expense.ExpenseLabel));
           
        }

        private Command _selectParentCommand;


        public Command SelectParentCommand => _selectParentCommand ?? (
                                             _selectParentCommand = new Command(async () =>
                                             {
                                                 var allCategories = await Service.GetAllCategories();
                                                 await NavigationService.NavigateTo<SelectParentPageModel>(new SelectParentParameter() { LevelId = 0, AllCategories = allCategories });
                                             }
                                             )
                                         );



        private Command _addTagsCommand;
        private ICollection<TagVm> linkedTags;

        public Command AddTagsCommand => _addTagsCommand ?? (
                                             _addTagsCommand = new Command(async () =>
                                             {
                                                 await NavigationService.NavigateTo<TagChoicePageModel>(LinkedTags);
                                             }
                                             )
                                         );



        private Command _removeTagCommand;


        public Command RemoveTagCommand => _removeTagCommand ?? (
                                             _removeTagCommand = new Command<TagVm>(async (tag) =>
                                             {
                                                 if(await NavigationService.DisplayYesNoMessage(AppContent.RemoveTagPrompt))
                                                 {
                                                     LinkedTags.Remove(tag);
                                                 }
                                             }
                                             )
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