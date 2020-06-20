using System;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;

namespace expense.manager.Models
{
    public class Expense
    {

        [Name("Identifier")]

        public int Id { get; set; }

        [Name("Date")]

        public DateTime CreationDate { get; set; }



        [Name("Label")]

        public string ExpenseLabel { get; set; }
       
        [Ignore]
        public int CategoryId { get; set; }


        [Ignore]
        public int PreviousCategId { get; set; } = -1;

        public string CategoryName { get; set; }
        public ICollection<Tag> TagList { get; set; } = new List<Tag>();

        [Ignore]
        public string Name { get; set; }

        public double Ammount { get; set; }


    }
}