using expense.manager.Mapping;
using expense.manager.Models;
using expense.manager.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using expense.manager.Resources;
using expense.manager.Utils;
using Xamarin.Forms;

namespace expense.manager.ViewModels.PageModels
{
    public class TagRecapPageModel : ListPageModel<TagVm>
    {
        protected override async Task AddItemImpl()
        {
            var editedTagName = await NavigationService.DisplayPrompt(AppContent.EnterTagNamePrompt);

            if (!string.IsNullOrEmpty(editedTagName))
            {
                var newId = await Service.AddTag(new Tag { Name = editedTagName });
                Items.Add(new TagVm { Id = newId, Name = editedTagName });
            }
        }

        protected override async Task DeleteItemImpl(TagVm item)
        {
            if (await NavigationService.DisplayYesNoMessage(AppContent.DeleteTagConfirmation))
            {
                await Service.DeleteTag(item.Map<TagVm, Tag>());
                Items.Remove(item);

            }           
        }

        protected override async Task EditItemImpl(TagVm item)
        {
            var editedTagName = await NavigationService.DisplayPrompt(AppContent.EnterTagNamePrompt, item.Name);
            if (!string.IsNullOrEmpty(editedTagName))
            {
                item.Name = editedTagName;

                await Service.AddTag(item.Map<TagVm,Tag>());

                             
            }
        }

        protected override async Task ItemSelectionImpl()
        {
            if (SelectedItem == null)
            {
                return;
            }

            await NavigationService.NavigateTo<TagRecapDetailedPageModel>(SelectedItem);
        }

        protected override async Task<IEnumerable<TagVm>> LoadItems()
        {
            var tags = await Service.GetTags();

            if (tags != null)
            {
                return tags.Select(tag => tag.Map<Tag, TagVm>()).ToList();
            }

            return null;
        }

        public override async Task LoadData()
        {
            Items = new ObservableCollection<TagVm>();



            var items = await LoadItems();
            if (items != null && items.Any())
            {
                Items = new ObservableCollection<TagVm>(items);
            }
        }

        protected override void BeforeLoadingData()
        {
            base.BeforeLoadingData();

            MessagingCenter.Subscribe<MessagingService>(this, MessagingKeys.CurrencyChange, (sender) =>
            {
                if (Items != null)
                {
                    foreach (var cell in Items)
                    {
                        cell.RaiseAllPropertiesChanged();
                    }
                }
              
            });
        }


     
    }

}
