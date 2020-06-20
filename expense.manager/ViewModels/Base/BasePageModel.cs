using System;
using System.Threading.Tasks;
using expense.manager.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace expense.manager.ViewModels.Base
{
    public class BasePageModel : BaseViewModel
    {
        public IExpenseManagerService Service => DependencyService.Get<IExpenseManagerService>();

        public IMessagingService MessagingService => DependencyService.Get<IMessagingService>();


        protected CollectionUpdateService CollectionUpdateService { get; set; }


        public Page CurrentPage { get; set; }
        public object Parameter { get; set; }

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }



        public NavigationService NavigationService { get; set; }
        public virtual async Task Initialize()
        {
            NavigationService = new NavigationService();
            CollectionUpdateService = new CollectionUpdateService();
            BeforeLoadingData();
            await EnsureIsBusy(() => LoadData());
            AfterLoadingData();
        }


    


        protected virtual void BeforeLoadingData()
        {
        }

        protected virtual void  AfterLoadingData()
        {
            
        }

        public virtual Task LoadData()
        {
            return Task.CompletedTask;
        }




        public virtual bool HandleBackCommandImpl { get; protected set; } = true;


        private Command _backCommand;

        public Command BackCommand => _backCommand ?? (
                                             _backCommand = new Command(async () => await NavigateBackImpl())

                                         );


        public virtual Task NavigateBackImpl(bool modal = false)
        {
            return CleanPopAsync(modal);
        }

        protected async Task CleanPopAsync(bool modal = false)
        {
            BeforePop();
            await NavigationService.PopAsync(modal);
        }


        protected virtual void BeforePop()
        {
        }


        protected async Task EnsureIsBusy(Func<Task> action)
        {
            if (this.IsBusy)
            {
                return;
            }



            var mainThread = MainThread.IsMainThread;

            MainThread.BeginInvokeOnMainThread(()=>
            {
                this.IsBusy = true;

            });
            await action();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.IsBusy = false;

            });

        }



    


    }



}
