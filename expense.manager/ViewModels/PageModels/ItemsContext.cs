using expense.manager.ViewModels.Base;
using System;

namespace expense.manager.ViewModels.PageModels
{
    public class ItemsContext : BaseViewModel
    {
        private string monthId;
        private DateTime contextDate;

        public CategoryVm Category { get; set; }

        public string MonthId { get => monthId; set => SetProperty(ref monthId, value); }


        public DateTime ContextDate
        {
            get => contextDate;
            set
            {
                contextDate = value;
                MonthId = contextDate.ToMonthId();
            }

        }

    }
}