using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class TagChoicePageModel : BasePageModel
    {

        public ObservableCollection<TagVm> Tags { get => _tags; set { SetProperty(ref _tags, value); } }



        public TagVm SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        private TagVm selectedItem;


        public Command ItemSelectionCommand => _selectionChangedCommand ?? (
                                    _selectionChangedCommand = new Command( () =>
                                    {
                                        if (SelectedItem != null)
                                        {
                                            SelectedItem.IsSelected = !SelectedItem.IsSelected;

                                            if (SelectedItem.IsSelected && !SelectedTags.Contains(SelectedItem))
                                            {
                                                SelectedTags.Add(SelectedItem);
                                            }
                                            else if(!SelectedItem.IsSelected && SelectedTags.Contains(SelectedItem))
                                            {
                                                SelectedTags.Remove(SelectedItem);
                                            }
                                        }

                                        SelectedItem = null;


                                    }
                                    )
                                );


        public override async Task LoadData()
        {
            await base.LoadData();
            SelectedTags = Parameter as ObservableCollection<TagVm>;

            var tags = (await Service.GetTags(computeAmmounts: false))?.Select(tag => 
            {
                var result= tag.Map<Tag, TagVm>();
                result.IsSelected = SelectedTags.Contains(result);

                return result;
            }
            ).ToList();


            Tags = new ObservableCollection<TagVm>(tags);



        }




        private Command _validateSelectionCommand;
        private ObservableCollection<TagVm> _tags;
        private Command _selectionChangedCommand;
        private ObservableCollection<TagVm> SelectedTags { get; set; }

        public Command ValidateSelectionCommand => _validateSelectionCommand ?? (
                                             _validateSelectionCommand = new Command(async () =>
                                             {
                                                 await NavigationService.PopAsync();
                                             }
                                             )
                                         );




    }

}
