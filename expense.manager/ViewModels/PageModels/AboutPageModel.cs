using expense.manager.Models;
using expense.manager.ViewModels.Base;
using Xamarin.Forms;


namespace expense.manager.ViewModels.PageModels
{
    public class AboutPageModel: BasePageModel
    {


        private Command _goToGithubPageCommand;

        public Command GoToGithubPageCommand => _goToGithubPageCommand ?? (
                                                    _goToGithubPageCommand = new Command(async () =>
                                                        {
                                                            await NavigationService.NavigateTo<GithubPageModel>(modal:true);
                                                        }
                                             )
                                         );
    }
}
