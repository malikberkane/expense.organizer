using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace expense.manager.ViewModels.PageModels
{
    public class TagRecapDetailedPageModel : ListPageModel<ExpenseVm>
    {


   

        public TagVm CurrentTag { get; set; }
        protected override async Task<IEnumerable<ExpenseVm>> LoadItems()
        {

            if (CurrentTag != null)
            {
                var result = new List<ExpenseVm>();
                var expenses = (await Service.GetExpensesByTag(CurrentTag.Id)).Select(n => n.Map<Expense, ExpenseVm>());


                foreach (var item in expenses)
                {
                    result.Add(item);
                }

                return result;
            }

            return null;
        }


        protected override async Task DeleteItemImpl(ExpenseVm item)
        {

            
            if (item == null)
            {
                return;
            }

            if (await NavigationService.DisplayYesNoMessage(AppContent.UntagExpenseConfirmation))
            {
                await Service.UntagExpense(CurrentTag.Map<TagVm, Tag>(), item.Map<ExpenseVm, Expense>());

                Items.Remove(item); 
            }



            await CollectionUpdateService.QueueAddOrUpdateCollectionItemTasks(item);

        }

    

        public override Task LoadData()
        {
            CurrentTag = Parameter as TagVm;
            return base.LoadData();

        }

        protected override Task ItemSelectionImpl()
        {
            return Task.CompletedTask;
        }

        protected override Task AddItemImpl()
        {
            return Task.CompletedTask;

        }

        protected override async Task EditItemImpl(ExpenseVm item)
        {
            await NavigationService.NavigateTo<AddExpensePageModel>(item);
        }
    }
}
