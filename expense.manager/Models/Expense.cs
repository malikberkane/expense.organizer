using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using expense.manager.Annotations;

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


        public List<Tag> TagList { get; set; } = new List<Tag>();


        public string Name { get; set; }

        public double Ammount { get; set; }


    }
}