using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace expense.manager.ViewModels.Base
{
    public class BaseViewModel: INotifyPropertyChanged
    {
        protected bool SetProperty<T>(ref T backingStore, T value,
            Action onChanged = null, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public override bool Equals(object obj)
        {
            if(obj is CategoryVm category && this is CategoryVm currentCategory)
            {
                return category.Id == currentCategory.Id;
            }

            if (obj is ExpenseVm expense && this is ExpenseVm currentExpense)
            {
                return expense.Id == currentExpense.Id;
            }

            return base.Equals(obj);
        }


        public void RaiseAllPropertiesChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}