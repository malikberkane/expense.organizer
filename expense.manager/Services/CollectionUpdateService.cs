using expense.manager.ViewModels;
using expense.manager.ViewModels.Base;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace expense.manager.Services
{
    public class CollectionUpdateService
    {

        private static ConcurrentStack<Task> CollectionUpdateTasks = new ConcurrentStack<Task>();
        public IExpenseManagerService Service => DependencyService.Get<IExpenseManagerService>();

        public IMessagingService MessagingService => DependencyService.Get<IMessagingService>();





        public void QueueTask(Task action)
        {

            CollectionUpdateTasks.Push(action);
            
        }

        public void Dequeue()
        {
            while(CollectionUpdateTasks.Any())
            {
                if (CollectionUpdateTasks.TryPop(out var action) && action != null)
                {
                    action.RunSynchronously();

                }
            }
           
        }

        public async Task QueueAddOrUpdateCollectionItemTasks()
        {
            await Task.Run(() =>
            {
                
                    MessagingService.Send(MessagingKeys.AddItemKey);
                    Dequeue();


            });


        }

        public async Task QueueDeleteCollectionItemTasks()
        {
            await Task.Run(() =>
            {

                MessagingService.Send(MessagingKeys.DeleteItemKey);
                Dequeue();


            });


        }




    }
}
