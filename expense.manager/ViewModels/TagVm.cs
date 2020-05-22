using expense.manager.Utils;
using expense.manager.ViewModels.Base;

namespace expense.manager.ViewModels
{
    public class TagVm : BaseViewModel
    {
        private int _id;

        public int Id
        {
            get => _id; set
            {
                _id = value;
                SetProperty(ref _id, value);
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private double? _ammountSpent;
        private bool _isSelected;

        public double? AmmountSpent
        {
            get => _ammountSpent;
            set {

                SetProperty(ref _ammountSpent, value);
                OnPropertyChanged(nameof(AmmountFormatted));

            }
        }


        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public override bool Equals(object obj)
        {
            if (obj is TagVm other)
            {
                return other.Id == Id;
            }

            return false;
        }

 


        public string AmmountFormatted => $"{string.Format("{0:0.##}", AmmountSpent)} {AppPreferences.CurrentCurrency?.symbol}";

    }
}
