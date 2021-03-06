﻿using System;
using System.Collections.Generic;
using expense.manager.Resources;
using expense.manager.Utils;
using expense.manager.ViewModels.Base;
using Xamarin.Essentials;

namespace expense.manager.ViewModels
{
    public class CategoryVm : BaseViewModel
    {
        private int _id;
        private CategoryVm _parentCategory;
        private List<CategoryVm> _children;
        private string _name;
        private double? _ammountSpent;
        private double? _budget;
        private double? _recurringBudget;

        

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                SetProperty(ref _id, value);
            }
        }

        public CategoryVm ParentCategory
        {
            get => _parentCategory;
            set => SetProperty(ref _parentCategory, value);
        }

        public List<CategoryVm> Children
        {
            get => _children;
            set => SetProperty(ref _children, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public double? AmmountSpent
        {
            get => _ammountSpent;
            set {
                SetProperty(ref _ammountSpent, value);
                OnPropertyChanged(nameof(SpentBudgetRatio));
                OnPropertyChanged(nameof(AmmountFormatted));
                }
        }

        public double? Budget
        {
            get => _budget;
            set
            {
                SetProperty(ref _budget, value);
                OnPropertyChanged(nameof(HasBudget));
                OnPropertyChanged(nameof(SpentBudgetRatio));
            }
        }




        public double? RecurringBudget { get => _recurringBudget; set => SetProperty(ref _recurringBudget, value); }



        public string SpentBudgetRatio => Budget != 0 && AmmountSpent.HasValue ? $"{string.Format("{0:0.##}", ((AmmountSpent / Budget) * 100).Value)}% {AppContent.OfBudget}" : null;




        public bool HasBudget => Budget != null && Budget != 0;



        public string AmmountFormatted => AmmountSpent.HasValue && AmmountSpent!=0  ? $"{AmmountSpent:0.##} {AppPreferences.CurrentCurrency?.symbol}" :null;


        public override bool Equals(object obj)
        {
            if (obj is CategoryVm otherCateg)
            {
                return this.Id == otherCateg.Id;

            }

            return false;
        }
    }
}