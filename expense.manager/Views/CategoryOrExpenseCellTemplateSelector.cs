using expense.manager.ViewModels;
using Xamarin.Forms;

namespace expense.manager.Views
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
