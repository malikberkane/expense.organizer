using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace expense.manager.Data
{
    public class CategoryData
    {

        public int Id { get; set; }

        public int ParentId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }

        public double RecurringBudget { get; set; }

    }
}