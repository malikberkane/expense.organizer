using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace expense.manager.Data
{
    public class ExpenseData
    {
        public int Id { get; set; }
        public double Ammount { get; set; }
        public DateTime CreationDate { get; set; }
        public string ExpenseLabel { get; set; }
        public int CategoryId { get; set; }
        public double RecurringBudget { get; set; }

        public string MonthId { get; set; }

    }
}