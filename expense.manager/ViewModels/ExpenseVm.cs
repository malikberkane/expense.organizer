using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using expense.manager.Annotations;
using expense.manager.Models;
using expense.manager.Utils;
using expense.manager.ViewModels.Base;

namespace expense.manager.ViewModels
{
    public class ExpenseVm: BaseViewModel
    {
        private int _id;
        private DateTime _creationDate;
        private string _expenseLabel;
        private int _categoryId;
        private IEnumerable<Tag> _tagList;
        private string _name;
        private double _ammount;

        public bool ShowCompleteDate { get; set; } = true;

        public bool HasDateValue => !string.IsNullOrEmpty(DateFormat);

        public string DateFormat => ExpenseDateFormat();

        private string ExpenseDateFormat()
        {
            if (ShowCompleteDate)
            {
                return CreationDate.TimeOfDay.Ticks > 0 ? $"{CreationDate:dd/MM/yy HH:mm}" : $"{CreationDate:dd/MM/yy}";
            }
            else
            {
                return CreationDate.TimeOfDay.Ticks > 0 ? $"{CreationDate:HH:mm}" : null;

            }
        }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public DateTime CreationDate
        {
            get => _creationDate;
            set
            {
                SetProperty(ref _creationDate, value);
                OnPropertyChanged(nameof(DateFormat));
                OnPropertyChanged(nameof(HasDateValue));

            }
        }


        public string ExpenseLabel
        {
            get => _expenseLabel;
            set
            {
                SetProperty(ref _expenseLabel, value);

            }
        }

        public int CategoryId
        {
            get => _categoryId;
            set => SetProperty(ref _categoryId,value);
        }

        public IEnumerable<Tag> TagList
        {
            get => _tagList;
            set => SetProperty(ref _tagList,value);
        }


        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        public double Ammount
        {
            get => _ammount;
            set
            {
                SetProperty(ref _ammount, value);
                OnPropertyChanged(nameof(AmmountFormatted));
            }
        }


        public int PreviousCategId { get; set; } = -1;

        public double InitialAmmount { get; set; } 



     

        public string AmmountFormatted => $"{string.Format("{0:0.##}",Ammount)} {AppPreferences.CurrentCurrency?.symbol}";

    }
}