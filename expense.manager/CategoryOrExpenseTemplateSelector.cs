using expense.manager.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace expense.manager
{
    public class CategoryOrExpenseCellTemplateSelector: DataTemplateSelector
    {
        public DataTemplate CategoryTemplate { get; set; }
        public DataTemplate ExpenseTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
              return item is ExpenseVm ? ExpenseTemplate : CategoryTemplate;
        }
    }
}
