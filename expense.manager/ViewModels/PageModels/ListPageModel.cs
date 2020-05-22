using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using expense.manager.Services;
using expense.manager.ViewModels.Base;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public abstract class ListPageModel<T> : BasePageModel where T: class
    {
        public ICollection<T> Items { get => _items; set => SetProperty(ref _items, value); }
        public Command LoadItemsCommand { get; set; }


        public T SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }


        private Command _itemSelectionCommand;

        public Command ItemSelectionCommand => this._itemSelectionCommand ?? (
                                            this._itemSelectionCommand = new Command(async () =>
                                            {
                                                await ItemSelectionImpl();

                                                SelectedItem = null;


                                            }
                                            )
                                        );

        protected abstract  Task ItemSelectionImpl();

        private Command _addItemCommand;


        public Command AddItemCommand => this._addItemCommand ?? (
                                             this._addItemCommand = new Command(async () =>
                                             {
                                                 await AddItemImpl();

                                             }
                                                   )
                                               );

        protected abstract Task AddItemImpl();

       



        private T selectedItem;
        private ICollection<T> _items;


        private Command editItemCommand;

        public Command EditItemCommand => editItemCommand ?? (
                                             editItemCommand = new Command<T>(async (item) => await EditItemImpl(item)));

        protected  abstract Task EditItemImpl(T item);

        private Command _deleteItemCommand;

        public Command DeleteItemCommand => _deleteItemCommand ?? (
                                             _deleteItemCommand = new Command<T>(async (item) =>  await DeleteItemImpl(item)));

        protected  abstract Task DeleteItemImpl(T item);







        public override async Task LoadData()
        {


            if (Items == null)
            {
                Items = new ObservableCollection<T>();

            }



            var items = await Task.Run(async () => await LoadItems());
                if (items != null)
            {
                Items = new ObservableCollection<T>(items);
            }
        }

        protected abstract Task<IEnumerable<T>> LoadItems();

   







        protected override void BeforePop()
        {
            MessagingCenter.Unsubscribe<MessagingService, BaseViewModel>(this, MessagingKeys.AddItemKey);
            MessagingCenter.Unsubscribe<MessagingService, BaseViewModel>(this, MessagingKeys.DeleteItemKey);

            MessagingCenter.Unsubscribe<MessagingService>(this, MessagingKeys.CurrencyChange);


        }

        protected override void BeforeLoadingData()
        {
            MessagingCenter.Subscribe<MessagingService>(this, MessagingKeys.AddItemKey, (sender) =>
            {
                QueueReloadTask();
            });
            MessagingCenter.Subscribe<MessagingService>(this, MessagingKeys.DeleteItemKey, (sender) =>
            {
                QueueReloadTask();
            });

        }

        private void QueueReloadTask()
        {
            var reloadTask = new Task(async () =>
            {
                await EnsureIsBusy(() => LoadData());
                AfterLoadingData();
            });
            CollectionUpdateService.QueueTask(reloadTask);
        }



    }


}