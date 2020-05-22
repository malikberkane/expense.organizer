using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class SelectParentPageModel: BasePageModel
    {
        public SelectParentParameter CurrentParentContext { get; private set; }
        public IEnumerable<CategoryVm> Categories { get; set; }

        public CategoryVm SelectedCategory { get; set; }
        public override Task LoadData()
        {
            var selectParentContext = Parameter as SelectParentParameter;

            CurrentParentContext = selectParentContext;
            Categories = selectParentContext?.AllCategories;
            return base.LoadData();
        }

        private Command _navigateToChildrenList;

        public Command NavigateToChildrenListCommand => _navigateToChildrenList ??= new Command<CategoryVm> (async (categ) =>
            {
                await NavigationService.NavigateTo<SelectParentPageModel>(new SelectParentParameter() { AllCategories = categ.Children});
            }
        );

        private Command _validateParentCategoryCommand;

        public Command ValidateParentCategoryCommand => _validateParentCategoryCommand ??= new Command(async (categ) =>
            {
                MessagingService.Send(SelectedCategory, MessagingKeys.SelectParentKey);

                await NavigationService.RemoveFromNavigation<SelectParentPageModel>();

            }
        );




        private Command _noParentCategoryCommand;

        public Command NoParentCategoryCommand => _noParentCategoryCommand ??= new Command(async (categ) =>
            {

                SelectedCategory = null;
                MessagingService.Send(SelectedCategory, MessagingKeys.SelectParentKey);

                await NavigationService.RemoveFromNavigation<SelectParentPageModel>();

            }
        );
    }


    public class SelectParentParameter
    {
        public int LevelId { get; set; }

        public IEnumerable<CategoryVm> AllCategories { get; set; }
    }
}
