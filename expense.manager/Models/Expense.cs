using System;
using System.Collections.Generic;

namespace expense.manager.Models
{
    public class Expense
    {

        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public string ExpenseLabel { get; set; }
        public int CategoryId { get; set; }

        public int PreviousCategId { get; set; } = -1;
        public double InitialAmmount { get; set; }


        public ICollection<Tag> TagList { get; set; } = new List<Tag>();


        public string Name { get; set; }

        public double Ammount { get; set; }


    }
}