using System;
using System.Collections.Generic;

namespace expense.manager.ViewModels.PageModels
{
    public class GroupedExpenses : List<ExpenseVm>
    {
        public DateTime CreationDate { get; set; }


        public GroupedExpenses(DateTime date, IEnumerable<ExpenseVm> expenses):base(expenses)
        {
            CreationDate = date;
        }
    }
}