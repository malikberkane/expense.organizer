using Xamarin.Forms;

namespace expense.manager.Services
{
    public class MessagingService: IMessagingService
    {
        public void Send<T>(T message, string key)
        {
            MessagingCenter.Send(this, key, message);
        }

        public void Send(string key)
        {
            MessagingCenter.Send(this, key);
        }
    }

    public interface IMessagingService
    {
        void Send<T>(T message, string key);
        void Send(string key);
    }
}
