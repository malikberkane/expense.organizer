using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace expense.manager.ViewModels.PageModels
{
    public class ExpenseByDatePageModel : ItemsPageModel
    {
        private ICollection<GroupedExpenses> groupedExpenses;

        public ICollection<GroupedExpenses> GroupedExpenses { get => groupedExpenses; set { SetProperty(ref groupedExpenses, value); } }
        protected override async Task<IEnumerable<BaseViewModel>> LoadItems()
        {
            var result = new List<BaseViewModel>();

            var expenses = (await Service.GetExpensesRecap(ItemsContext.MonthId, -1)).Select(n => n.Map<Expense, ExpenseVm>());

            if (expenses != null && expenses.Any())
            {
                result.AddRange(expenses);
            }

            return result;
        }


        protected override void AfterLoadingData()
        {
            base.AfterLoadingData();

            var groups = Items.Select(t => t as ExpenseVm).GroupBy(n => (n.CreationDate.Date));

            GroupedExpenses = groups.Select(n => new GroupedExpenses(n.Key, n)).ToList();


        }

    }
}