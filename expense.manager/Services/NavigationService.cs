using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using expense.manager.ViewModels;
using expense.manager.ViewModels.Base;
using Xamarin.Forms;

namespace expense.manager.Services
{
    public class NavigationService
    {
        public Task NavigateTo<T>(object parameter = null, bool modal = false) where T : BasePageModel
        {
            var pageModel = Activator.CreateInstance<T>();

            var pageType = GetPageTypeName(pageModel.GetType());

            var page = (Page)Activator.CreateInstance(pageType);

            pageModel.Parameter = parameter;
            pageModel.CurrentPage = page;
            pageModel.Initialize();

            

            page.BindingContext = pageModel;

            if (pageModel.HandleBackCommandImpl)
            {
                var backButtonBehavior = new BackButtonBehavior();

                backButtonBehavior.SetBinding(BackButtonBehavior.CommandProperty, new Binding
                {
                    Path = nameof(BasePageModel.BackCommand),
                    Source = page.BindingContext,
                    Mode = BindingMode.OneWay

                });

                Shell.SetBackButtonBehavior(page, backButtonBehavior);

            }
           

            if (modal)
            {
                Shell.Current.Navigation.PushModalAsync(page, animated: false);

            }
            else
            {
                Shell.Current.Navigation.PushAsync(page, animated: false);

            }
            return Task.CompletedTask;
        }



        public Task RemoveFromNavigation<T>() where T : BasePageModel
        {

            var stack = Shell.Current.Navigation.NavigationStack.ToList();
            foreach (var page in stack)
            {
                if (page != null && stack.IndexOf(page) != stack.Count - 1 && page.BindingContext.GetType() == typeof(T))
                {
                    Shell.Current.Navigation.RemovePage(page);
                }
            }

            Shell.Current.Navigation.PopAsync(false);


            return Task.CompletedTask;
        }
        public Task DisplayAlert(string message, string title = null, string okMessage = null)
        {
            return Shell.Current.DisplayAlert(title ?? AppContent.Alert, message, okMessage ?? AppContent.Ok);
        }

        public Task<bool> DisplayYesNoMessage(string content)
        {
            return Shell.Current.DisplayAlert(AppContent.Alert, content, AppContent.Yes, AppContent.No);
        }

        public Task<string> DisplayPrompt(string content, string placeholder = null)
        {
            return Shell.Current.DisplayPromptAsync(string.Empty, content, cancel: AppContent.Cancel, initialValue: placeholder);
        }

        public Task<string> DisplayActionSheet(string[] actions)
        {
            return Shell.Current.DisplayActionSheet(string.Empty,AppContent.Cancel, string.Empty, actions);
        }

        public Task PopAsync(bool modal = false)
        {

            if (Shell.Current.Navigation.NavigationStack.Count <= 1)
            {
                return Task.CompletedTask;
            }

            if (modal)
            {

                return Shell.Current.Navigation.PopModalAsync(false);



            }
            else
            {
                return Shell.Current.Navigation.PopAsync(false);

            }




        }


        public Task PopToRoot()
        {

            if (Shell.Current.Navigation.NavigationStack.Count <= 1)
            {
                return Task.CompletedTask;
            }


            return Shell.Current.Navigation.PopToRootAsync(false);


        }






        Type GetPageTypeName(Type pageModelType)
        {
            var pageTypeName = pageModelType?.AssemblyQualifiedName?
                .Replace("PageModel", "Page")
                .Replace("ViewModel", "View");

            return pageTypeName != null ? Type.GetType(pageTypeName) : null;


        }
    }


}