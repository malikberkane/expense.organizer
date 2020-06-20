using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class ExpenseByDatePageModel : ItemsPageModel
    {
        private ICollection<GroupedExpenses> groupedExpenses;

        private int currentPage = 1;

        public bool CanLoadPrevious { get => canLoadPrevious; set { SetProperty(ref canLoadPrevious, value); } }

        private bool canLoadPrevious;

        public ICollection<GroupedExpenses> GroupedExpenses { get => groupedExpenses; set { SetProperty(ref groupedExpenses, value); } }
        protected override async Task<IEnumerable<BaseViewModel>> LoadItems()
        {
            var result = new List<BaseViewModel>();


                currentPage = 1;

                var expenses = (await Service.GetPagedExpense(currentPage)).Select(n => {
                    var result = n.Map<Expense, ExpenseVm>();
                    result.ShowCompleteDate = false ;
                    return result;
                    });
                if (expenses != null && expenses.Any())
                {
                    result.AddRange(expenses);
                }


            CanLoadPrevious = (await Service.GetPagedExpense(currentPage+1)).Any();



            return result;
        }



        protected override async Task AddItemImpl()
        {
            await NavigationService.NavigateTo<AddExpensePageModel>(
                              new ExpenseVm()
                              {
                                  CreationDate = DateTime.Now,
                                  CategoryId = ItemsContext.Category.Id
                              });
        }



        protected override void AfterLoadingData()
        {
            base.AfterLoadingData();

            var groups = Items.Select(t => t as ExpenseVm).GroupBy(n => (n.CreationDate.Date)).Select(n => new GroupedExpenses(n.Key, n));


            if(groups!= null)
            {
                GroupedExpenses = new ObservableCollection<GroupedExpenses>(groups);
            }

        }


        public bool AllExpensesDisplayed { get; set; }
        private Command _loadMoreExpensesCommand;

        public Command LoadMoreExpensesCommand => _loadMoreExpensesCommand ?? (
                                             _loadMoreExpensesCommand = new Command(async (monthId) => await EnsureIsBusy(()=>   LoadMoreExpensesImpl())


                                             ));




        public async Task LoadMoreExpensesImpl()
        {

                currentPage++;
                 
          
                var expenses = (await Service.GetPagedExpense(currentPage)).Select(n => {
                    var result = n.Map<Expense, ExpenseVm>();
                    result.ShowCompleteDate = false;
                    return result;
                });
                if (expenses != null && expenses.Any())
                {
                    foreach (var expense in expenses)
                    {
                        Items.Add(expense);
                    }

                    var groups = expenses.GroupBy(n => (n.CreationDate.Date)).Select(n => new GroupedExpenses(n.Key, n)).ToList();


                    foreach (var group in groups)
                    {


                    var existingGroup = GroupedExpenses.SingleOrDefault(g => g.CreationDate == group.CreationDate);
                    if (existingGroup != null)
                    {
                        existingGroup.AddRange(group);

              
                    }
                    else
                    {
                        GroupedExpenses.Add(group);

                    }


                }
                  

                }



               CanLoadPrevious = (await Service.GetPagedExpense(currentPage + 1)).Any();



        }

    }
}